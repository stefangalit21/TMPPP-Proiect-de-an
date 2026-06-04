using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Factory;

public abstract class TrackFactory
{
    public abstract ITrack Create(string source);
}