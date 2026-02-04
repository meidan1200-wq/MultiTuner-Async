using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media
{
    internal interface IPlaylistSyncService
    {
        /// <summary>
        /// Saves the playlist state in the platform, respecting the given order.
        /// </summary>
        /// <param name="playlistId">The platform-specific playlist identifier.</param>
        /// <param name="orderedTracks">Ordered tracks as edited by the user.</param>
        Task<string> SavePlaylistAsync(
            IPlaylist playlist,
            IReadOnlyList<ITrack> orderedTracks);

        
    }

}

