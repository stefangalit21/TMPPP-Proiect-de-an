using MusicPlayer.Application.Services;

namespace MusicPlayer.Application.Patterns.Command;

public class SkipCommand : IPlayerCommand
{
    private readonly IPlaybackService _playback;
    private readonly string           _previousTrackId;
    private readonly bool             _forward;

    public string Name => _forward ? "Next" : "Previous";

    public SkipCommand(IPlaybackService playback, bool forward = true)
    {
        _playback        = playback;
        _forward         = forward;
        _previousTrackId = playback.GetState().CurrentId;
    }

    public void Execute()
    {
        if (_forward) _playback.Next();
        else          _playback.Previous();
        Console.Error.WriteLine($"[SkipCommand] Execute: {Name}");
    }

    public void Undo()
    {
        if (!string.IsNullOrEmpty(_previousTrackId))
            _playback.Play(_previousTrackId);
        Console.Error.WriteLine($"[SkipCommand] Undo → {_previousTrackId}");
    }
}