using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Windows.Forms;
using MultiTuner.Contracts.Media;
using MultiTuner.Model.AppModel.SpotifyAppModel;
using MultiTuner.Model.JsonData.SpotifyJsonDeserializer;
using MultiTuner.Services;
using static Google.Apis.Requests.BatchRequest;


namespace MultiTuner.Services.Spotify
{

    // ============================
    // SPOTIFY IMPLEMENTATION
    // ============================

    public class SpotifyAPI : MusicServiceBase
    {
        private readonly SpotifyClient _client;
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private DateTime _tokenExpiry;

        public override MediaServiceType ServiceType => MediaServiceType.Spotify;

        public SpotifyAPI()
        {
            _client = new SpotifyClient(
                clientId: "4854448ec18f44c2b0fa2d34d50e8a4a"
            );

            _httpClient = new HttpClient();
        }

        public override async Task InitializeAsync()
        {
            if (_initialized && DateTime.UtcNow < _tokenExpiry)
                return;

            _accessToken = await _client.GetAccessTokenAsync();

            if (string.IsNullOrEmpty(_accessToken))
                throw new InvalidOperationException("Failed to obtain Spotify access token");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _accessToken);

            _tokenExpiry = DateTime.UtcNow.AddMinutes(55);
            _initialized = true;
        }

        public override async Task<List<IPlaylist>> GetUserPlaylistsAsync()
        {
            await EnsureInitializedAsync();

            var response = await _httpClient.GetAsync(
                "https://api.spotify.com/v1/me/playlists?limit=50");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<UserPlaylistsResponse>(json);

            var playlists = new List<IPlaylist>();

            foreach (var item in data.Items)
            {
                var playlist = new SpotifyPlaylist
                {
                    Id = item.Id,
                    Title = item.Name,
                    ThumbnailUrl = item.Images?.FirstOrDefault()?.Url
                };

                playlist.AttachApi(this);
                playlists.Add(playlist);
            }

            return playlists;
        }

        public override async Task<List<ITrack>> GetPlaylistTracksAsync(string playlistId)
        {
            await EnsureInitializedAsync();

            var tracks = new List<ITrack>();
            string nextUrl = $"https://api.spotify.com/v1/playlists/{playlistId}/tracks?limit=100";

            while (!string.IsNullOrEmpty(nextUrl))
            {
                var response = await _httpClient.GetAsync(nextUrl);
                if (!response.IsSuccessStatusCode) break;

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<UserTracksResponse>(json);

                if (data?.Items == null) break;

                foreach (var wrapper in data.Items)
                {
                    var track = wrapper.Track;
                    if (track == null) continue;

                    tracks.Add(new SpotifyTrack
                    {
                        Id = track.Id,
                        Title = track.Name ?? "Unknown",
                        Artist = track.Artists?.Any() == true
                            ? string.Join(", ", track.Artists.Select(a => a.Name))
                            : "Unknown Artist",
                        Album = track.Album?.Name ?? "Unknown Album",
                        ThumbnailUrl = track.Album?.Images?.FirstOrDefault()?.Url,
                        Duration = FormatDuration(track.DurationMs)
                    });
                }

                nextUrl = data.Next;
            }

            return tracks;
        }

        public override async Task<string> CreatePlaylistAsync(string playlistName, string description = null)
        {
            await EnsureInitializedAsync();

            var payload = new
            {
                name = playlistName,
                @public = true,
                description = description ?? "Synced via Multi-Service Music App"
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://api.spotify.com/v1/me/playlists",
                content);

            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception(body);

            dynamic result = JsonConvert.DeserializeObject(body);
            return result.id;
        }

        public override async Task<string> SearchTrackAsync(string title, string artist = null)
        {
            await EnsureInitializedAsync();

            var query = string.IsNullOrWhiteSpace(artist)
                ? title
                : $"{title} {artist}";

            var url = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=5";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SpotifySearchResponse>(json);

            return result?.Tracks?.Items?
                .FirstOrDefault(t => IsValidMatch(title, artist, t))?.Id;
        }

        public override bool CanResolveNatively(ITrack track)
            => track is SpotifyTrack && !string.IsNullOrWhiteSpace(track.Id);

        public override async Task PushTracksToPlaylistAsync(string playlistId, List<string> trackIds)
        {
            const int BatchSize = 100;
            var batches = trackIds.Chunk(BatchSize).ToList();

            for (int i = 0; i < batches.Count; i++)
            {
                var uris = batches[i]
                    .Select(id => id.StartsWith("spotify:track:")
                        ? id
                        : $"spotify:track:{id}")
                    .ToArray();
                
                var payload = new { uris };

                HttpResponseMessage response = i == 0
                    ? await _httpClient.PutAsJsonAsync(
                        $"https://api.spotify.com/v1/playlists/{playlistId}/tracks",
                        payload)
                    : await _httpClient.PostAsJsonAsync(
                        $"https://api.spotify.com/v1/playlists/{playlistId}/tracks",
                        payload);

                if (!response.IsSuccessStatusCode)
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }

        protected override ITrack CreateTrackFromId(string trackId, TrackDescriptor descriptor)
        {
            return new SpotifyTrack
            {
                Id = trackId,
                Title = descriptor.Title,
                Artist = descriptor.Artist,
                Album = descriptor.Album
            };
        }

        private bool IsValidMatch(string inputTitle, string inputArtist, TrackInfo candidate)
        {
            var titleA = Normalize(inputTitle);
            var titleB = Normalize(candidate.Name);

            if (!string.IsNullOrWhiteSpace(inputArtist))
            {
                bool artistMatch = candidate.Artists.Any(a =>
                    Normalize(a.Name) == Normalize(inputArtist));

                if (artistMatch)
                    return GetWords(titleA).Overlaps(GetWords(titleB));
            }

            return titleA == titleB;
        }

        private static string FormatDuration(long durationMs)
        {
            var ts = TimeSpan.FromMilliseconds(durationMs);
            return ts.TotalHours >= 1
                ? ts.ToString(@"h\:mm\:ss")
                : ts.ToString(@"m\:ss");
        }

        public override void Dispose()
        {
            _httpClient?.Dispose();
        }
    }



}
