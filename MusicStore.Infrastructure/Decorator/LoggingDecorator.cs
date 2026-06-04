using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Decorator;

public class LoggingDecorator : AudioEngineDecorator
{
    public LoggingDecorator(IAudioEngine inner) : base(inner) { }

    public override void Load(string filePath)
    {
        Console.Error.WriteLine($"[Log] Load: {Path.GetFileName(filePath)}");
        _inner.Load(filePath);
    }

    public override void Play()
    {
        Console.Error.WriteLine("[Log] Play");
        _inner.Play();
    }

    public override void Pause()
    {
        Console.Error.WriteLine("[Log] Pause");
        _inner.Pause();
    }

    public override void Stop()
    {
        Console.Error.WriteLine("[Log] Stop");
        _inner.Stop();
    }

    public override void SetVolume(float volume)
    {
        Console.Error.WriteLine($"[Log] SetVolume: {volume:F2}");
        _inner.SetVolume(volume);
    }
}