using System;
using System.Collections.Generic;
using System.Text;
using MultiTuner.Contracts.Media;

namespace MultiTuner.Model.AppModel.SpotifyAppModel
{
    public class SpotifyTrack : ITrack
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }          // Album name
        public string ThumbnailUrl { get; set; }
        public string Duration { get; set; }       // Duration as "mm:ss"
        public string Id { get; set; }
        public string Link { get; set; }           // Spotify URL


        public MediaServiceType Platform => MediaServiceType.Spotify;
    }

}
