using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Strategy;

public interface ITrackSortStrategy
{
    List<ITrack> Sort(List<ITrack> tracks);
}