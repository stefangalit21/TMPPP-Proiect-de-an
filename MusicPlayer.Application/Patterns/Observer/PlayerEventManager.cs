using MusicPlayer.Domain;

namespace MusicPlayer.Application.Patterns.Observer;

public class PlayerEventManager
{
    private readonly List<IPlayerObserver> _observers = new();

    public void Subscribe(IPlayerObserver observer)
    {
        _observers.Add(observer);
        Console.Error.WriteLine($"[Observer] Subscribed: {observer.GetType().Name}");
    }

    public void Unsubscribe(IPlayerObserver observer)
    {
        _observers.Remove(observer);
        Console.Error.WriteLine($"[Observer] Unsubscribed: {observer.GetType().Name}");
    }

    public void NotifyPlay(Track track)
    {
        Console.Error.WriteLine($"[Observer] NotifyPlay: {track.Title}");
        foreach (var o in _observers)
            o.OnPlay(track);
    }

    public void NotifyPause(Track track)
    {
        Console.Error.WriteLine($"[Observer] NotifyPause: {track.Title}");
        foreach (var o in _observers)
            o.OnPause(track);
    }

    public void NotifyStop()
    {
        Console.Error.WriteLine("[Observer] NotifyStop");
        foreach (var o in _observers)
            o.OnStop();
    }
}