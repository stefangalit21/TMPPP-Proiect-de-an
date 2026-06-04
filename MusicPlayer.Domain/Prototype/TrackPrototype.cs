using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Prototype;

public class TrackPrototype
{
    private readonly Track _original;

    public TrackPrototype(Track original)
    {
        _original = original;
    }

    public Track Clone() => new Track
    {
        Id       = Guid.NewGuid().ToString(),
        Title    = _original.Title,
        Artist   = _original.Artist,
        Album    = _original.Album,
        Genre    = _original.Genre,
        FilePath = _original.FilePath,
        Duration = _original.Duration,
        Year     = _original.Year,
        FileSize = _original.FileSize
    };
}