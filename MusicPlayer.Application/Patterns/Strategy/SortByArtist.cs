using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Strategy;

public class SortByArtist : ITrackSortStrategy
{
    public List<ITrack> Sort(List<ITrack> tracks) =>
        tracks.OrderBy(t => t.Artist).ToList();
}