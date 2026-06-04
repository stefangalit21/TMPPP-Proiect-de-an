using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Observer;

public class PlayCountObserver : IPlayerObserver
{
    private readonly Dictionary<string, int> _counts = new();

    public void OnPlay(Track track)
    {
        _counts[track.Id] = _counts.GetValueOrDefault(track.Id) + 1;
        Console.Error.WriteLine($"[PlayCount] {track.Title} redat de {_counts[track.Id]}x");
    }

    public void OnPause(Track track) { }
    public void OnStop()             { }

    public int GetCount(string trackId) =>
        _counts.GetValueOrDefault(trackId);

    public Dictionary<string, int> GetAll() => new(_counts);
}