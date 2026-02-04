using System.Web;
using MultiTuner.Contracts.Media;

public interface ITrack
{
    string Title { get; }          // Always available
    string Artist { get; }         // Always available
    string Album { get; }          // Can be null if not applicable
    string Duration { get; }       // Can be null if unknown

    string Id { get; }             // Unique identifier for the track

    MediaServiceType Platform { get; }
}
