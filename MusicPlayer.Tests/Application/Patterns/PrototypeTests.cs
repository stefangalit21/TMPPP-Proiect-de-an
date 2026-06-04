using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;
using Xunit;

namespace MusicPlayer.Tests.Application.Patterns;

public class PrototypeTests
{
    [Fact]
    public void Clone_ShouldHaveDifferentId()
    {
        var original  = new Track { Title = "Song", Artist = "Artist" };
        var prototype = new TrackPrototype(original);
        var clone     = prototype.Clone();
        Assert.NotEqual(original.Id, clone.Id);
    }

    [Fact]
    public void Clone_ShouldCopyTitle()
    {
        var original  = new Track { Title = "Bohemian Rhapsody" };
        var clone     = new TrackPrototype(original).Clone();
        Assert.Equal("Bohemian Rhapsody", clone.Title);
    }

    [Fact]
    public void Clone_ShouldCopyArtist()
    {
        var original = new Track { Artist = "Queen" };
        var clone    = new TrackPrototype(original).Clone();
        Assert.Equal("Queen", clone.Artist);
    }

    [Fact]
    public void Clone_ShouldNotCopyPlayCount()
    {
        var original = new Track { PlayCount = 10 };
        var clone    = new TrackPrototype(original).Clone();
        Assert.Equal(0, clone.PlayCount);
    }

    [Fact]
    public void Clone_ShouldNotCopyIsFavorite()
    {
        var original = new Track { IsFavorite = true };
        var clone    = new TrackPrototype(original).Clone();
        Assert.False(clone.IsFavorite);
    }
}