using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media.API
{
    /// <summary>
    /// Base interface that all music streaming services must implement
    /// </summary>
    public interface IMusicService : IDisposable
    {
        // Service Metadata
        MediaServiceType ServiceType { get; }
        bool IsInitialized { get; }

        // Authentication
        Task InitializeAsync();

        // Playlist Operations
        Task<List<IPlaylist>> GetUserPlaylistsAsync();
        Task<List<ITrack>> GetPlaylistTracksAsync(string playlistId);
        Task<string> CreatePlaylistAsync(string playlistName, string description = null);
        Task SyncPlaylistAsync(string playlistId, IReadOnlyList<ITrack> tracks);

        // Search Operations
        Task<string> SearchTrackAsync(string title, string artist = null);
        Task<ITrack> ResolveTrackAsync(TrackDescriptor descriptor, ITrack originalTrack = null);

        // Track Matching
        bool CanResolveNatively(ITrack track);
    }
}
