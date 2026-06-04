namespace MusicPlayer.Application.Patterns.Memento;


public class PlayerCaretaker
{
    private readonly Stack<PlayerMemento> _history = new();
    private const int MaxHistory = 10;

    public void Save(PlayerMemento memento)
    {
        _history.Push(memento);
        Console.Error.WriteLine($"[Memento] Saved: {memento}");


        while (_history.Count > MaxHistory)
        {
            var list = _history.ToList();
            list.RemoveAt(list.Count - 1);
            _history.Clear();
            list.ForEach(m => _history.Push(m));
        }
    }

    public PlayerMemento? Restore()
    {
        if (_history.Count == 0)
        {
            Console.Error.WriteLine("[Memento] Nimic de restaurat");
            return null;
        }

        var memento = _history.Pop();
        Console.Error.WriteLine($"[Memento] Restored: {memento}");
        return memento;
    }

    public List<PlayerMemento> GetHistory() =>
        _history.ToList();

    public bool CanRestore => _history.Count > 0;
    public int  Count      => _history.Count;
}