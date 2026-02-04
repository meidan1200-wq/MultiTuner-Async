using AngleSharp.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MultiTuner.Contracts.Media;
using MultiTuner.Model.AppModel.YoutubeAppModel;
using MultiTuner.Services.Youtube;

namespace MultiTuner.Services.YouTube
{
    public sealed class YouTubeLibraryProvider : IPlaylistSyncService
    {
        private readonly YouTubeAPI _api;

        public YouTubeLibraryProvider(YouTubeAPI api) => _api = api;

        public async Task<string> SavePlaylistAsync(IPlaylist playlist, IReadOnlyList<ITrack> uiTracks)
        {
            // 1. Resolve the Target ID (Check if it's already a YouTube Playlist)
            string? targetId = (playlist.Platform == MediaServiceType.YouTube) ? playlist.UniqueKey : null;

            // 2. Initial Creation: If it's new or coming from Spotify/Local
            if (string.IsNullOrWhiteSpace(targetId))
            {
                targetId = await _api.CreatePlaylistAsync(playlist.Title);
            }

            try
            {
                // 3. Sync: Use the base class logic to resolve tracks and push them
                // This will internally call ResolveTrackAsync -> PushTracksToPlaylistAsync
                await _api.SyncPlaylistAsync(targetId, uiTracks);
                return targetId;
            }
            catch (Exception ex)
            {
                // 4. Recovery: If the playlist was deleted manually on YouTube (404), recreate it
                targetId = await _api.CreatePlaylistAsync(playlist.Title);
                await _api.SyncPlaylistAsync(targetId, uiTracks);
                return targetId;
            }
        }
    }
}