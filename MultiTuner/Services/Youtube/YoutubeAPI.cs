using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using System.IO;
using System.Xml;
using MultiTuner.Contracts.Media;
using MultiTuner.Contracts.Media.API;
using MultiTuner.Model.AppModel.YoutubeAppModel;
using MultiTuner.RegexHelpers;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Text;




namespace MultiTuner.Services.Youtube
{
    // ============================
    // YOUTUBE IMPLEMENTATION
    // ============================

    public class YouTubeAPI : MusicServiceBase, IDownloadableService
    {
        private readonly string _downloadPath;
        private Google.Apis.YouTube.v3.YouTubeService _yt;


        // Embedded resource + AES info (hardcoded in class)
        private const string ResourceName = "MultiTuner.Authmanager.YoutubeAuth.Youtube.Credentials.enc";
        private const string Base64Key = "txma316dpBxzgvb2YXMK79KE3lPPHGkCofpESsmzGJo="; // 32-byte AES key
        private const string Base64Iv = "v7PftL/T5guP3vclkB4ARw==";  // 16-byte AES IV


        public override MediaServiceType ServiceType => MediaServiceType.YouTube;

        public YouTubeAPI(string downloadPath = null)
        {
            _downloadPath = downloadPath;
        }

        public override async Task InitializeAsync()
        {
            if (_initialized)
                return;

            var authManager = new YoutubeAuth(ResourceName, Base64Key, Base64Iv);
            UserCredential credential = await authManager.GetCredentialAsync();

            _yt = new Google.Apis.YouTube.v3.YouTubeService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Multi-Service Music App"
                });

