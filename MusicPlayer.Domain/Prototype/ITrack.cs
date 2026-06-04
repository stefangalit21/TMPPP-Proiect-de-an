namespace MusicPlayer.Application.Patterns.Prototype;
public interface ITrack : ICloneable
{
    string Id         { get; set; }
    string Title      { get; set; }
    string Artist     { get; set; }
    string Album      { get; set; }
    string Genre      { get; set; }
    string FilePath   { get; set; }
    int    Duration   { get; set; }
    int    Year       { get; set; }
    long   FileSize   { get; set; }
    int    PlayCount  { get; set; }
    bool   IsFavorite { get; set; }
 
    string DurationFormatted { get; }
 
    /// <summary>Clonare tipizată — returnează ITrack, nu object.</summary>
    new ITrack Clone();
}