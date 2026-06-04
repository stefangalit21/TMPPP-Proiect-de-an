using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Decorator;

public class FadeInDecorator : AudioEngineDecorator
{
    private readonly int _fadeSteps;
    private readonly int _delayMs;

    public FadeInDecorator(IAudioEngine inner, int fadeSteps = 10, int delayMs = 50)
        : base(inner)
    {
        _fadeSteps = fadeSteps;
        _delayMs   = delayMs;
    }

    public override void Play()
    {
        Console.Error.WriteLine("[FadeIn] Pornire cu fade in...");
        _inner.SetVolume(0);
        _inner.Play();

        Task.Run(async () =>
        {
            for (int i = 1; i <= _fadeSteps; i++)
            {
                float vol = (float)i / _fadeSteps;
                _inner.SetVolume(vol);
                await Task.Delay(_delayMs);
            }
            Console.Error.WriteLine("[FadeIn] Fade in complet.");
        });
    }
}