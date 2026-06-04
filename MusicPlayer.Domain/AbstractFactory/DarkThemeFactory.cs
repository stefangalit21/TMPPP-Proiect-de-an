namespace MusicPlayer.Application.Patterns.AbstractFactory;

public class DarkThemeFactory : IThemeFactory
{
    public IColors CreateColors() => new DarkColors();
    public IFonts  CreateFonts()  => new DarkFonts();
}