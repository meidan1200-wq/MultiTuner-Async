using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using MultiTuner.Model.VersionControlModel;
using MultiTuner.View.PopupViewControl;
using MultiTuner.ViewModel.UpdateServiceViewModel;
using MultiTuner.ViewModel.ViewModelLibraryServices;

namespace MultiTuner.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private MediaLibraryViewModel _activeService;
        private bool _isSaving;

        private readonly GitHubUpdateService _updateService;

        // Popup related fields
        private object? _updatePopup;
        private bool _isUpdatePopupVisible;

        
        public string AppVersion { get; }

        // Popup visibility property
        public object? UpdatePopup
        {
            get => _updatePopup;
            private set
            {
                _updatePopup = value;
                OnPropertyChanged();
            }
        }

        // popup visibility property
        public bool IsUpdatePopupVisible
        {
            get => _isUpdatePopupVisible;
            private set
            {
                _isUpdatePopupVisible = value;
                OnPropertyChanged();
            }
        }


        public SpotifyLibraryViewModel Spotify { get; }
        public YouTubeLibraryViewModel YouTube { get; }

        public MediaLibraryViewModel ActiveService
        {
            get => _activeService;
            set
            {
                if (_activeService == value)
                    return;

                if (_activeService != null)
                    _activeService.PropertyChanged -= ActiveService_PropertyChanged;

                _activeService = value;

                if (_activeService != null)
                    _activeService.PropertyChanged += ActiveService_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusMessage));
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public string StatusMessage => ActiveService?.StatusMessage;

        public AsyncRelayCommand<object> SaveCommand { get; }

        public MainWindowViewModel()
        {
            _updateService = new GitHubUpdateService();


            Spotify = new SpotifyLibraryViewModel();
            YouTube = new YouTubeLibraryViewModel();

            SaveCommand = new AsyncRelayCommand<object>(ExecuteSaveAsync, CanSave);

            // set the current Version
            var version = FileVersionInfo.GetVersionInfo(Environment.ProcessPath!).FileVersion;
            AppVersion = !string.IsNullOrEmpty(version) ? $"v{version}" : "Unknown";

        }


        public void ShowUpdatePopup(AppVersionInfo info)
        {
            // We pass the service we created earlier or create a new one
            var viewModel = new UpdatePopupViewModel(new GitHubUpdateService(), info, CloseUpdatePopup);

            UpdatePopup = new PopupUpdateControl
            {
                DataContext = viewModel
            };

            IsUpdatePopupVisible = true;
        }


        private void CloseUpdatePopup()
        {
            IsUpdatePopupVisible = false;
            UpdatePopup = null;
        }



        private bool CanSave(object _)
         => !_isSaving;

        private async Task ExecuteSaveAsync(object parameter)
        {
            ActiveService = parameter switch
            {
                "Spotify" => Spotify,
                "YouTube" => YouTube,
                _ => ActiveService
            };

            _isSaving = true;
            SaveCommand.NotifyCanExecuteChanged();

            try
            {
                await ActiveService.SaveAsync();
            }
            finally
            {
                _isSaving = false;
                SaveCommand.NotifyCanExecuteChanged();
            }
        }

        private void ActiveService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MediaLibraryViewModel.StatusMessage))
                OnPropertyChanged(nameof(StatusMessage));
        }
    }

}
