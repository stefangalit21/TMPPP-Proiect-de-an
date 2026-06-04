namespace MusicPlayer.Application.Patterns.AbstractFactory;

public interface IThemeFactory
{
    IColors CreateColors();
    IFonts  CreateFonts();
}