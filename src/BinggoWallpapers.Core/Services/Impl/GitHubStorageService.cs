using BinggoWallpapers.Core.DataAccess.Repositories;
using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Services;
using BinggoWallpapers.Core.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Services.Impl;

public class GitHubStorageService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<GitHubStorageService> logger) : IGitHubStorageService
{
    public async Task RunAsync(
        Action<string> onLoading = null,
        Action onEnded = null,
        Action<Exception> onError = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("开始获取 Github 归档文件");
            using var scope = serviceScopeFactory.CreateScope();
            var githubRepository = scope.ServiceProvider.GetRequiredService<IGithubRepositoryService>();
            var archiveItems = await githubRepository.GetArchiveListAsync(cancellationToken);
            onLoading?.Invoke($"成功获取 {archiveItems.Count()} 个国家的归档文件。");

            if (archiveItems.Any())
            {
                foreach (var archiveItem in archiveItems)
                {
                    onLoading?.Invoke($"正在同步 {archiveItem.Name} 国家/地区的归档文件...");
                    var wallpaperRepository = scope.ServiceProvider.GetRequiredService<IWallpaperRepository>();
                    await ProcessArchivedWallpapersAsync(githubRepository, wallpaperRepository, archiveItem, cancellationToken);
                    onLoading?.Invoke("所有国家/地区的归档文件同步完成。");
                }

                onLoading?.Invoke("GitHub 归档文件处理完成。");
            }
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
            logger.LogError(ex, "获取 GitHub 归档文件异常: {Message}", ex.Message);
        }
        finally
        {
            onEnded?.Invoke();
        }
    }

    private static async Task ProcessArchivedWallpapersAsync(
        IGithubRepositoryService githubRepository,
        IWallpaperRepository wallpaperRepository,
        ArchiveItem archiveItem,
        CancellationToken cancellationToken = default)
    {
        var wallpaperInfoStorages = await githubRepository.GetArchiveDetailsAsync(archiveItem, cancellationToken);
        var entities = wallpaperInfoStorages.Select(WallpaperMapper.MapToEntity);
        await wallpaperRepository.BulkSaveIfNotExistsAsync(entities, cancellationToken);
    }
}
