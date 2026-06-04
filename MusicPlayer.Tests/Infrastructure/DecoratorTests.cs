using MusicStore.Infrastructure.Audio;
using MusicStore.Infrastructure.Decorator;
using Xunit;

namespace MusicPlayer.Tests.Infrastructure;

public class DecoratorTests
{
    private static AudioEngine MakeEngine() =>
        new AudioEngine(new DummyBackend());

    [Fact]
    public void LoggingDecorator_ShouldImplementIAudioEngine()
    {
        var decorator = new LoggingDecorator(MakeEngine());
        Assert.IsAssignableFrom<MusicPlayer.Domain.IAudioEngine>(decorator);
    }

    [Fact]
    public void VolumeNormalizer_ShouldClampMaxVolume()
    {
        var engine    = MakeEngine();
        var decorator = new VolumeNormalizerDecorator(engine);
        var ex        = Record.Exception(() => decorator.SetVolume(2.0f));
        Assert.Null(ex);
    }

    [Fact]
    public void VolumeNormalizer_ShouldClampMinVolume()
    {
        var engine    = MakeEngine();
        var decorator = new VolumeNormalizerDecorator(engine);
        var ex        = Record.Exception(() => decorator.SetVolume(-1.0f));
        Assert.Null(ex);
    }

    [Fact]
    public void FadeInDecorator_Play_ShouldNotThrow()
    {
        var engine    = MakeEngine();
        engine.Load("/demo/test.mp3");
        var decorator = new FadeInDecorator(engine);
        var ex        = Record.Exception(() => decorator.Play());
        Assert.Null(ex);
    }

    [Fact]
    public void Decorators_CanBeStacked()
    {
        
        MusicPlayer.Domain.IAudioEngine engine = MakeEngine();
        engine = new LoggingDecorator(engine);
        engine = new VolumeNormalizerDecorator(engine);
        engine = new FadeInDecorator(engine);

        engine.Load("/demo/test.mp3");
        var ex = Record.Exception(() => engine.Play());
        Assert.Null(ex);
    }

    [Fact]
    public void Decorator_IsLoaded_ShouldDelegateToInner()
    {
        var inner     = MakeEngine();
        var decorator = new LoggingDecorator(inner);
        Assert.False(decorator.IsLoaded);
        inner.Load("/demo/test.mp3");
        Assert.True(decorator.IsLoaded);
    }
}