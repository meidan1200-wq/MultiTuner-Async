using System;
using System.Collections.Generic;
using System.Text;

namespace MultiTuner.Contracts.Media
{
    public sealed class TrackDescriptor
    {
        public string Title { get; init; }
        public string Artist { get; init; }
        public string? Album { get; init; }
        public TimeSpan? Duration { get; init; }

    }


}
