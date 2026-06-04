using MusicPlayer.Application.Patterns.Builder;
using MusicPlayer.Domain;
using Xunit;

namespace MusicPlayer.Tests.Application.Patterns;

public class BuilderTests
{
    [Fact]
    public void Builder_ShouldSetName()
    {
        var playlist = new PlaylistBuilder()
            .WithName("My Playlist")
            .Build();
        Assert.Equal("My Playlist", playlist.Name);
    }

    [Fact]
    public void Builder_ShouldSetMode()
    {
        var playlist = new PlaylistBuilder()
            .WithName("Test")
            .WithMode(PlayMode.Shuffle)
            .Build();
        Assert.Equal(PlayMode.Shuffle, playlist.Mode);
    }

    [Fact]
    public void Builder_ShouldAddTrack()
    {
        var track    = new Track { Title = "Song" };
        var playlist = new PlaylistBuilder()
            .WithName("Test")
            .AddTrack(track)
            .Build();
        Assert.Single(playlist.Tracks);
    }

    [Fact]
    public void Builder_ShouldSetColor()
    {
        var playlist = new PlaylistBuilder()
            .WithName("Test")
            .WithColor("#FF0000")
            .Build();
        Assert.Equal("#FF0000", playlist.Color);
    }

    [Fact]
    public void Builder_WithoutName_ShouldThrow()
    {
        Assert.Throws<Exception>(() =>
            new PlaylistBuilder().Build());
    }
}