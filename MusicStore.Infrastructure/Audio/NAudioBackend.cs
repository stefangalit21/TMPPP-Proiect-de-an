using NAudio.Wave;

namespace MusicStore.Infrastructure.Audio;

public class NAudioBackend : IAudioBackend
{
    private AudioFileReader? _reader;
    private WaveOutEvent?    _output;


    public bool IsLoaded => _reader != null && _output != null;

    public void Open(string path)
    {
        _output?.Stop();
        _output?.Dispose();
        _reader?.Dispose();

        _reader = new AudioFileReader(path);
        _output = new WaveOutEvent();
        _output.Init(_reader);

        Console.Error.WriteLine($"[NAudio] Open: {Path.GetFileName(path)}");
    }

    public void Play()
    {
        _output?.Play();
        Console.Error.WriteLine("[NAudio] Play");
    }

    public void Pause()
    {
        _output?.Pause();
        Console.Error.WriteLine("[NAudio] Pause");
    }

    public void Stop()
    {
        _output?.Stop();
        if (_reader != null)
            _reader.Position = 0;
        Console.Error.WriteLine("[NAudio] Stop");
    }

    public void SetVolume(float v)
    {
        if (_output != null)
            _output.Volume = Math.Clamp(v, 0f, 1f);
    }

    public int GetPosition()
    {
        // ← verifica null inainte de orice
        if (_reader == null) return 0;
        try
        {
            return (int)_reader.CurrentTime.TotalSeconds;
        }
        catch
        {
            return 0;
        }
    }
    
    public void Seek(int seconds)
    {
        if (_reader == null) return;
        _reader.CurrentTime = TimeSpan.FromSeconds(seconds);
    }
}