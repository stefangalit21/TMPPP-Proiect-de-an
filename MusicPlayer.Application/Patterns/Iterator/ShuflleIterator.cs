using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Iterator;

// Iterator care parcurge playlist-ul in ordine aleatoare
public class ShuffleIterator : IPlaylistIterator
{
    private readonly List<Track> _shuffled;
    private int _index = 0;

    public ShuffleIterator(List<Track> tracks)
    {
        _shuffled = tracks.OrderBy(_ => Guid.NewGuid()).ToList();
        Console.Error.WriteLine("[ShuffleIterator] Playlist amestecat");
    }

    public bool  HasNext()     => _index < _shuffled.Count - 1;
    public bool  HasPrevious() => _index > 0;

    public Track Next()
    {
        if (!HasNext())
            throw new InvalidOperationException("Nu exista track urmator");
        _index++;
        Console.Error.WriteLine($"[ShuffleIterator] Next → {_shuffled[_index].Title}");
        return _shuffled[_index];
    }

    public Track Previous()
    {
        if (!HasPrevious())
            throw new InvalidOperationException("Nu exista track anterior");
        _index--;
        return _shuffled[_index];
    }

    public Track Current() => _shuffled[_index];

    public void Reset()
    {
        _index = 0;
        Console.Error.WriteLine("[ShuffleIterator] Reset");
    }

    public int CurrentIndex => _index;
    public int Count        => _shuffled.Count;
}