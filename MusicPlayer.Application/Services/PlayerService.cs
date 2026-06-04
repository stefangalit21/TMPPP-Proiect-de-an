using MusicPlayer.Application.Patterns.Iterator;
using MusicPlayer.Application.Patterns.Memento;
using MusicPlayer.Application.Patterns.Observer;
using MusicPlayer.Application.Patterns.Prototype;
using MusicPlayer.Application.Patterns.Strategy;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Services;

public class PlayerService : IPlaybackService, IPlaylistService
{
    private readonly IAudioEngine  _audio;
    private readonly List<ITrack>  _playlist = new();
    private readonly Random        _rng      = new();
    private PlayerState            _state    = new();
 
    public readonly PlayerEventManager EventManager = new();
 
    public PlayerService(IAudioEngine audio)
    {
        _audio = audio;
    }
 
    // ── IPlaybackService ──────────────────────────────────────────────────
 
    public void Play(string trackId)
    {
        var track = _playlist.FirstOrDefault(t => t.Id == trackId);
        if (track == null) return;
 
        _audio.Load(track.FilePath);
        _audio.Play();
        track.PlayCount++;
 
        _state.IsPlaying = true;
        _state.CurrentId = trackId;
        _state.Position  = 0;
 
        // Observer notificat cu Track concret (cast sigur)
        if (track is Track t2) EventManager.NotifyPlay(t2);
    }
 
    public void Pause()
    {
        _audio.Pause();
        _state.IsPlaying = false;
 
        var track = _playlist.FirstOrDefault(t => t.Id == _state.CurrentId);
        if (track is Track t2) EventManager.NotifyPause(t2);
    }
 
    public void Seek(int seconds)
    {
        _audio.Seek(seconds);
        _state.Position = seconds;
    }
 
    public void Stop()
    {
        _audio.Stop();
        _state.IsPlaying = false;
        _state.Position  = 0;
        EventManager.NotifyStop();
    }
 
    public void Next()
    {
        if (_playlist.Count == 0) return;
 
        ITrack? next = _state.Mode switch
        {
            PlayMode.Shuffle   => _playlist[_rng.Next(_playlist.Count)],
            PlayMode.RepeatOne => _playlist.FirstOrDefault(t => t.Id == _state.CurrentId),
            _                  => GetNextInOrder()
        };
 
        if (next != null) Play(next.Id);
    }
 
    public void Previous()
    {
        int idx = _playlist.FindIndex(t => t.Id == _state.CurrentId);
        if (idx > 0) Play(_playlist[idx - 1].Id);
    }
 
    public void SetVolume(float volume)
    {
        _state.Volume = Math.Clamp(volume, 0f, 1f);
        _audio.SetVolume(_state.Volume);
    }
 
    public void SetMode(PlayMode mode) => _state.Mode = mode;
 
    public PlayerState GetState()
    {
        if (_audio.IsLoaded)
            _state.Position = _audio.GetPosition();
        return _state;
    }
 
    // ── IPlaylistService ──────────────────────────────────────────────────
 
    public void AddTrack(ITrack track)
    {
        
            _playlist.Add(track);
    }
 
    public void RemoveTrack(string trackId) =>
        _playlist.RemoveAll(t => t.Id == trackId);
 
    public List<ITrack> GetAll(ITrackSortStrategy? strategy = null)
    {
        var list = new List<ITrack>(_playlist);
        return strategy == null ? list : strategy.Sort(list);
    }
 
    public IPlaylistIterator GetIterator(bool shuffle = false)
    {
      
        var tracks = _playlist.OfType<Track>().ToList();
        if (shuffle)
        {
            Console.Error.WriteLine("[Iterator] Creat ShuffleIterator");
            return new ShuffleIterator(tracks);
        }
        Console.Error.WriteLine("[Iterator] Creat PlaylistIterator");
        return new PlaylistIterator(tracks);
    }
 
    // ── Memento ───────────────────────────────────────────────────────────
 
    public PlayerMemento CreateMemento()
    {
        var state   = GetState();
        var memento = new PlayerMemento(
            currentTrackId: state.CurrentId,
            position:       state.Position,
            volume:         state.Volume,
            mode:           state.Mode,
            isPlaying:      state.IsPlaying);
        Console.Error.WriteLine($"[Memento] Created: {memento}");
        return memento;
    }
 
    public void RestoreMemento(PlayerMemento memento)
    {
        Console.Error.WriteLine($"[Memento] Restoring: {memento}");
        SetVolume(memento.Volume);
        SetMode(memento.Mode);
 
        if (!string.IsNullOrEmpty(memento.CurrentTrackId))
        {
            if (memento.IsPlaying)
                Play(memento.CurrentTrackId);
            else
            {
                var track = _playlist.FirstOrDefault(t => t.Id == memento.CurrentTrackId);
                if (track != null)
                {
                    _audio.Load(track.FilePath);
                    _state.CurrentId = memento.CurrentTrackId;
                    _state.IsPlaying = false;
                }
            }
        }
    }
 
    // ── Private helpers ───────────────────────────────────────────────────
 
    private ITrack? GetNextInOrder()
    {
        int idx = _playlist.FindIndex(t => t.Id == _state.CurrentId);
        if (idx == -1 || idx + 1 >= _playlist.Count) return null;
        return _playlist[idx + 1];
    }
}