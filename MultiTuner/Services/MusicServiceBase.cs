using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using MultiTuner.Contracts.Media;
using MultiTuner.Model.AppModel.SpotifyAppModel;
using MultiTuner.Model.JsonData.SpotifyJsonDeserializer;
using MultiTuner.Contracts.Media.API;

namespace MultiTuner.Services
{
    /// <summary>
    /// Base implementation providing common functionality for all music services
    /// </summary>
    public abstract class MusicServiceBase : IMusicService
    {
        protected bool _initialized;

        public abstract MediaServiceType ServiceType { get; }
        public bool IsInitialized => _initialized;

        // Abstract methods that MUST be implemented
        public abstract Task InitializeAsync();
        public abstract Task<List<IPlaylist>> GetUserPlaylistsAsync();
        public abstract Task<List<ITrack>> GetPlaylistTracksAsync(string playlistId);
        public abstract Task<string> CreatePlaylistAsync(string playlistName, string description = null);
        public abstract Task<string> SearchTrackAsync(string title, string artist = null);

        // Virtual methods with default implementation
        public virtual async Task SyncPlaylistAsync(string playlistId, IReadOnlyList<ITrack> tracks)
        {
            if (string.IsNullOrWhiteSpace(playlistId))
                throw new ArgumentException("Playlist ID cannot be empty", nameof(playlistId));

            if (tracks == null || tracks.Count == 0)
                return;

            await EnsureInitializedAsync();

            var resolvedIds = new List<string>();

            foreach (var track in tracks)
            {
                var descriptor = CreateDescriptor(track);
                var resolvedTrack = await ResolveTrackAsync(descriptor, track);

                if (resolvedTrack != null && !string.IsNullOrWhiteSpace(resolvedTrack.Id))
                    resolvedIds.Add(resolvedTrack.Id);
            }

            if (resolvedIds.Count > 0)
                await PushTracksToPlaylistAsync(playlistId, resolvedIds);
        }

        public virtual async Task<ITrack> ResolveTrackAsync(TrackDescriptor descriptor, ITrack originalTrack = null)
        {
            // If track is already from this service, return its ID
            if (originalTrack != null && CanResolveNatively(originalTrack))
                return originalTrack;

            // Otherwise search for it
            var trackId = await SearchTrackAsync(descriptor.Title, descriptor.Artist);

            if (string.IsNullOrWhiteSpace(trackId))
                return null;

            return CreateTrackFromId(trackId, descriptor);
        }

        public abstract bool CanResolveNatively(ITrack track);

        // helper methods
        public abstract Task PushTracksToPlaylistAsync(string playlistId, List<string> trackIds);
        protected abstract ITrack CreateTrackFromId(string trackId, TrackDescriptor descriptor);

        protected async Task EnsureInitializedAsync()
        {
            if (!_initialized)
                await InitializeAsync();
        }

        protected TrackDescriptor CreateDescriptor(ITrack track)
        {
            return new TrackDescriptor
            {
                Title = track.Title,
                Artist = track.Artist,
                Album = track.Album,
                Duration = TimeSpan.TryParse(track.Duration, out var d) ? d : null
            };
        }

        // String normalization utilities
        protected static string Normalize(string value)
            => value?.ToLowerInvariant().Trim().Replace("-", " ") ?? string.Empty;

        protected static HashSet<string> GetWords(string value)
            => Normalize(value)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToHashSet();

        protected static bool FuzzyMatch(string input, string candidate)
        {
            var inputWords = GetWords(input);
            var candidateWords = GetWords(candidate);
            return inputWords.Overlaps(candidateWords);
        }

        public abstract void Dispose();
    }

}
