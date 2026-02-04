using System.Diagnostics;
using System.Windows;
using MultiTuner.Services.Spotify;

namespace MultiTuner.ViewModel.ViewModelLibraryServices
{
    public class SpotifyLibraryViewModel : MediaLibraryViewModel
    {
        private readonly SpotifyAPI _spotifyApi = new();
        private readonly SpotifyLibraryProvider _provider;

        public SpotifyLibraryViewModel()
        {
            // Initialize the provider with the API
            _provider = new SpotifyLibraryProvider(_spotifyApi);

            // Start loading data immediately
            _ = LoadPlaylistsAsync();

            StatusMessage = "Waiting for request...";
        }

        /// <summary>
        /// Call this when the user clicks "Sync" or "Save" in the UI.
        /// Now purely acts as a bridge between UI and Service.
        /// </summary>
        protected override async Task SaveCurrentPlaylistAsync()
        {
            // 1. Validation: Ensure a playlist object exists
            if (SelectedPlaylist == null)
            {
                StatusMessage = "No playlist selected to save.";
                return;
            }

            StatusMessage = "Syncing with Spotify...";

            try
            {
                // 2. Snapshot: Capture the tracks currently on screen
                var tracksToSave = AllTracks.ToList();

                // 3. Delegate: Pass the ENTIRE object to the provider.
                // The Provider will inspect the 'Platform' enum and 'Id' 
                // to decide if it needs to Create or Update.
                string finalSpotifyId = await _provider.SavePlaylistAsync(SelectedPlaylist, tracksToSave);

                // 4. State Update: If the provider created a NEW playlist (e.g., from YouTube source),
                // we update our local object to reflect its new identity on Spotify.
                if (SelectedPlaylist.Id != finalSpotifyId)
                {
                    Debug.WriteLine($"[ViewModel] Updating local Playlist ID from '{SelectedPlaylist.Id}' to '{finalSpotifyId}'");

                    // Assuming 'Id' is the settable property backing 'UniqueKey'
                    SelectedPlaylist.Id = finalSpotifyId;

                    // If you have a specific property for the Platform, you might update it too
                    // e.g., SelectedPlaylist.Platform = TrackPlatform.Spotify;
                }

                // 5. Success Feedback
                StatusMessage = $"Successfully synced '{SelectedPlaylist.Title}' at {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                // UI-friendly error
                StatusMessage = $"Sync Failed: {ex.Message}";

                // Developer-friendly log
                Debug.WriteLine($"[Spotify Sync Error] Playlist: {SelectedPlaylist.Title}");
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override async Task<IEnumerable<IPlaylist>> FetchPlaylistsAsync()
        {
            try
            {
                // Fetch directly from API
                var playlists = await _spotifyApi.GetUserPlaylistsAsync();

                // Re-attach the API instance so these objects can lazy-load their tracks
                foreach (var p in playlists)
                {
                    p.AttachApi(_spotifyApi);
                }

                // Silent success (status bar / binding only)
                StatusMessage = $"Loaded {playlists.Count()} playlist(s) successfully.";

                return playlists;
            }
            catch (Exception ex)
            {
                // UI-facing error
                StatusMessage = $"Failed to load playlists: {ex.Message}";
                MessageBox.Show(
                    StatusMessage,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Developer log
                Debug.WriteLine($"[Spotify Fetch Error] {ex}");

                return Enumerable.Empty<IPlaylist>();
            }
        }


    }
}