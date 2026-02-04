using System;
using System.Collections.Generic;
using System.Text;
using MultiTuner.Contracts.Media;

namespace MultiTuner.Model.AppModel.YoutubeAppModel 
{
    public class YouTubeTrack : ITrack
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album => null;
        public string ThumbnailUrl { get; set; }
        public string Id { get; set; }                // Video ID (e.g. dQw4w9...)
        public string PlaylistItemId { get; set; }    // Unique ID for the item in the playlist
        public long? Position { get; set; }           // Current index on YouTube
        public string VideoUrl => $"https://www.youtube.com/watch?v={Id}";
        public DateTime? PublishedAt { get; set; }
        public string Duration { get; set; }
        public MediaServiceType Platform => MediaServiceType.YouTube;


    }


}

