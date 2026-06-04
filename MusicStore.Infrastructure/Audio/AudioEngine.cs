using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Audio;

public class AudioEngine : AudioPlayer
{
    public AudioEngine(IAudioBackend backend) : base(backend) { }
}