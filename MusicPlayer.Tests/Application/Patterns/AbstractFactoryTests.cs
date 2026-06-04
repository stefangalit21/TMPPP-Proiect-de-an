using MusicPlayer.Application.Patterns.AbstractFactory;
using Xunit;

namespace MusicPlayer.Tests.Application.Patterns;

public class AbstractFactoryTests
{
    [Fact]
    public void DarkThemeFactory_ShouldCreateDarkBackground()
    {
        IThemeFactory factory = new DarkThemeFactory();
        Assert.Equal("#0A0A0F", factory.CreateColors().Bg);
    }

    [Fact]
    public void LightThemeFactory_ShouldCreateLightBackground()
    {
        IThemeFactory factory = new LightThemeFactory();
        Assert.Equal("#F5F5FA", factory.CreateColors().Bg);
    }

    [Fact]
    public void DarkThemeFactory_ShouldCreateCorrectAccent()
    {
        IThemeFactory factory = new DarkThemeFactory();
        Assert.Equal("#7C4DFF", factory.CreateColors().Accent);
    }

    [Fact]
    public void DarkAndLight_ShouldHaveDifferentFonts()
    {
        IThemeFactory dark  = new DarkThemeFactory();
        IThemeFactory light = new LightThemeFactory();
        Assert.NotEqual(dark.CreateFonts().Display, light.CreateFonts().Display);
    }

    [Fact]
    public void DarkThemeFactory_AllProducts_ShouldNotBeNull()
    {
        IThemeFactory factory = new DarkThemeFactory();
        Assert.NotNull(factory.CreateColors());
        Assert.NotNull(factory.CreateFonts());
    }
}