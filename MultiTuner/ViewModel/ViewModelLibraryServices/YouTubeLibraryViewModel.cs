using System.Diagnostics;
using System.IO;
using System.Windows;
using MultiTuner.Model.AppModel.YoutubeAppModel;
using MultiTuner.Services.Youtube;
using MultiTuner.Services.YouTube;
using MultiTuner.ViewModel.ViewModelLibraryServices;



public class YouTubeLibraryViewModel : MediaLibraryViewModel
{
    private readonly YouTubeAPI _youtubeApi;
    private readonly YouTubeLibraryProvider _provider;

    public YouTubeLibraryViewModel()
    {
        _youtubeApi = new YouTubeAPI(
            @"C:\Users\Temp\Documents\Music"
        );

        // Initialize the provider with the API
        _provider = new YouTubeLibraryProvider(_youtubeApi);

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

        StatusMessage = "Syncing with YouTube...";

        try
        {
            // 2. Snapshot: Capture the tracks currently on screen
            var tracksToSave = AllTracks.ToList();

            // 3. Delegate: Pass the ENTIRE object to the provider.
            // The Provider will inspect the 'Platform' enum and 'Id' 
            // to decide if it needs to Create or Update.
            string finalYouTubeId = await _provider.SavePlaylistAsync(SelectedPlaylist, tracksToSave);

            // 4. State Update: If the provider created a NEW playlist (e.g., from Spotify source),
            // we update our local object to reflect its new identity on YouTube.
            if (SelectedPlaylist.Id != finalYouTubeId)
            {
                Debug.WriteLine($"[ViewModel] Updating local Playlist ID from '{SelectedPlaylist.Id}' to '{finalYouTubeId}'");

                // Update the ID to reflect the new YouTube playlist ID
                SelectedPlaylist.Id = finalYouTubeId;
            }

            // 5. Success Feedback
            StatusMessage = $"Successfully synced '{SelectedPlaylist.Title}' at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            // UI-friendly error
            StatusMessage = $"Sync Failed: {ex.Message}";

            // Developer-friendly log
            Debug.WriteLine($"[YouTube Sync Error] Playlist: {SelectedPlaylist.Title}");
            Debug.WriteLine(ex.ToString());
        }
    }

    protected override async Task<IEnumerable<IPlaylist>> FetchPlaylistsAsync()
    {
        try
        {
            // Fetch directly from API
            var playlists = await _youtubeApi.GetUserPlaylistsAsync();

            // Re-attach the API instance so these objects can lazy-load their tracks
            foreach (var p in playlists)
            {
                p.AttachApi(_youtubeApi);
            }

            // Silent success
            StatusMessage = $"Loaded {playlists.Count()} playlist(s) successfully.";

            return playlists;
        }
        catch (Exception ex)
        {
            // UI-facing error
            StatusMessage = $"Failed to load YouTube playlists: {ex.Message}";
            MessageBox.Show(
                StatusMessage,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Developer log
            Debug.WriteLine($"[YouTube Fetch Error] {ex}");

            return Enumerable.Empty<IPlaylist>();
        }
    }

}
