using MusicPlayer.Domain;

namespace MusicPlayer.Application.Services;

public class PlayerState
{
    public bool     IsPlaying { get; set; }
    public string   CurrentId { get; set; } = "";
    public int      Position  { get; set; }
    public float    Volume    { get; set; } = 0.8f;
    public PlayMode Mode      { get; set; }
}