using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Iterator;

public interface IPlaylistIterator
{
    bool  HasNext();
    bool  HasPrevious();
    Track Next();
    Track Previous();
    Track Current();
    void  Reset();
    int   CurrentIndex { get; }
    int   Count        { get; }
}