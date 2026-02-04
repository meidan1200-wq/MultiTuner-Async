using System.Diagnostics;
using System.Reflection;
using System.Windows;
using MultiTuner.Model.VersionControlModel;
using MultiTuner.ViewModel;

namespace MultiTuner
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Initialize Window and ViewModel
            var mainWindow = new MainWindow();
            var vm = (MainWindowViewModel)mainWindow.DataContext;

            mainWindow.Show();

            // 2. Use the Service to get the full Update Info (Version + URL)
            var updateService = new GitHubUpdateService();
            var updateInfo = await updateService.CheckForUpdatesAsync();


            //if (updateInfo == null)
            //{
            //    // If your GitHub API isn't set up yet, create "Fake" data 
            //    // just to see if the UI and Progress Bar work.
            //    updateInfo = new AppVersionInfo
            //    {
            //        Version = new Version(2, 0, 0),
            //        DownloadUrl = "https://github.com/OWNER/REPO/releases/download/v2.0.0/YourApp.exe",
            //        ReleaseNotes = "Testing the UI layout and download stream."
            //    };
            //}


            

            // 3. Compare and trigger the popup
            if (updateInfo != null && updateInfo.Version > CurrentVersion)
            {
                // Pass the whole updateInfo object, not just the version
                vm.ShowUpdatePopup(updateInfo);
            }
        }

        // private static Version CurrentVersion => new Version(0, 0, 1);
        private static Version CurrentVersion =>
        Version.TryParse(
        FileVersionInfo
            .GetVersionInfo(Environment.ProcessPath!)
            .FileVersion, out var v) ? v : new Version(0, 0, 1);






    }
}
