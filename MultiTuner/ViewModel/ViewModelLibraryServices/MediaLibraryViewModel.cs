using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiTuner.Contracts.Media;



namespace MultiTuner.ViewModel.ViewModelLibraryServices
{
    public abstract class MediaLibraryViewModel : ViewModelBase
    {
        // UI-facing collections
        public ObservableCollection<IPlaylist> AllPlaylists { get; } = new();
        public ObservableCollection<ITrack> AllTracks { get; } = new();

       
        private IPlaylist _selectedPlaylist;
        public IPlaylist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                if (!ReferenceEquals(_selectedPlaylist, value))
                {
                    // Save UI changes back to the currently selected playlist
                    if (_selectedPlaylist != null)
                    {
                        _selectedPlaylist.Tracks.Clear();
                        foreach (var t in AllTracks)
                            _selectedPlaylist.Tracks.Add(t);
                    }

                    _selectedPlaylist = value;
                    OnPropertyChanged();

                    _ = LoadTracksAsync();
                }
            }
        }


        // ── the single shared command ────────────────────────────────
        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            protected set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public Task SaveAsync()
            => SaveCurrentPlaylistAsync();

        protected abstract Task SaveCurrentPlaylistAsync();




        /* =========================
           Playlist loading
           ========================= */

        protected abstract Task<IEnumerable<IPlaylist>> FetchPlaylistsAsync();

        protected async Task LoadPlaylistsAsync()
        {
            var playlists = await FetchPlaylistsAsync();
            if (playlists == null) return;

            AllPlaylists.Clear();
            foreach (var p in playlists)
                AllPlaylists.Add(p);
        }

        /* =========================
           Track loading
           ========================= */

        private async Task LoadTracksAsync()
        {
            AllTracks.Clear();

            if (_selectedPlaylist == null)
                return;

            // Load tracks only once per playlist
            if (!_selectedPlaylist.IsTracksLoaded)
            {
                var tracks = await _selectedPlaylist.LoadTracksAsync();
                if (tracks == null) return;

                foreach (var t in tracks)
                    _selectedPlaylist.Tracks.Add(t);

                _selectedPlaylist.IsTracksLoaded = true;
            }

            // Bind UI to playlist-owned state
            foreach (var t in _selectedPlaylist.Tracks)
                AllTracks.Add(t);
        }


        protected TrackDescriptor ToTrackDescriptor(ITrack track)
        {
            return new TrackDescriptor
            {
                Title = track.Title,
                Artist = track.Artist,
                Album = track.Album,
                Duration = TimeSpan.TryParse(track.Duration, out var d) ? d : null
            };
        }







    }


}
