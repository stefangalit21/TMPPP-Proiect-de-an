using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Iterator;

public class PlaylistIterator : IPlaylistIterator
{
    private readonly List<Track> _tracks;
    private int _index = 0;

    public PlaylistIterator(List<Track> tracks)
    {
        _tracks = tracks;
    }

    public bool HasNext()     => _index < _tracks.Count - 1;
    public bool HasPrevious() => _index > 0;

    public Track Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("Nu exista track urmator");
        _index++;
        Console.Error.WriteLine($"[Iterator] Next → {_tracks[_index].Title}");
        return _tracks[_index];
    }

    public Track Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("Nu exista track anterior");
        _index--;
        Console.Error.WriteLine($"[Iterator] Previous → {_tracks[_index].Title}");
        return _tracks[_index];
    }

    public Track Current()
    {
        if (_tracks.Count == 0)
            throw new InvalidOperationException("Playlist gol");
        return _tracks[_index];
    }

    public void Reset()
    {
        _index = 0;
        Console.Error.WriteLine("[Iterator] Reset la primul track");
    }

    public void GoTo(string trackId)
    {
        int idx = _tracks.FindIndex(t => t.Id == trackId);
        if (idx == -1)
            throw new InvalidOperationException($"Track {trackId} negasit");
        _index = idx;
        Console.Error.WriteLine($"[Iterator] GoTo → {_tracks[_index].Title}");
    }

    public int CurrentIndex => _index;
    public int Count        => _tracks.Count;
}