using MusicPlayer.Domain;
using MusicStore.Infrastructure.Flyweight;
using Xunit;

namespace MusicPlayer.Tests.Infrastructure;

public class FlyweightTests
{
    [Fact]
    public void Pool_SameGenreArtist_ShouldReturnSameInstance()
    {
        var pool = FlyweightPool.Instance;
        var fw1  = pool.Get("Rock", "Queen");
        var fw2  = pool.Get("Rock", "Queen");
        Assert.Same(fw1, fw2);
    }

    [Fact]
    public void Pool_DifferentArtists_ShouldReturnDifferentInstances()
    {
        var pool = FlyweightPool.Instance;
        var fw1  = pool.Get("Rock", "Queen");
        var fw2  = pool.Get("Rock", "Pink Floyd");
        Assert.NotSame(fw1, fw2);
    }

    [Fact]
    public void Flyweight_ApplyTo_ShouldSetGenre()
    {
        var pool  = FlyweightPool.Instance;
        var fw    = pool.Get("Jazz", "Miles Davis");
        var track = new Track { Title = "Kind of Blue" };
        fw.ApplyTo(track);
        Assert.Equal("Jazz", track.Genre);
    }

    [Fact]
    public void Flyweight_ApplyTo_ShouldSetArtist()
    {
        var pool  = FlyweightPool.Instance;
        var fw    = pool.Get("Jazz", "Miles Davis");
        var track = new Track { Title = "Kind of Blue" };
        fw.ApplyTo(track);
        Assert.Equal("Miles Davis", track.Artist);
    }

    [Fact]
    public void Flyweight_ApplyTo_ShouldIncrementUses()
    {
        var pool  = FlyweightPool.Instance;
        var fw    = pool.Get("Pop_Test", "Artist_Test");
        int before = fw.Uses;
        fw.ApplyTo(new Track());
        Assert.Equal(before + 1, fw.Uses);
    }
}