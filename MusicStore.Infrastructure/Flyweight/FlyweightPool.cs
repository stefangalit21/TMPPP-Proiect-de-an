namespace MusicStore.Infrastructure.Flyweight;

public class FlyweightPool
{
    private static FlyweightPool? _instance;
    public static FlyweightPool Instance =>
        _instance ??= new FlyweightPool();

    private readonly Dictionary<string, GenreArtistFlyweight> _pool = new();

    private FlyweightPool() { }

    public GenreArtistFlyweight Get(string genre, string artist)
    {
        string key = $"{genre}|{artist}";

        if (!_pool.ContainsKey(key))
        {
            _pool[key] = new GenreArtistFlyweight(genre, artist);
            Console.Error.WriteLine($"[Flyweight] Creat: {key}");
        }
        else
        {
            Console.Error.WriteLine($"[Flyweight] Reutilizat: {key}");
        }

        return _pool[key];
    }

    public void PrintStats()
    {
        Console.Error.WriteLine($"\n[Flyweight] {_pool.Count} obiecte in pool:");
        foreach (var f in _pool.Values)
            Console.Error.WriteLine($"  {f.Artist} / {f.Genre} → {f.Uses}x");
    }
}