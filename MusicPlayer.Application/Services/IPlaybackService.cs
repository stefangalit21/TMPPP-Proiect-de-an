using MusicPlayer.Application.Patterns.Memento;
using MusicPlayer.Domain;

namespace MusicPlayer.Application.Services;

public interface IPlaybackService
{
    void        Play(string trackId);
    void        Pause();
    void        Stop();
    void        Next();
    void        Previous();
    void        SetVolume(float volume);
    void        SetMode(PlayMode mode);
    PlayerState GetState();
    PlayerMemento  CreateMemento();         
    void Seek(int seconds);   
    void RestoreMemento(PlayerMemento m);
}