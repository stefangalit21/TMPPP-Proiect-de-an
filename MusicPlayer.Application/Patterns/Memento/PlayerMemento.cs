using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Memento;

public class PlayerMemento
{
    public string   CurrentTrackId { get; }
    public int      Position       { get; }
    public float    Volume         { get; }
    public PlayMode Mode           { get; }
    public bool     IsPlaying      { get; }
    public DateTime SavedAt        { get; }

    public PlayerMemento(
        string   currentTrackId,
        int      position,
        float    volume,
        PlayMode mode,
        bool     isPlaying)
    {
        CurrentTrackId = currentTrackId;
        Position       = position;
        Volume         = volume;
        Mode           = mode;
        IsPlaying      = isPlaying;
        SavedAt        = DateTime.Now;
    }

    public override string ToString() =>
        $"[{SavedAt:HH:mm:ss}] Track={CurrentTrackId} " +
        $"Pos={Position}s Vol={Volume:F2} Mode={Mode}";
}