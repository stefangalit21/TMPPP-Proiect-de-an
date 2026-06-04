namespace MusicStore.Infrastructure.Audio;

public interface IAudioBackend
{
    void Open(string path);
    void Play();
    void Pause();
    void Stop();
    void SetVolume(float v);
    int  GetPosition();
    bool IsLoaded { get; }
    void Seek(int seconds);
}