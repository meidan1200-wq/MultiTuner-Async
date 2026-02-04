using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media.API
{
    /// <summary>
    /// Extended interface for services that support downloading
    /// </summary>
    public interface IDownloadableService : IMusicService
    {
        Task DownloadTrackAsync(string trackId, string downloadPath);
    }

}
