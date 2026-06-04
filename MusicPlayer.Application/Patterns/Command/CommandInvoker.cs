namespace MusicPlayer.Application.Patterns.Command;

public class CommandInvoker
{
    private readonly Stack<IPlayerCommand> _history = new();

    public void Execute(IPlayerCommand command)
    {
        command.Execute();
        _history.Push(command);
        Console.Error.WriteLine($"[Command] Executed: {command.Name}");
    }

    public void Undo()
    {
        if (_history.Count == 0)
        {
            Console.Error.WriteLine("[Command] Nimic de undo");
            return;
        }

        var command = _history.Pop();
        command.Undo();
        Console.Error.WriteLine($"[Command] Undo: {command.Name}");
    }

    public List<string> GetHistory() =>
        _history.Select(c => c.Name).ToList();

    public bool CanUndo => _history.Count > 0;
}