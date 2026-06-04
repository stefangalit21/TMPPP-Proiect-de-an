using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Observer;

public class LoggerObserver : IPlayerObserver
{
    private readonly List<string> _log = new();

    public void OnPlay(Track track)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] PLAY  — {track.Artist} - {track.Title}";
        _log.Add(entry);
        Console.Error.WriteLine($"[Logger] {entry}");
    }

    public void OnPause(Track track)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] PAUSE — {track.Artist} - {track.Title}";
        _log.Add(entry);
        Console.Error.WriteLine($"[Logger] {entry}");
    }

    public void OnStop()
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] STOP";
        _log.Add(entry);
        Console.Error.WriteLine($"[Logger] {entry}");
    }

    public List<string> GetLog() => new(_log);
}