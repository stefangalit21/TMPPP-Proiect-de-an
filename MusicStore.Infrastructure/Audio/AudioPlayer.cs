namespace MusicStore.Infrastructure.Audio;

public abstract class AudioPlayer : MusicPlayer.Domain.IAudioEngine
{
    protected readonly IAudioBackend Backend;
 
    protected AudioPlayer(IAudioBackend backend)
    {
        Backend = backend;
    }
 
    public virtual void Load(string filePath)   => Backend.Open(filePath);
    public virtual void Play()                  => Backend.Play();
    public virtual void Pause()                 => Backend.Pause();
    public virtual void Stop()                  => Backend.Stop();
    public virtual void SetVolume(float volume) => Backend.SetVolume(volume);
    public virtual int  GetPosition()           => Backend.GetPosition();
    public virtual bool IsLoaded                => Backend.IsLoaded;
    public virtual void Seek(int seconds)       => Backend.Seek(seconds);
}