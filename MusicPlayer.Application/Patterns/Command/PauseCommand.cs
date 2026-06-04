using MusicPlayer.Application.Services;

namespace MusicPlayer.Application.Patterns.Command;

public class PauseCommand : IPlayerCommand
{
    private readonly IPlaybackService _playback;
    private readonly bool             _wasPlaying;

    public string Name => "Pause";

    public PauseCommand(IPlaybackService playback)
    {
        _playback   = playback;
        _wasPlaying = playback.GetState().IsPlaying;
    }

    public void Execute()
    {
        _playback.Pause();
        Console.Error.WriteLine("[PauseCommand] Execute");
    }

    public void Undo()
    {
        if (_wasPlaying)
        {
            var state = _playback.GetState();
            _playback.Play(state.CurrentId);
        }
        Console.Error.WriteLine("[PauseCommand] Undo → Play");
    }
}