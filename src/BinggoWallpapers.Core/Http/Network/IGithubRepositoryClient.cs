using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Network;
public interface IGithubRepositoryClient
{
    Task<IEnumerable<ArchiveItem>> GetArchiveAsync(string path, CancellationToken cancellationToken = default);
    Task<WallpaperInfoStorage> GetArchiveFileAsync(string downloadUrl, CancellationToken cancellationToken = default);
}
