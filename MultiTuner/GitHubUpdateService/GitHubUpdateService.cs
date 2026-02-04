using System;
using System.Collections.Generic;
using System.Text;
using MultiTuner.Model.VersionControlModel;

namespace MultiTuner
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class GitHubUpdateService
    {
        private const string RepoOwner = "meidan1200-wq";
        private const string RepoName = "Spotify-playlist-downloader";


        private readonly string _apiEndpoint = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        public async Task<AppVersionInfo?> CheckForUpdatesAsync()
        {
            using var client = new HttpClient();

            // GitHub still requires a User-Agent even for public repos
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WPF-App-Updater");

            try
            {
                var response = await client.GetAsync(_apiEndpoint);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // 1. Get Version
                var tagName = root.GetProperty("tag_name").GetString();
                if (string.IsNullOrEmpty(tagName)) return null;
                var cleanVersion = tagName.TrimStart('v', 'V');

                // 2. Get the Asset URL
                var assets = root.GetProperty("assets").EnumerateArray();
                var downloadUrl = assets
                    .FirstOrDefault(asset => asset.GetProperty("name").GetString()?.EndsWith(".exe") ?? false)
                    // CHANGED: Use 'browser_download_url' for public direct downloads instead of 'url' (API link)
                    .GetProperty("browser_download_url")
                    .GetString();

                if (string.IsNullOrEmpty(downloadUrl)) return null;

                return new AppVersionInfo
                {
                    Version = Version.Parse(cleanVersion),
                    DownloadUrl = downloadUrl,
                    ReleaseNotes = root.GetProperty("body").GetString()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> DownloadUpdateAsync(string downloadUrl, IProgress<double> progress)
        {
            string fileName = "Setup.exe";
            string tempPath = Path.Combine(Path.GetTempPath(), fileName);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WPF-App-Updater");

            // REMOVED: Authorization header
            // REMOVED: Accept header (octet-stream). 
            // Since we are using 'browser_download_url', we don't need to tell the API to stream raw data.

            try
            {
                using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalRead += bytesRead;

                            if (totalBytes > 0)
                                progress?.Report((double)totalRead / totalBytes * 100);
                        }
                    }
                }
                return tempPath;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Download failed: {ex.Message}");
            }
        }

        public void InstallUpdate(string installerPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = installerPath,
                Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART",
                UseShellExecute = true,
                Verb = "runas" // Asks for Admin permission if required to write to Program Files
            };

            Process.Start(startInfo);

            // CRITICAL: We must exit now so Inno Setup can overwrite our EXE
            System.Windows.Application.Current.Shutdown();
        }
    }

}
