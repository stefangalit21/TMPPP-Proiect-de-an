using Microsoft.AspNetCore.Mvc;
using MusicPlayer.Application.Patterns.Command;
using MusicPlayer.Application.Patterns.Factory;
using MusicPlayer.Application.Patterns.Iterator;
using MusicPlayer.Application.Patterns.Memento;
using MusicPlayer.Application.Patterns.Strategy;
using MusicPlayer.Application.Services;
using MusicPlayer.Domain;
using MusicStore.Infrastructure.Flyweight;
using MusicStore.Infrastructure.Metadata;

namespace MusicPlayer.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlaybackService _playback;
    private readonly IPlaylistService _playlist;
    private readonly IMetadataReader  _metadata;
    private readonly FlyweightPool    _flypool;
    private readonly CommandInvoker   _invoker;
    private readonly PlayerCaretaker  _caretaker;
 
    
    private readonly TrackFactory _fileFactory;
    private readonly TrackFactory _demoFactory;
 
    public PlayerController(
        IPlaybackService playback,
        IPlaylistService playlist,
        IMetadataReader  metadata,
        FlyweightPool    flypool,
        CommandInvoker   invoker,
        PlayerCaretaker  caretaker,
        FileTrackFactory fileFactory,
        DemoTrackFactory demoFactory)
    {
        _playback    = playback;
        _playlist    = playlist;
        _metadata    = metadata;
        _flypool     = flypool;
        _invoker     = invoker;
        _caretaker   = caretaker;
        _fileFactory = fileFactory;
        _demoFactory = demoFactory;
    }
 
    // ── State ─────────────────────────────────────────────────────────────
 
    [HttpGet("state")]
    public IActionResult GetState() =>
        Ok(_playback.GetState());
 
    // ── Playback ──────────────────────────────────────────────────────────
 
    [HttpPost("play")]
    public IActionResult Play([FromBody] IdRequest req)
    {
        _invoker.Execute(new PlayCommand(_playback, req.Id));
        return Ok(_playback.GetState());
    }
 
    [HttpPost("pause")]
    public IActionResult Pause()
    {
        _invoker.Execute(new PauseCommand(_playback));
        return Ok(_playback.GetState());
    }
 
    [HttpPost("stop")]
    public IActionResult Stop()
    {
        _invoker.Execute(new StopCommand(_playback));
        return Ok(_playback.GetState());
    }
 
    [HttpPost("next")]
    public IActionResult Next()
    {
        _invoker.Execute(new SkipCommand(_playback, forward: true));
        return Ok(_playback.GetState());
    }
    
 
    [HttpPost("previous")]
    public IActionResult Previous()
    {
        _invoker.Execute(new SkipCommand(_playback, forward: false));
        return Ok(_playback.GetState());
    }
 
    [HttpPost("undo")]
    public IActionResult Undo()
    {
        if (!_invoker.CanUndo)
            return BadRequest("Nimic de undo");
        _invoker.Undo();
        return Ok(_playback.GetState());
    }
 
    [HttpGet("history")]
    public IActionResult GetHistory() =>
        Ok(_invoker.GetHistory());
 
    [HttpPost("volume")]
    public IActionResult SetVolume([FromBody] VolumeRequest req)
    {
        _playback.SetVolume(req.Value);
        return Ok(_playback.GetState());
    }
 
    [HttpPost("mode")]
    public IActionResult SetMode([FromBody] ModeRequest req)
    {
        if (!Enum.TryParse<PlayMode>(req.Mode, out var mode))
            return BadRequest("Mod invalid. Valori: Normal, Shuffle, RepeatOne, RepeatAll");
        _playback.SetMode(mode);
        return Ok(_playback.GetState());
    }
 
    [HttpPost("seek")]
    public IActionResult Seek([FromBody] SeekRequest req)
    {
        _playback.Seek(req.Position);
        return Ok(_playback.GetState());
    }
 
    // ── Playlist ──────────────────────────────────────────────────────────
 
    [HttpGet("playlist")]
    public IActionResult GetPlaylist([FromQuery] string? sort = null)
    {
        ITrackSortStrategy? strategy = sort switch
        {
            "title"     => new SortByTitle(),
            "artist"    => new SortByArtist(),
            "duration"  => new SortByDuration(),
            "playcount" => new SortByPlayCount(),
            _           => null
        };
        return Ok(_playlist.GetAll(strategy));
    }
 
 
    [HttpPost("add")]
    public IActionResult AddFile([FromBody] PathRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Path))
            return BadRequest("Path-ul nu poate fi gol.");
 
        // Factory Method — Create() returnează ITrack
        var track = _fileFactory.Create(req.Path);
 
        // Flyweight: aplică gen/artist comun dacă există în pool
        _flypool.Get(track.Genre, track.Artist).ApplyTo(track);
 
        _playlist.AddTrack(track);
 
        Console.Error.WriteLine($"[Factory] FileTrackFactory → \"{track.Title}\" by {track.Artist}");
 
        return Ok(track);
    }
    
    [HttpPost("add-demo")]
    public IActionResult AddDemo([FromBody] TitleRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            return BadRequest("Titlul nu poate fi gol.");
        
        var track = _demoFactory.Create(req.Title);
        _playlist.AddTrack(track);
 
        Console.Error.WriteLine($"[Factory] DemoTrackFactory → {track.Title}");
 
        return Ok(new
        {
            track,
            factoryUsed = nameof(DemoTrackFactory),
            message     = $"Track demo creat: \"{track.Title}\" de {track.Artist}"
        });
    }

    [HttpPost("clone/{id}")]
    public IActionResult CloneTrack(string id)
    {
        var all   = _playlist.GetAll();
        var track = all.FirstOrDefault(t => t.Id == id);
        if (track == null)
            return NotFound($"Track-ul cu id={id} nu există.");
        
        var clone = track.Clone();
        _playlist.AddTrack(clone);
 
        Console.Error.WriteLine($"[Prototype] Clonat: {track.Title} → {clone.Id}");
 
        return Ok(new
        {
            original = new { track.Id, track.Title, track.PlayCount, track.IsFavorite },
            clone    = new { clone.Id, clone.Title, clone.PlayCount, clone.IsFavorite },
            message  = $"Clonat: \"{clone.Title}\" cu Id nou"
        });
    }
 
    [HttpDelete("track/{id}")]
    public IActionResult RemoveTrack(string id)
    {
        _playlist.RemoveTrack(id);
        return Ok(_playlist.GetAll());
    }
 
    // ── Observer / Log ────────────────────────────────────────────────────
 
    [HttpGet("log")]
    public IActionResult GetLog() =>
        Ok(new { observers = new[] { "NowPlayingObserver", "PlayCountObserver", "LoggerObserver" } });
 
    // ── Memento ───────────────────────────────────────────────────────────
 
    [HttpPost("save")]
    public IActionResult SaveState()
    {
        var memento = _playback.CreateMemento();
        _caretaker.Save(memento);
        return Ok(new { message = "Stare salvata", savedAt = memento.SavedAt, count = _caretaker.Count });
    }
 
    [HttpPost("restore")]
    public IActionResult RestoreState()
    {
        if (!_caretaker.CanRestore)
            return BadRequest("Nicio stare salvata");
        var memento = _caretaker.Restore();
        if (memento == null) return BadRequest("Restaurare esuata");
        _playback.RestoreMemento(memento);
        return Ok(_playback.GetState());
    }
 
    [HttpGet("snapshots")]
    public IActionResult GetSnapshots()
    {
        var snapshots = _caretaker.GetHistory().Select(m => new
        {
            trackId   = m.CurrentTrackId,
            position  = m.Position,
            volume    = m.Volume,
            mode      = m.Mode.ToString(),
            isPlaying = m.IsPlaying,
            savedAt   = m.SavedAt.ToString("HH:mm:ss")
        });
        return Ok(snapshots);
    }
 
    // ── Iterator ──────────────────────────────────────────────────────────
 
    [HttpGet("iterator")]
    public IActionResult GetIteratorInfo([FromQuery] bool shuffle = false)
    {
        var iterator = _playlist.GetIterator(shuffle);
        return Ok(new
        {
            count        = iterator.Count,
            currentIndex = iterator.CurrentIndex,
            shuffle,
            tracks       = GetIteratorTracks(iterator)
        });
    }
 
    [HttpPost("iterator/next")]
    public IActionResult IteratorNext()
    {
        var iterator = _playlist.GetIterator();
        var state    = _playback.GetState();
        if (!string.IsNullOrEmpty(state.CurrentId))
        {
            try { ((PlaylistIterator)iterator).GoTo(state.CurrentId); } catch { }
        }
        if (!iterator.HasNext()) return BadRequest("Nu exista track urmator");
        var next = iterator.Next();
        _invoker.Execute(new PlayCommand(_playback, next.Id));
        return Ok(new { track = next, state = _playback.GetState() });
    }
 
    [HttpPost("iterator/previous")]
    public IActionResult IteratorPrevious()
    {
        var iterator = _playlist.GetIterator();
        var state    = _playback.GetState();
        if (!string.IsNullOrEmpty(state.CurrentId))
        {
            try { ((PlaylistIterator)iterator).GoTo(state.CurrentId); } catch { }
        }
        if (!iterator.HasPrevious()) return BadRequest("Nu exista track anterior");
        var prev = iterator.Previous();
        _invoker.Execute(new PlayCommand(_playback, prev.Id));
        return Ok(new { track = prev, state = _playback.GetState() });
    }
 
    [HttpPost("iterator/reset")]
    public IActionResult IteratorReset()
    {
        var iterator = _playlist.GetIterator();
        iterator.Reset();
        return Ok(new { track = iterator.Current() });
    }
 
    // ── Helpers ───────────────────────────────────────────────────────────
 
    private List<object> GetIteratorTracks(IPlaylistIterator iterator)
    {
        return _playlist.GetAll().Select((t, i) => (object)new
        {
            index    = i,
            id       = t.Id,
            title    = t.Title,
            artist   = t.Artist,
            duration = t.Duration
        }).ToList();
    }
}
 
// ── Request models ────────────────────────────────────────────────────────
public record IdRequest(string Id);
public record VolumeRequest(float Value);
public record SeekRequest(int Position);
public record ModeRequest(string Mode);
public record PathRequest(string Path);
public record TitleRequest(string Title);