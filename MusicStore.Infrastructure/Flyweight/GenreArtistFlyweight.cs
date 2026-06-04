using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Flyweight;

public class GenreArtistFlyweight
{
    public string Genre  { get; }
    public string Artist { get; }
    public int    Uses   { get; private set; }
 
    public GenreArtistFlyweight(string genre, string artist)
    {
        Genre  = genre;
        Artist = artist;
    }
 
    public void ApplyTo(ITrack track)
    {
        track.Genre  = Genre;
        track.Artist = Artist;
        Uses++;
    }
}