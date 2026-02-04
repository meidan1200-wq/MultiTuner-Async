using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Model.VersionControlModel
{
    public class AppVersionInfo
    {
        public Version Version { get; set; }
        public string DownloadUrl { get; set; } // The direct link to the .exe
        public string ReleaseNotes { get; set; } // Optional: Display what's new
    }
}
