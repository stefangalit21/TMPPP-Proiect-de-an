namespace MusicPlayer.Application.Patterns.Command;

public interface IPlayerCommand
{
    void Execute();
    void Undo();
    string Name { get; }
}