using MusicPlayer.Domain;
using Xunit;

namespace MusicPlayer.Tests.Domain;

public class TrackTests
{
    [Fact]
    public void Track_DefaultId_ShouldNotBeEmpty()
    {
        var track = new Track();
        Assert.NotEmpty(track.Id);
    }

    [Fact]
    public void Track_DurationFormatted_ShouldFormatCorrectly()
    {
        var track = new Track { Duration = 354 };
        Assert.Equal("5:54", track.DurationFormatted);
    }

    [Fact]
    public void Track_DurationFormatted_ShouldPadSeconds()
    {
        var track = new Track { Duration = 65 };
        Assert.Equal("1:05", track.DurationFormatted);
    }

    [Fact]
    public void Track_ToString_ShouldReturnArtistAndTitle()
    {
        var track = new Track { Artist = "Queen", Title = "Bohemian Rhapsody" };
        Assert.Equal("Queen - Bohemian Rhapsody", track.ToString());
    }

    [Fact]
    public void Track_PlayCount_DefaultShouldBeZero()
    {
        var track = new Track();
        Assert.Equal(0, track.PlayCount);
    }

    [Fact]
    public void Track_IsFavorite_DefaultShouldBeFalse()
    {
        var track = new Track();
        Assert.False(track.IsFavorite);
    }
}