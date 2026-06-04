using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Builder;

public class PlaylistBuilder
{
    private readonly Playlist _playlist = new();

    public PlaylistBuilder WithName(string name)
    { _playlist.Name = name; return this; }

    public PlaylistBuilder WithDescription(string desc)
    { _playlist.Description = desc; return this; }

    public PlaylistBuilder WithMode(PlayMode mode)
    { _playlist.Mode = mode; return this; }

    public PlaylistBuilder WithColor(string color)
    { _playlist.Color = color; return this; }

    public PlaylistBuilder AddTrack(Track track)
    { _playlist.Tracks.Add(track); return this; }

    public Playlist Build()
    {
        if (string.IsNullOrWhiteSpace(_playlist.Name))
            throw new Exception("Playlist-ul trebuie sa aiba un nume!");
        return _playlist;
    }
}