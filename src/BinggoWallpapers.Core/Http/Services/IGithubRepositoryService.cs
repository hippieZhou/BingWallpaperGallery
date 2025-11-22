using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Services;
public interface IGithubRepositoryService
{
    Task<IEnumerable<ArchiveItem>> GetArchiveListAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<WallpaperInfoStorage>> GetArchiveDetailsAsync(ArchiveItem archiveItem, CancellationToken cancellationToken = default);
}
