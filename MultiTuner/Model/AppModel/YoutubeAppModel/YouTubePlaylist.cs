using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MultiTuner.Contracts.Media;
using MultiTuner.Contracts.Media.API;
using MultiTuner.Services;  
using MultiTuner.Services.Youtube;


namespace MultiTuner.Model.AppModel.YoutubeAppModel
{
    public class YouTubePlaylist : IPlaylist
    {
        public string UniqueKey => Id;
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        

        public ObservableCollection<ITrack> Tracks { get; }
            = new ObservableCollection<ITrack>();
        public bool IsTracksLoaded { get; set; }
        public MediaServiceType Platform => MediaServiceType.YouTube;

        private YouTubeAPI _api; 

        // Interface implementation - accepts IMusicService
        public void AttachApi(IMusicService api)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));

            if (api is YouTubeAPI youtubeApi)
                _api = youtubeApi;
            else
                throw new ArgumentException("Only YouTubeService can be attached to YouTubePlaylist", nameof(api));
        }

        public async Task<IEnumerable<ITrack>> LoadTracksAsync()
        {
            if (_api == null)
                throw new InvalidOperationException("YouTube API not attached");
            return await _api.GetPlaylistTracksAsync(UniqueKey);
        }
    }


}
