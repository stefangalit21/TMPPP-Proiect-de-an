namespace MusicPlayer.Application.Patterns.AbstractFactory;

public class LightThemeFactory : IThemeFactory
{
    public IColors CreateColors() => new LightColors();
    public IFonts  CreateFonts()  => new LightFonts();
}