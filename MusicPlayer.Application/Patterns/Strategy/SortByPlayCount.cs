using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Strategy;

public class SortByPlayCount : ITrackSortStrategy
{
    public List<ITrack> Sort(List<ITrack> tracks) =>
        tracks.OrderByDescending(t => t.PlayCount).ToList();
}