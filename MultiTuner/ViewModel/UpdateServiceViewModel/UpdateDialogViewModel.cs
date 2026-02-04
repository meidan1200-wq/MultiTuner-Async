using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MultiTuner.Model.VersionControlModel;
using System.Windows.Input;

namespace MultiTuner.ViewModel.UpdateServiceViewModel
{


    public class UpdatePopupViewModel : INotifyPropertyChanged
    {
        private readonly GitHubUpdateService _updateService;
        private readonly AppVersionInfo _updateInfo;
        private readonly Action _close;

        // UI Properties
        private double _downloadProgress;
        private string _statusText;
        private bool _isDownloading;

        public string VersionText => $"Latest version: {_updateInfo.Version}";
        public string ReleaseNotes => _updateInfo.ReleaseNotes; // Bind this to a TextBlock if you want

        public double DownloadProgress
        {
            get => _downloadProgress;
            set { _downloadProgress = value; OnPropertyChanged(); }
        }

        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public bool IsDownloading
        {
            get => _isDownloading;
            set { _isDownloading = value; OnPropertyChanged(); }
        }

        public ICommand UpdateCommand { get; }
        public ICommand DismissCommand { get; }

        // Constructor gets the Service and the Info object
        public UpdatePopupViewModel(
            GitHubUpdateService service,
            AppVersionInfo info,
            Action closeAction)
        {
            _updateService = service;
            _updateInfo = info;
            _close = closeAction;

            StatusText = "Update available";

            UpdateCommand = new RelayCommand(async () => await StartUpdateProcess());
            DismissCommand = new RelayCommand(() => _close());
        }

        private async Task StartUpdateProcess()
        {
            if (IsDownloading) return;

            try
            {
                IsDownloading = true;
                StatusText = "Downloading...";

                // 1. Create a Progress reporter that updates our UI property
                var progressReporter = new Progress<double>(percent =>
                {
                    DownloadProgress = percent;
                });

                // 2. Ask service to download
                string installerPath = await _updateService.DownloadUpdateAsync(_updateInfo.DownloadUrl, progressReporter);

                StatusText = "Installing...";

                // 3. Ask service to install
                _updateService.InstallUpdate(installerPath);
            }
            catch (Exception ex)
            {
                StatusText = "Error during update.";
                IsDownloading = false;
                // Log error here
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}
