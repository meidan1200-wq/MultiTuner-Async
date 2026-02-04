using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media
{
    internal interface IPlaylistSource
    {
        Task<IEnumerable<IPlaylist>> GetPlaylistsAsync();
    }
}
