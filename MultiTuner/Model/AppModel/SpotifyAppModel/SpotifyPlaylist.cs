using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MultiTuner.Contracts.Media;
using MultiTuner.Contracts.Media.API;
using MultiTuner.Services.Spotify;
using static System.Windows.Forms.LinkLabel;

namespace MultiTuner.Model.AppModel.SpotifyAppModel
{

    public class SpotifyPlaylist : IPlaylist
    {
        // ===== Identity =====
        public string UniqueKey => Id;

        // ===== Existing data =====
        public string Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }

        // ===== Track state =====
        public ObservableCollection<ITrack> Tracks { get; }
            = new ObservableCollection<ITrack>();
        public bool IsTracksLoaded { get; set; }

        // Hardcoded logic: This object ALWAYS represents a Spotify entity
        public MediaServiceType Platform => MediaServiceType.Spotify;

        // ===== Dependency =====
        private SpotifyAPI _api;

        // Interface implementation - accepts IMusicService
        public void AttachApi(IMusicService api)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));

            if (api is SpotifyAPI spotifyApi)
                _api = spotifyApi;
            else
                throw new ArgumentException("Only SpotifyService can be attached to SpotifyPlaylist", nameof(api));
        }

        // ===== Behavior =====
        public async Task<IEnumerable<ITrack>> LoadTracksAsync()
        {
            if (_api == null)
                throw new InvalidOperationException("Spotify API not attached");
            return await _api.GetPlaylistTracksAsync(UniqueKey);
        }
    }



}
