using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Factory;

public class DemoTrackFactory : TrackFactory
{
    private static int _counter = 1;
 
    public override ITrack Create(string title) => new Track
    {
        Title    = title,
        Artist   = "Demo Artist",
        Album    = "Demo Album",
        Genre    = "Demo",
        Duration = 180 + _counter * 17,
        FilePath = $"/demo/track{_counter++}.mp3"
    };
}