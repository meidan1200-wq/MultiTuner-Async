using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MultiTuner.Contracts.Media;

namespace MultiTuner.Services.Spotify
{
    public sealed class SpotifyLibraryProvider : IPlaylistSyncService
    {
        private readonly SpotifyAPI _api;

        public SpotifyLibraryProvider(SpotifyAPI api) => _api = api;

        // We now return Task<string> so the ViewModel receives the final Spotify ID
        public async Task<string> SavePlaylistAsync(IPlaylist playlist, IReadOnlyList<ITrack> orderedTracks)
        {
            string? targetId = null;

            // 1. GATEKEEPER CHECK: Is this ID actually a Spotify ID?
            // If it's YouTube, UniqueKey is a YouTube ID. We must ignore it.
            if (playlist.Platform == MediaServiceType.Spotify)
            {
                targetId = playlist.UniqueKey;
            }

            // 2. INITIAL CREATION: If no ID exists (or it's a different platform)
            if (string.IsNullOrWhiteSpace(targetId))
            {
                Debug.WriteLine($"[Provider] No Spotify ID found for '{playlist.Title}'. Creating new...");
                targetId = await _api.CreatePlaylistAsync(playlist.Title);
            }

            try
            {
                // 3. ATTEMPT PUSH: Rename your SpotifyAPI method to PushTracksToPlaylistAsync
                // to avoid naming collisions with this Provider method.
                await _api.SyncPlaylistAsync(targetId, orderedTracks);
                return targetId;
            }
            catch (Exception ex)
            {
                // 4. RECOVERY: If the ID was invalid/deleted (404), create a fresh one
                Debug.WriteLine($"[Provider] Push failed for {targetId}. Attempting recovery via re-creation.");

                targetId = await _api.CreatePlaylistAsync(playlist.Title);
                await _api.SyncPlaylistAsync(targetId, orderedTracks);

                return targetId;
            }
        }
    }


}
