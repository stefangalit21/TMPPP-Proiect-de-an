using MusicStore.Infrastructure.Metadata;
using Xunit;

namespace MusicPlayer.Tests.Infrastructure;

public class MetadataProxyTests
{
    [Fact]
    public void Proxy_FileNotFound_ShouldReturnNull()
    {
        var proxy = new CachingMetadataProxy(new FileMetadataReader());
        var track = proxy.Read("/inexistent/file.mp3");
        Assert.Null(track);
    }

    [Fact]
    public void Proxy_UnsupportedExtension_ShouldReturnNull()
    {
        var proxy = new CachingMetadataProxy(new FileMetadataReader());
        var track = proxy.Read("/document.pdf");
        Assert.Null(track);
    }

    [Fact]
    public void Proxy_NullResult_ShouldNotCache()
    {
        var proxy = new CachingMetadataProxy(new FileMetadataReader());
        proxy.Read("/inexistent.mp3");
        Assert.Equal(0, proxy.CacheSize);
    }
}