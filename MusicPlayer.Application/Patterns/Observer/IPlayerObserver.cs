using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Observer;

public interface IPlayerObserver
{
    void OnPlay(Track track);
    void OnPause(Track track);
    void OnStop();
}