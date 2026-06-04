using MusicPlayer.Application.Patterns.Factory;
using Xunit;

namespace MusicPlayer.Tests.Application.Patterns;

public class FactoryTests
{
    [Fact]
    public void FileTrackFactory_ShouldSetTitleFromFileName()
    {
        var factory = new FileTrackFactory();
        var track   = factory.Create("/music/Queen - Bohemian Rhapsody.mp3");
        Assert.Equal("Queen - Bohemian Rhapsody", track.Title);
    }

    [Fact]
    public void FileTrackFactory_ShouldSetFilePath()
    {
        var factory = new FileTrackFactory();
        var track   = factory.Create("/music/song.mp3");
        Assert.Equal("/music/song.mp3", track.FilePath);
    }

    [Fact]
    public void DemoTrackFactory_ShouldSetTitle()
    {
        var factory = new DemoTrackFactory();
        var track   = factory.Create("Test Song");
        Assert.Equal("Test Song", track.Title);
    }

    [Fact]
    public void DemoTrackFactory_ShouldSetDemoArtist()
    {
        var factory = new DemoTrackFactory();
        var track   = factory.Create("Test Song");
        Assert.Equal("Demo Artist", track.Artist);
    }

    [Fact]
    public void DemoTrackFactory_ShouldHaveDuration()
    {
        var factory = new DemoTrackFactory();
        var track   = factory.Create("Test Song");
        Assert.True(track.Duration > 0);
    }
}