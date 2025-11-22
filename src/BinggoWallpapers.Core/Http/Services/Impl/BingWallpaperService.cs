// Copyright (c) hippieZhou. All rights reserved.

using System.Diagnostics;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;
using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Network;
using BinggoWallpapers.Core.Http.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BinggoWallpapers.Core.Http.Services.Impl;

/// <summary>
/// 必应壁纸信息收集服务实现
/// </summary>
public sealed class BingWallpaperService(
    IBingWallpaperClient httpClient,
    IOptions<CollectionOptions> options,
    ILogger<BingWallpaperService> logger) : IBingWallpaperService
{
    public async Task<IEnumerable<CollectedWallpaperInfo>> CollectAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            logger.LogInformation("🚀 应用程序启动，开始初始化...");

            // 获取用户配置
            var config = options.Value;

            logger.LogInformation("=== 开始收集必应壁纸信息 ===");
            logger.LogInformation("配置信息:");
            logger.LogInformation("  - 目标国家: {Country}", config.CollectAllCountries ? "所有支持的国家" : config.MarketCode.ToString());
            logger.LogInformation("  - 目标分辨率: {ResolutionCode}", config.ResolutionCode);
            logger.LogInformation("  - 历史天数: {Days} 天", config.CollectDays);
            logger.LogInformation("  - 并发请求: {Concurrent} 个", config.MaxConcurrentRequests);
            logger.LogInformation("  - JSON格式: {Format}", config.PrettyJsonFormat ? "美化" : "压缩");
            logger.LogInformation("================================");

            var result = config.CollectAllCountries
                ? await CollectForAllCountriesAsync(config, cancellationToken)
                : await CollectForCountryAsync(config.MarketCode, config.CollectDays, config, cancellationToken);
            stopwatch.Stop();

            logger.LogInformation("所有壁纸信息收集完成！总计: {Total},  耗时: {Duration}ms", result, stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "运行过程中发生错误: {Message}", ex.Message);

            // 返回失败结果
            return [];
        }
    }

    private async Task<IEnumerable<CollectedWallpaperInfo>> CollectForCountryAsync(
        MarketCode marketCode,
        int daysToCollect,
        CollectionOptions config,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(daysToCollect);

        var marketCodeStr = marketCode.GetMarketCode();
        logger.LogInformation("开始为 {Country} ({MarketCode}) 收集 {Days} 天的历史壁纸信息...",
            marketCode.ToString(), marketCodeStr, daysToCollect);

        var allWallpapers = await CollectWallpaperInfoForDayAsync(
            daysToCollect,
            marketCode,
            config.ResolutionCode,
            cancellationToken);

        return allWallpapers;
    }

    /// <summary>
    /// 为所有国家收集壁纸信息
    /// </summary>
    private async Task<IEnumerable<CollectedWallpaperInfo>> CollectForAllCountriesAsync(CollectionOptions config, CancellationToken cancellationToken)
    {
        var countries = Enum.GetValues<MarketCode>();
        var semaphore = new SemaphoreSlim(config.MaxConcurrentRequests, config.MaxConcurrentRequests);
        var tasks = new List<Task<IEnumerable<CollectedWallpaperInfo>>>();

        try
        {
            foreach (var country in countries)
            {
                tasks.Add(CollectForCountryWithSemaphore(country, config, semaphore, cancellationToken));
            }

            var results = await Task.WhenAll(tasks);

            // 合并所有结果
            var totalCollected = results.Length;

            var allWallpapers = results.SelectMany(r => r).ToList();

            logger.LogInformation("✅ 所有国家的壁纸信息收集完成 - 总计: {Total}", totalCollected);

            return allWallpapers;
        }
        finally
        {
            semaphore.Dispose();
        }
    }

    /// <summary>
    /// 使用信号量控制并发的国家信息收集
    /// </summary>
    private async Task<IEnumerable<CollectedWallpaperInfo>> CollectForCountryWithSemaphore(
        MarketCode marketCode,
        CollectionOptions config,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await CollectForCountryAsync(marketCode, config.CollectDays, config, cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// 收集指定天数的壁纸信息
    /// </summary>
    private async Task<IEnumerable<CollectedWallpaperInfo>> CollectWallpaperInfoForDayAsync(
        int count,
        MarketCode marketCode,
        ResolutionCode resolutionCode,
        CancellationToken cancellationToken)
    {
        try
        {
            var wallpapers = await httpClient.GetWallpapersAsync(count, marketCode, resolutionCode, cancellationToken);

            if (wallpapers.Any())
            {
                logger.LogDebug("获取到壁纸信息: {MarketCode} - {ResolutionCode} - {Count}", marketCode, resolutionCode, wallpapers.Count());

                var actualDate = DateTimeProvider.GetUtcNow();

                return [.. wallpapers.Select(x => new CollectedWallpaperInfo(
                    MarketCode: marketCode,
                    ResolutionCode: resolutionCode,
                    CollectionDate: actualDate,
                    WallpaperInfo: x
                ))];
            }
            else
            {
                logger.LogWarning("未获取到壁纸信息: {MarketCode} - Day {Day}", marketCode, count);

                return [];
            }
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "收集壁纸信息失败: {MarketCode} - Day {Day}", marketCode, count);
            return [];
        }
    }
}
