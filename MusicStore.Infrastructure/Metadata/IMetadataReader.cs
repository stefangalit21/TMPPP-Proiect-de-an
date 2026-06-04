using MusicPlayer.Domain;

namespace MusicStore.Infrastructure.Metadata;

public interface IMetadataReader
{
    Track? Read(string filePath);
}