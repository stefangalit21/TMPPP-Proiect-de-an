using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Observer;

public class NowPlayingObserver : IPlayerObserver
{
    public Track?  CurrentTrack  { get; private set; }
    public string  CurrentStatus { get; private set; } = "Stopped";

    public void OnPlay(Track track)
    {
        CurrentTrack  = track;
        CurrentStatus = "Playing";
        Console.Error.WriteLine($"[NowPlaying] Playing: {track.Title}");
    }

    public void OnPause(Track track)
    {
        CurrentStatus = "Paused";
        Console.Error.WriteLine($"[NowPlaying] Paused: {track.Title}");
    }

    public void OnStop()
    {
        CurrentTrack  = null;
        CurrentStatus = "Stopped";
        Console.Error.WriteLine("[NowPlaying] Stopped");
    }
}