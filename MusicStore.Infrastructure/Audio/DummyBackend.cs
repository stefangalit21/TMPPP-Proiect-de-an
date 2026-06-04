namespace MusicStore.Infrastructure.Audio;

public class DummyBackend : IAudioBackend
{
    private bool     _loaded;
    private bool     _playing;
    private DateTime _start;
    private int      _offset;

    public bool IsLoaded => _loaded;

    public void Open(string path)
    {
        _loaded  = true;
        _playing = false;
        _offset  = 0;
        Console.Error.WriteLine($"[Dummy] Open: {Path.GetFileName(path)}");
    }

    public void Play()
    {
        _playing = true;
        _start   = DateTime.Now - TimeSpan.FromSeconds(_offset);
        Console.Error.WriteLine("[Dummy] Play");
    }

    public void Pause()
    {
        _offset  = GetPosition();
        _playing = false;
        Console.Error.WriteLine("[Dummy] Pause");
    }

    public void Stop()
    {
        _playing = false;
        _offset  = 0;
        Console.Error.WriteLine("[Dummy] Stop");
    }

    public void SetVolume(float v) { }

    public int GetPosition() =>
        _playing ? (int)(DateTime.Now - _start).TotalSeconds : _offset;
    
    public void Seek(int seconds)
    {
        _offset = seconds;
        if (_playing)
            _start = DateTime.Now - TimeSpan.FromSeconds(seconds);
    }
}