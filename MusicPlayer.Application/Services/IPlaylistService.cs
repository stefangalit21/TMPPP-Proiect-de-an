using MusicPlayer.Application.Patterns.Iterator;
using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Application.Patterns.Strategy;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Services;

public interface IPlaylistService
{
    void         AddTrack(ITrack track);
    void         RemoveTrack(string trackId);
    List<ITrack> GetAll(ITrackSortStrategy? strategy = null);
    IPlaylistIterator GetIterator(bool shuffle = false);
}