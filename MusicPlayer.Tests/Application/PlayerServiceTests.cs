using MusicPlayer.Application.Services;
using MusicPlayer.Domain;
using MusicStore.Infrastructure.Audio;
using Xunit;

namespace MusicPlayer.Tests.Application;

public class PlayerServiceTests
{
    private static PlayerService MakeService()
    {
        var engine = new AudioEngine(new DummyBackend());
        return new PlayerService(engine);
    }

    private static Track MakeTrack(string id, string title = "Song") => new Track
    {
        Id       = id,
        Title    = title,
        FilePath = $"/demo/{id}.mp3"
    };

    [Fact]
    public void AddTrack_ShouldAddToPlaylist()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        Assert.Single(svc.GetAll());
    }

    [Fact]
    public void AddTrack_SamePath_ShouldNotAddDuplicate()
    {
        var svc = MakeService();
        var t   = MakeTrack("1");
        svc.AddTrack(t);
        svc.AddTrack(t);
        Assert.Single(svc.GetAll());
    }

    [Fact]
    public void RemoveTrack_ShouldRemoveFromPlaylist()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.RemoveTrack("1");
        Assert.Empty(svc.GetAll());
    }

    [Fact]
    public void Play_ShouldSetIsPlaying()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.Play("1");
        Assert.True(svc.GetState().IsPlaying);
    }

    [Fact]
    public void Play_ShouldSetCurrentId()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.Play("1");
        Assert.Equal("1", svc.GetState().CurrentId);
    }

    [Fact]
    public void Play_ShouldIncrementPlayCount()
    {
        var svc   = MakeService();
        var track = MakeTrack("1");
        svc.AddTrack(track);
        svc.Play("1");
        Assert.Equal(1, track.PlayCount);
    }

    [Fact]
    public void Pause_ShouldSetIsPlayingFalse()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.Play("1");
        svc.Pause();
        Assert.False(svc.GetState().IsPlaying);
    }

    [Fact]
    public void Stop_ShouldResetPosition()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.Play("1");
        svc.Stop();
        Assert.Equal(0, svc.GetState().Position);
    }

    [Fact]
    public void Next_ShouldPlayNextTrack()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.AddTrack(MakeTrack("2"));
        svc.Play("1");
        svc.Next();
        Assert.Equal("2", svc.GetState().CurrentId);
    }

    [Fact]
    public void Previous_ShouldPlayPreviousTrack()
    {
        var svc = MakeService();
        svc.AddTrack(MakeTrack("1"));
        svc.AddTrack(MakeTrack("2"));
        svc.Play("2");
        svc.Previous();
        Assert.Equal("1", svc.GetState().CurrentId);
    }

    [Fact]
    public void SetVolume_ShouldClampBetweenZeroAndOne()
    {
        var svc = MakeService();
        svc.SetVolume(1.5f);
        Assert.Equal(1.0f, svc.GetState().Volume);
        svc.SetVolume(-0.5f);
        Assert.Equal(0.0f, svc.GetState().Volume);
    }

    [Fact]
    public void SetMode_ShouldUpdateMode()
    {
        var svc = MakeService();
        svc.SetMode(PlayMode.Shuffle);
        Assert.Equal(PlayMode.Shuffle, svc.GetState().Mode);
    }
}