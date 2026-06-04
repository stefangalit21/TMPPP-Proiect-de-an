namespace MusicPlayer.Domain;


public interface IAudioEngine
{
    void Load(string filePath);
    void Play();
    void Pause();
    void Stop();
    void SetVolume(float volume);
    int  GetPosition();
    bool IsLoaded { get; }
    void Seek(int seconds);

}