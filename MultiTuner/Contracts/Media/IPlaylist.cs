using System.Collections.ObjectModel;
using MultiTuner.Contracts.Media;
using MultiTuner.Contracts.Media.API;

public interface IPlaylist
{
    string Id { get; set; }
    string Title { get; set; }
    string ThumbnailUrl { get; set; }
    string UniqueKey { get; }

    bool IsTracksLoaded { get; set; }

    ObservableCollection<ITrack> Tracks { get; }

    MediaServiceType Platform { get; }

    void AttachApi(IMusicService api);

    Task<IEnumerable<ITrack>> LoadTracksAsync();

   

}
