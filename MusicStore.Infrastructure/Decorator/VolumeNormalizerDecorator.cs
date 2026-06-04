using MusicPlayer.Domain;
using MusicStore.Infrastructure.Decorator;

public class VolumeNormalizerDecorator : AudioEngineDecorator
{
    private const float MaxVolume = 0.95f;
    private const float MinVolume = 0.0f;  

    public VolumeNormalizerDecorator(IAudioEngine inner) : base(inner) { }

    public override void SetVolume(float volume)
    {
        float normalized = Math.Clamp(volume, MinVolume, MaxVolume);
        Console.Error.WriteLine($"[VolumeNormalizer] {volume:F2} → {normalized:F2}");
        _inner.SetVolume(normalized);
    }
}