            _initialized = true;
        }

        public override async Task<List<IPlaylist>> GetUserPlaylistsAsync()
        {
            await EnsureInitializedAsync();

            var playlists = new List<IPlaylist>();
            string nextPageToken = null;

            do
            {
                var request = _yt.Playlists.List("snippet,contentDetails");
                request.Mine = true;
                request.MaxResults = 50;
                request.PageToken = nextPageToken;

                var response = await request.ExecuteAsync();

                foreach (var playlist in response.Items)
                {
                    var ytPlaylist = new YouTubePlaylist
                    {
                        Id = playlist.Id,
                        Title = playlist.Snippet.Title,
                        Description = playlist.Snippet.Description,
                        ThumbnailUrl = playlist.Snippet.Thumbnails?.Medium?.Url
                    };

                    ytPlaylist.AttachApi(this);
                    playlists.Add(ytPlaylist);
                }

                nextPageToken = response.NextPageToken;

            } while (nextPageToken != null);

            return playlists;
        }

       
        public override async Task<List<ITrack>> GetPlaylistTracksAsync(string playlistId)
        {
            await EnsureInitializedAsync();

            var tracks = new List<ITrack>();
            string nextPageToken = null;

            do
            {
                var playlistRequest = _yt.PlaylistItems.List("snippet,contentDetails");
                playlistRequest.PlaylistId = playlistId;
                playlistRequest.MaxResults = 50;
                playlistRequest.PageToken = nextPageToken;

                var playlistResponse = await playlistRequest.ExecuteAsync();
                var ids = playlistResponse.Items.Select(i => i.ContentDetails.VideoId).ToList();

                var detailRequest = _yt.Videos.List("snippet,contentDetails");
                detailRequest.Id = string.Join(",", ids);
                var detailResponse = await detailRequest.ExecuteAsync();

                foreach (var vid in detailResponse.Items)
                {
                    var cleaned = VideoTitleCleaner.Clean(
                        vid.Snippet.Title,
                        vid.Snippet.ChannelTitle);

                    TimeSpan duration = TimeSpan.Zero;
                    if (!string.IsNullOrEmpty(vid.ContentDetails?.Duration))
                        duration = XmlConvert.ToTimeSpan(vid.ContentDetails.Duration);

                    tracks.Add(new YouTubeTrack
                    {
                        Id = vid.Id,
                        Title = cleaned.Title,
                        Artist = cleaned.Artist,
                        ThumbnailUrl = vid.Snippet.Thumbnails?.Medium?.Url,
                        Duration = $"{(int)duration.TotalMinutes}:{duration.Seconds:D2}"
                    });
                }

                nextPageToken = playlistResponse.NextPageToken;

            } while (nextPageToken != null);

            return tracks;
        }

        public override async Task<string> CreatePlaylistAsync(string playlistName, string description = null)
        {
            await EnsureInitializedAsync();

            var newPlaylist = new Google.Apis.YouTube.v3.Data.Playlist
            {
                Snippet = new PlaylistSnippet
                {
                    Title = playlistName,
                    Description = description ?? "Synced via Multi-Service Music App"
                },
                Status = new PlaylistStatus { PrivacyStatus = "public" }
            };

            var request = _yt.Playlists.Insert(newPlaylist, "snippet,status");
            var response = await request.ExecuteAsync();

            return response.Id;
        }

        public override async Task<string> SearchTrackAsync(string title, string artist = null)
        {
            await EnsureInitializedAsync();

            var query = string.IsNullOrWhiteSpace(artist)
                ? title
                : $"{title} {artist}";

            var searchRequest = _yt.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "video";
            searchRequest.MaxResults = 5;

            var searchResponse = await searchRequest.ExecuteAsync();

            return searchResponse.Items?.FirstOrDefault()?.Id?.VideoId;
        }


        // TOOL 1: Get the current "Truth" (Lightweight, no snippet details needed)
        // 1. Fetches current state using your YouTubeTrack model
        public async Task<List<YouTubeTrack>> GetPlaylistStateAsync(string playlistId)
        {
            await EnsureInitializedAsync();
            var items = new List<YouTubeTrack>();
            string nextPageToken = null;

            do
            {
                var request = _yt.PlaylistItems.List("id,snippet");
                request.PlaylistId = playlistId;
                request.MaxResults = 50;
                request.PageToken = nextPageToken;

                var response = await request.ExecuteAsync();
                foreach (var item in response.Items)
                {
                    items.Add(new YouTubeTrack
                    {
                        PlaylistItemId = item.Id,
                        Id = item.Snippet.ResourceId.VideoId, // Correctly mapping VideoId to Id
                        Position = item.Snippet.Position      // Capturing position for reorder checks
                    });
                }
                nextPageToken = response.NextPageToken;
            } while (nextPageToken != null);

            return items;
        }



        public override bool CanResolveNatively(ITrack track)
            => track is YouTubeTrack && !string.IsNullOrWhiteSpace(track.Id);


        // 2. Simple delete function
        public async Task ClearTracksAsync(IEnumerable<string> playlistItemIds)
        {
            await EnsureInitializedAsync();
            foreach (var id in playlistItemIds)
            {
                var deleteRequest = _yt.PlaylistItems.Delete(id);
                await deleteRequest.ExecuteAsync();
            }
        }



        // 3. Smart Push: Decide to Insert or Update based on PlaylistItemId
        // COMPILER FIX: Implements the abstract member from MusicServiceBase
        public override async Task PushTracksToPlaylistAsync(string playlistId, List<string> trackIds)
        {
            await EnsureInitializedAsync();

            // 1. Fetch the "Truth" (The eyes of the function)
            // This is where we get the PlaylistItemIds we need for reordering/deleting
            var currentState = await GetPlaylistStateAsync(playlistId);

            // Map: VideoId -> Queue of existing items (to handle duplicates safely)
            var inventory = currentState
                .GroupBy(x => x.Id)
                .ToDictionary(g => g.Key, g => new Queue<YouTubeTrack>(g));

            // 2. Identify and Clear "Scraps"
            // Delete items that exist on YouTube but are NOT in our new trackIds list
            var desiredIdSet = new HashSet<string>(trackIds);
            var itemsToDelete = currentState
                .Where(x => !desiredIdSet.Contains(x.Id))
                .Select(x => x.PlaylistItemId)
                .ToList();

            if (itemsToDelete.Any())
            {
                await ClearTracksAsync(itemsToDelete);
            }

            // 3. The Smart Push (Add or Reorder)
            for (int i = 0; i < trackIds.Count; i++)
            {
                string videoId = trackIds[i];

                // If the track is already in the playlist, we REORDER (Update)
                if (inventory.TryGetValue(videoId, out var queue) && queue.Count > 0)
                {
                    var existingItem = queue.Dequeue();

                    // OPTIMIZATION: Only call API if the position has actually changed
                    if (existingItem.Position != i)
                    {
                        await ReorderTrackAsync(existingItem.PlaylistItemId, videoId, playlistId, i);
                    }
                }
                // If the track is not in the inventory, we ADD (Insert)
                else
                {
                    await AddTrackAsync(videoId, playlistId, i);
                }
            }
        }

        // HELPER 1: Add new track
        private async Task AddTrackAsync(string videoId, string playlistId, int position)
        {
            var newItem = new Google.Apis.YouTube.v3.Data.PlaylistItem
            {
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    Position = (long)position,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            await _yt.PlaylistItems.Insert(newItem, "snippet").ExecuteAsync();
        }

        private async Task ReorderTrackAsync(string playlistItemId, string videoId, string playlistId, int position)
        {
            var updateItem = new Google.Apis.YouTube.v3.Data.PlaylistItem
            {
                Id = playlistItemId,
                Snippet = new PlaylistItemSnippet
                {
                    PlaylistId = playlistId,
                    Position = (long)position,
                    ResourceId = new ResourceId { Kind = "youtube#video", VideoId = videoId }
                }
            };
            await _yt.PlaylistItems.Update(updateItem, "snippet").ExecuteAsync();
        }

        protected override ITrack CreateTrackFromId(string trackId, TrackDescriptor descriptor)
        {
            return new YouTubeTrack
            {
                Id = trackId,
                Title = descriptor.Title,
                Artist = descriptor.Artist
            };
        }

        public async Task DownloadTrackAsync(string videoId, string downloadPath)
        {
            var path = downloadPath ?? _downloadPath;
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Download path not configured");

            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoId);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            if (streamInfo == null)
                throw new Exception("No audio stream available");

            string sanitizedName = SanitizeFileName(video.Title);
            string filePath = Path.Combine(path, $"{sanitizedName}.wav");

            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars));
        }

        public override void Dispose()
        {
            _yt?.Dispose();
        }
    }
}

    