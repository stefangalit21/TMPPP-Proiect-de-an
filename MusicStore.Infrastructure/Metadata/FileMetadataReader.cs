using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Metadata;

public class FileMetadataReader : IMetadataReader
{
    private static readonly HashSet<string> Supported =
        new(StringComparer.OrdinalIgnoreCase)
            { ".mp3", ".flac", ".wav", ".ogg", ".m4a" };

    public Track? Read(string filePath)
    {
        if (!File.Exists(filePath))                            return null;
        if (!Supported.Contains(Path.GetExtension(filePath))) return null;

        var info = new FileInfo(filePath);
        var name = Path.GetFileNameWithoutExtension(filePath);

        string artist = "Unknown Artist";
        string title  = name;

        if (name.Contains(" - "))
        {
            var parts = name.Split(" - ", 2);
            artist = parts[0].Trim();
            title  = parts[1].Trim();
        }

        Console.Error.WriteLine($"[FileReader] Reading: {name}");

        return new Track
        {
            Title    = title,
            Artist   = artist,
            FilePath = filePath,
            FileSize = info.Length,
            Duration = (int)(info.Length / 40_000)
        };
    }
}