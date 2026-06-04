using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Decorator;

public abstract class AudioEngineDecorator : IAudioEngine
{
    protected readonly IAudioEngine _inner;

    protected AudioEngineDecorator(IAudioEngine inner)
    {
        _inner = inner;
    }

    public virtual void Load(string filePath)   => _inner.Load(filePath);
    public virtual void Play()                  => _inner.Play();
    public virtual void Pause()                 => _inner.Pause();
    public virtual void Stop()                  => _inner.Stop();
    public virtual void SetVolume(float volume) => _inner.SetVolume(volume);
    public virtual int  GetPosition()           => _inner.GetPosition();
    public virtual bool IsLoaded                => _inner.IsLoaded;
    
    public virtual void Seek(int seconds) => _inner.Seek(seconds);

}