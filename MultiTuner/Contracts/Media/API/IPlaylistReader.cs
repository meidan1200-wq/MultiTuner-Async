using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media.API
{
    interface IPlaylistReader
    {
        Task<IReadOnlyList<IPlaylist>> GetUserPlaylistsAsync();
        Task<IReadOnlyList<ITrack>> GetTracksAsync(IPlaylist playlist);
    }
}
