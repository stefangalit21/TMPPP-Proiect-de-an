using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Factory;

public class FileTrackFactory : TrackFactory
{
    public override ITrack Create(string filePath)
    {
        var info  = new FileInfo(filePath);
        var name  = Path.GetFileNameWithoutExtension(filePath);
 
        string artist = "Unknown Artist";
        string title  = name;
        
        if (name.Contains(" - "))
        {
            var parts = name.Split(" - ", 2);
            artist = parts[0].Trim();
            title  = parts[1].Trim();
        }
 
        Console.Error.WriteLine($"[FileTrackFactory] Create: {artist} - {title}");
 
        return new Track
        {
            Title    = title,
            Artist   = artist,
            FilePath = filePath,
            FileSize = File.Exists(filePath) ? info.Length : 0,
            Duration = File.Exists(filePath) ? (int)(info.Length / 40_000) : 0
        };
    }
}