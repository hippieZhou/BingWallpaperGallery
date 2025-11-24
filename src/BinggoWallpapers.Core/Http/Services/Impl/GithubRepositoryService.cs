using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Network;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Http.Services.Impl;
public class GithubRepositoryService(
    IGithubRepositoryClient httpClient,
    ILogger<GithubRepositoryService> logger) : IGithubRepositoryService
{
    public async Task<IEnumerable<ArchiveItem>> GetArchiveListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("开始获取GitHub仓库归档...");
            var result = await httpClient.GetArchiveAsync("archive", cancellationToken: cancellationToken);
            logger.LogInformation("成功获取GitHub仓库归档。");
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取GitHub仓库归档时发生错误: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<WallpaperInfoStorage>> GetArchiveDetailsAsync(ArchiveItem archiveItem, CancellationToken cancellationToken = default)
    {
        var storageEntities = new List<WallpaperInfoStorage>();
        try
        {
            logger.LogInformation("开始获取GitHub归档详情...");
            var archiveItems = await httpClient.GetArchiveAsync(archiveItem.Path, cancellationToken);
            foreach (var item in archiveItems)
            {
                var storageEntity = await httpClient.GetArchiveFileAsync(item.DownloadUrl, cancellationToken);
                storageEntities.Add(storageEntity);
            }

            logger.LogInformation("成功获取GitHub归档详情。");
            return storageEntities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取GitHub归档详情时发生错误: {Message}", ex.Message);
            throw;
        }
    }
}
