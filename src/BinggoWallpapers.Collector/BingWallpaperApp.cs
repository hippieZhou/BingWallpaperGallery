using System.Diagnostics;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Services;
using BinggoWallpapers.Core.Mappers;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Collector;

/// <summary>
/// 必应壁纸信息收集器主应用类
/// </summary>
public sealed class BingWallpaperApp(
    IBingWallpaperService wallpaperService,
    ILogger<BingWallpaperApp> logger) : IDisposable
{
    private bool _disposed;

    /// <summary>
    /// 运行应用程序
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var collectedWallpapers = await wallpaperService.CollectAsync(cancellationToken);
            if (collectedWallpapers.Any())
            {
                logger.LogInformation("成功收集到 {Total} 张壁纸", collectedWallpapers.Count());
                if (collectedWallpapers.Any())
                {
                    await ProcessCollectedWallpapersAsync(collectedWallpapers, cancellationToken);
                }
            }
            else
            {
                logger.LogInformation("未收集到新的壁纸");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "应用程序运行失败: {Message}", ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("应用程序执行结束，耗时 {Duration}ms", stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private async Task ProcessCollectedWallpapersAsync(
        IEnumerable<CollectedWallpaperInfo> collectedWallpapers,
        CancellationToken cancellationToken)
    {
        var dataDirectory = Path.Combine(Environment.CurrentDirectory, HTTPConstants.DataDirectoryName);

        // 创建存储信息对象
        var wallpapers = collectedWallpapers.Select(x => WallpaperMapper.MapToStorage(x.WallpaperInfo, x.MarketCode, DateTimeProvider.GetUtcNow().DateTime));
        await Parallel.ForEachAsync(wallpapers, cancellationToken, async (wallpaper, token) =>
        {
            // 计算实际日期 - 使用 StartDate 确保与 JSON startDate 字段一致
            var dateStr = wallpaper.TimeInfo.StartDate.ToString("yyyy-MM-dd");
            var marketCode = wallpaper.Country;
            // 创建目录结构：Country/
            var countryDir = Path.Combine(dataDirectory, marketCode);
            Directory.CreateDirectory(countryDir);
            // 生成文件路径：使用日期作为文件名
            var fileName = $"{dateStr}.json";
            var filePath = Path.Combine(countryDir, fileName);
            // 检查文件是否已存在
            if (File.Exists(filePath))
            {
                logger.LogInformation("📋 JSON文件已存在，跳过保存: {Country} - {Date}", marketCode, dateStr);
                return;
            }

            try
            {
                var jsonContent = Json.Stringify(wallpaper);
                await File.WriteAllTextAsync(filePath, jsonContent, token);
                logger.LogInformation("✓ {Country} - {Date} - {Title}",
                    marketCode,
                    dateStr,
                    wallpaper.Title.Length > 20 ? wallpaper.Title[..20] + "..." : wallpaper.Title);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "保存 {Country} 壁纸信息时发生错误: {Message}", marketCode, ex.Message);
                throw;
            }
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}
