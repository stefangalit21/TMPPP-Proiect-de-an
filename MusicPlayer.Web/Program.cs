using MusicPlayer.Application.Patterns.Command;
using MusicPlayer.Application.Patterns.Factory;
using MusicPlayer.Application.Patterns.Memento;
using MusicPlayer.Application.Patterns.Observer;
using MusicPlayer.Application.Services;
using MusicPlayer.Domain;
using MusicStore.Infrastructure.Audio;
using MusicStore.Infrastructure.Decorator;
using MusicStore.Infrastructure.Flyweight;
using MusicStore.Infrastructure.Metadata;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// ── Bridge: AudioEngine (RefinedAbstraction) + NAudioBackend (Implementor) ──
builder.Services.AddSingleton<IAudioEngine>(sp =>
{
    IAudioEngine engine = new AudioEngine(new NAudioBackend());
    // Decorator chain peste Bridge
    engine = new LoggingDecorator(engine);
    engine = new VolumeNormalizerDecorator(engine);
    engine = new FadeInDecorator(engine);
    return engine;
});

// ── PlayerService (implementează ambele interfețe) ────────────────────────
builder.Services.AddSingleton<PlayerService>(sp =>
    new PlayerService(sp.GetRequiredService<IAudioEngine>()));

builder.Services.AddSingleton<IPlaybackService>(sp =>
    sp.GetRequiredService<PlayerService>());

builder.Services.AddSingleton<IPlaylistService>(sp =>
    sp.GetRequiredService<PlayerService>());

// ── Factory Method: înregistrare factory-uri concrete ─────────────────────
// Controllerul primește TrackFactory (abstract) — nu știe tipul concret
builder.Services.AddSingleton<FileTrackFactory>();
builder.Services.AddSingleton<DemoTrackFactory>();

// ── Alte servicii ─────────────────────────────────────────────────────────
builder.Services.AddSingleton<CommandInvoker>();
builder.Services.AddSingleton<PlayerCaretaker>();

// Proxy: CachingMetadataProxy peste FileMetadataReader
builder.Services.AddSingleton<IMetadataReader>(sp =>
    new CachingMetadataProxy(new FileMetadataReader()));

builder.Services.AddSingleton(FlyweightPool.Instance);

// CORS pentru Electron
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// ── Observer setup ────────────────────────────────────────────────────────
var playerService      = app.Services.GetRequiredService<PlayerService>();
playerService.EventManager.Subscribe(new NowPlayingObserver());
playerService.EventManager.Subscribe(new PlayCountObserver());
playerService.EventManager.Subscribe(new LoggerObserver());

app.UseCors();
app.MapControllers();

// ── Seed cu DemoTrackFactory ──────────────────────────────────────────────
SeedDemo(
    app.Services.GetRequiredService<IPlaylistService>(),
    app.Services.GetRequiredService<FlyweightPool>(),
    app.Services.GetRequiredService<DemoTrackFactory>()
);

Console.WriteLine("[API] Pornit pe http://localhost:5000");
app.Run();

// ── Seed function ─────────────────────────────────────────────────────────
static void SeedDemo(IPlaylistService playlist, FlyweightPool pool, DemoTrackFactory factory)
{
    var titles = new[]
    {
        "Bohemian Rhapsody",
        "Hotel California",
        "Smells Like Teen Spirit",
        "Purple Rain",
        "Stairway to Heaven",
        "Like a Rolling Stone"
    };

    foreach (var title in titles)
    {
        // Factory Method — clientul apelează Create() pe interfața abstractă
        var track = factory.Create(title);
        pool.Get(track.Genre, track.Artist).ApplyTo(track);
        playlist.AddTrack(track);
        Console.Error.WriteLine($"[Seed] DemoTrackFactory → {track.Title}");
    }
}