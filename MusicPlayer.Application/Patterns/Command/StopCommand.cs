using MusicPlayer.Application.Services;

namespace MusicPlayer.Application.Patterns.Command;

public class StopCommand : IPlayerCommand
{
    private readonly IPlaybackService _playback;
    private readonly string           _previousTrackId;
    private readonly bool             _wasPlaying;

    public string Name => "Stop";

    public StopCommand(IPlaybackService playback)
    {
        _playback        = playback;
        var state        = playback.GetState();
        _previousTrackId = state.CurrentId;
        _wasPlaying      = state.IsPlaying;
    }

    public void Execute()
    {
        _playback.Stop();
        Console.Error.WriteLine("[StopCommand] Execute");
    }

    public void Undo()
    {
        if (_wasPlaying && !string.IsNullOrEmpty(_previousTrackId))
            _playback.Play(_previousTrackId);
        Console.Error.WriteLine($"[StopCommand] Undo → {_previousTrackId}");
    }
}