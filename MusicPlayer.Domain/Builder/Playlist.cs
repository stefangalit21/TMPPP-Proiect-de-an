using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Builder;

public class Playlist
{
    public string      Name        { get; set; } = "";
    public string      Description { get; set; } = "";
    public PlayMode    Mode        { get; set; }
    public string      Color       { get; set; } = "#7C4DFF";
    public List<Track> Tracks      { get; set; } = new();
}