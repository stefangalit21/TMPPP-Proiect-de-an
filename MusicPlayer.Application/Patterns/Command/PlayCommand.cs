using MusicPlayer.Application.Services;

namespace MusicPlayer.Application.Patterns.Command;

public class PlayCommand : IPlayerCommand
{
    private readonly IPlaybackService _playback;
    private readonly string           _trackId;
    private readonly string           _previousTrackId;
    private readonly bool             _wasPlaying;

    public string Name => $"Play({_trackId})";

    public PlayCommand(IPlaybackService playback, string trackId)
    {
        _playback        = playback;
        _trackId         = trackId;
        // salvam starea anterioara pentru Undo
        var state        = playback.GetState();
        _previousTrackId = state.CurrentId;
        _wasPlaying      = state.IsPlaying;
    }

    public void Execute()
    {
        _playback.Play(_trackId);
        Console.Error.WriteLine($"[PlayCommand] Execute: {_trackId}");
    }

    public void Undo()
    {
        if (_wasPlaying && !string.IsNullOrEmpty(_previousTrackId))
            _playback.Play(_previousTrackId);
        else
            _playback.Stop();
        Console.Error.WriteLine($"[PlayCommand] Undo → {_previousTrackId}");
    }
}