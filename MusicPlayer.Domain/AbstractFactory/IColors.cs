namespace MusicPlayer.Application.Patterns.AbstractFactory;

public interface IColors
{
    string Bg     { get; }
    string Accent { get; }
    string Text   { get; }
}