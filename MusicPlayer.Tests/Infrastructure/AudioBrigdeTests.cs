using MusicStore.Infrastructure.Audio;
using Xunit;

namespace MusicPlayer.Tests.Infrastructure;

public class AudioBridgeTests
{
    [Fact]
    public void AudioEngine_WithDummy_IsLoadedFalseInitially()
    {
        var engine = new AudioEngine(new DummyBackend());
        Assert.False(engine.IsLoaded);
    }

    [Fact]
    public void AudioEngine_AfterLoad_IsLoadedTrue()
    {
        var engine = new AudioEngine(new DummyBackend());
        engine.Load("/demo/test.mp3");
        Assert.True(engine.IsLoaded);
    }

    [Fact]
    public void AudioEngine_Play_ShouldNotThrow()
    {
        var engine = new AudioEngine(new DummyBackend());
        engine.Load("/demo/test.mp3");
        var ex = Record.Exception(() => engine.Play());
        Assert.Null(ex);
    }

    [Fact]
    public void AudioEngine_Pause_ShouldNotThrow()
    {
        var engine = new AudioEngine(new DummyBackend());
        engine.Load("/demo/test.mp3");
        engine.Play();
        var ex = Record.Exception(() => engine.Pause());
        Assert.Null(ex);
    }

    [Fact]
    public void AudioEngine_Stop_ShouldNotThrow()
    {
        var engine = new AudioEngine(new DummyBackend());
        engine.Load("/demo/test.mp3");
        engine.Play();
        var ex = Record.Exception(() => engine.Stop());
        Assert.Null(ex);
    }

    [Fact]
    public void AudioEngine_CanSwapBackend()
    {
        var engine1 = new AudioEngine(new DummyBackend());
        var engine2 = new AudioEngine(new NAudioBackend());
        Assert.IsAssignableFrom<MusicPlayer.Domain.IAudioEngine>(engine1);
        Assert.IsAssignableFrom<MusicPlayer.Domain.IAudioEngine>(engine2);
    }
}