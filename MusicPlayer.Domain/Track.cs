using MusicPlayer.Application.Patterns.Prototype;

namespace MusicPlayer.Domain;

public class Track : ITrack
{
    public string Id         { get; set; } = Guid.NewGuid().ToString();
    public string Title      { get; set; } = "Unknown Title";
    public string Artist     { get; set; } = "Unknown Artist";
    public string Album      { get; set; } = "Unknown Album";
    public string Genre      { get; set; } = "Unknown";
    public string FilePath   { get; set; } = "";
    public int    Duration   { get; set; }
    public int    Year       { get; set; }
    public long   FileSize   { get; set; }
    public int    PlayCount  { get; set; }
    public bool   IsFavorite { get; set; }
 
    public string DurationFormatted =>
        $"{Duration / 60}:{Duration % 60:D2}";
    
    public ITrack Clone()
    {
        var copy        = (Track)MemberwiseClone();
        copy.Id         = Guid.NewGuid().ToString();
        copy.PlayCount  = 0;
        copy.IsFavorite = false;
        return copy;
    }
    
    object ICloneable.Clone() => Clone();
 
    public override string ToString() => $"{Artist} - {Title}";
}