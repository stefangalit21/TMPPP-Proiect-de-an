using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Metadata;

public class CachingMetadataProxy : IMetadataReader
{
    private readonly IMetadataReader           _real;
    private readonly Dictionary<string, Track> _cache = new();

    public CachingMetadataProxy(IMetadataReader real)
    {
        _real = real;
    }

    public Track? Read(string filePath)
    {
        if (_cache.TryGetValue(filePath, out var cached))
        {
            Console.Error.WriteLine($"[Proxy] Cache HIT: {Path.GetFileName(filePath)}");
            return cached;
        }

        Console.Error.WriteLine($"[Proxy] Cache MISS: {Path.GetFileName(filePath)}");
        var track = _real.Read(filePath);

        if (track != null)
            _cache[filePath] = track;

        return track;
    }

    public int CacheSize => _cache.Count;
}