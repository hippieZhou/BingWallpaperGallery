// Copyright (c) hippieZhou. All rights reserved.

using System.Collections.Concurrent;
using BinggoWallpapers.Core.DataAccess.Repositories;
using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Services.Impl;

/// <summary>
/// 下载服务实现
/// 提供壁纸下载队列管理和实际下载功能
/// 注意: 作为 Singleton 服务，使用 IServiceScopeFactory 来解析 Scoped 依赖
/// </summary>
public class DownloadService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DownloadService> logger) : IDownloadService
{
    private readonly ConcurrentDictionary<Guid, DownloadInfoDto> _downloadQueue = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _downloadCancellations = new();

    /// <summary>
    /// 下载进度更新事件
    /// </summary>
    public event EventHandler<DownloadProgressEventArgs> DownloadProgressUpdated;

    /// <summary>
    /// 下载状态变更事件
    /// </summary>
    public event EventHandler<DownloadStatusEventArgs> DownloadStatusChanged;

    public string DownloadPath { get; private set; }

    public Task SetRequestedDownloadPathAsync(string downloadPath)
    {
        ArgumentException.ThrowIfNullOrEmpty(downloadPath);

        try
        {
            // 验证路径是否有效
            var fullPath = Path.GetFullPath(downloadPath);

            // 如果目录不存在，尝试创建
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                logger.LogInformation("创建下载目录: {Path}", fullPath);
            }

            DownloadPath = fullPath;
            logger.LogInformation("设置下载路径: {Path}", DownloadPath);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "设置下载路径失败: {Path}", downloadPath);
            throw;
        }
    }

    public IReadOnlyList<DownloadInfoDto> GetAllDownloads()
    {
        return _downloadQueue.Values.OrderByDescending(d => d.StartTime).ToList().AsReadOnly();
    }

    public DownloadInfoDto GetDownloadById(Guid downloadId)
    {
        return _downloadQueue.TryGetValue(downloadId, out var download) ? download : null;
    }

    public async Task<Guid> DownloadAsync(
        WallpaperInfoDto wallpaper,
        ResolutionInfoDto resolution,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wallpaper);
        ArgumentNullException.ThrowIfNull(resolution);

        var downloadInfo = CreateDownloadInfo(wallpaper, resolution);

        // 检查是否已在队列中
        var existingDownload = _downloadQueue.Values.FirstOrDefault(d =>
            d.Wallpaper.Id == wallpaper.Id && d.Resolution.Code == resolution.Code &&
            d.Status is DownloadStatus.Pending or DownloadStatus.InProgress);

        if (existingDownload != null)
        {
            logger.LogInformation("壁纸已在下载队列中，返回现有任务: {Title}", wallpaper.Title);
            return existingDownload.DownloadId;
        }

        // 添加到下载队列
        _downloadQueue[downloadInfo.DownloadId] = downloadInfo;
        logger.LogInformation("添加下载任务: {Title} - {Resolution}", wallpaper.Title, resolution.Name);

        // 启动下载任务（不等待完成）
        _ = Task.Run(async () => await PerformDownloadAsync(downloadInfo, OnDownloadProgressUpdated, cancellationToken), cancellationToken);

        return await Task.FromResult(downloadInfo.DownloadId);
    }

    public Task CancelDownloadAsync(Guid downloadId, CancellationToken cancellationToken = default)
    {
        if (_downloadCancellations.TryGetValue(downloadId, out var cts))
        {
            cts.Cancel();
            logger.LogInformation("取消下载任务: {DownloadId}", downloadId);
        }

        if (_downloadQueue.TryGetValue(downloadId, out var download))
        {
            var cancelOldStatus = download.Status;
            download.Status = DownloadStatus.Cancelled;
            download.CompletedTime = DateTimeProvider.GetUtcNow().DateTime;
            OnDownloadStatusChanged(downloadId, cancelOldStatus, download.Status, download);
        }

        return Task.CompletedTask;
    }

    public Task ClearDownloadQueueAsync(CancellationToken cancellationToken = default)
    {
        var count = _downloadQueue.Count;

        // 取消所有正在进行的下载
        foreach (var cts in _downloadCancellations.Values)
        {
            cts.Cancel();
        }

        _downloadQueue.Clear();
        _downloadCancellations.Clear();

        logger.LogInformation("清理下载队列: 移除 {Count} 项", count);
        return Task.CompletedTask;
    }

    public Task DeleteDownloadAsync(Guid downloadId, CancellationToken cancellationToken = default)
    {
        // 首先尝试取消下载（如果正在进行）
        if (_downloadCancellations.TryGetValue(downloadId, out var cts))
        {
            cts.Cancel();
            _downloadCancellations.TryRemove(downloadId, out _);
        }

        // 从队列中移除
        if (_downloadQueue.TryRemove(downloadId, out var download))
        {
            logger.LogInformation("删除下载任务: {DownloadId} - {Title}", downloadId, download.Wallpaper.Title);
        }
        else
        {
            logger.LogWarning("未找到要删除的下载任务: {DownloadId}", downloadId);
        }

        return Task.CompletedTask;
    }

    private static DownloadInfoDto CreateDownloadInfo(WallpaperInfoDto wallpaper, ResolutionInfoDto resolution)
    {
        return new DownloadInfoDto
        {
            DownloadId = Guid.NewGuid(),
            Wallpaper = wallpaper,
            Resolution = resolution,
            Status = DownloadStatus.Pending,
            StartTime = DateTime.Now
        };
    }

    private async Task PerformDownloadAsync(
        DownloadInfoDto downloadInfo,
        Action<DownloadInfoDto> progressCallback,
        CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _downloadCancellations[downloadInfo.DownloadId] = cts;

        try
        {
            var oldStatus = downloadInfo.Status;
            downloadInfo.Status = DownloadStatus.InProgress;
            OnDownloadStatusChanged(downloadInfo.DownloadId, oldStatus, downloadInfo.Status, downloadInfo);
            progressCallback?.Invoke(downloadInfo);

            logger.LogInformation("开始下载壁纸: {Title} - {Resolution}",
                downloadInfo.Wallpaper.Title, downloadInfo.Resolution.Name);

            // 创建 scope 来解析 Scoped 依赖
            using var scope = serviceScopeFactory.CreateScope();
            var wallpaperRepository = scope.ServiceProvider.GetRequiredService<IWallpaperRepository>();
            var imageDownloadService = scope.ServiceProvider.GetRequiredService<IImageDownloadService>();

            // 获取图片URL
            var imageUrl = await GetImageUrlAsync(wallpaperRepository, downloadInfo.Wallpaper, downloadInfo.Resolution);
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new InvalidOperationException($"无法获取壁纸URL: {downloadInfo.Wallpaper.Title}");
            }

            // 准备下载参数
            var country = downloadInfo.Wallpaper.Market?.Code.ToString() ?? "Unknown";
            var date = downloadInfo.Wallpaper.Fullstartdate?.ToString("yyyy-MM-dd");

            // 创建进度报告器
            var fileProgress = new Progress<FileDownloadProgress>(progress =>
            {
                UpdateDownloadProgress(downloadInfo, progress);
                progressCallback?.Invoke(downloadInfo);
            });

            // 执行下载
            var filePath = await imageDownloadService.DownloadWallpaperAsync(
                DownloadPath,
                imageUrl,
                country,
                date,
                downloadInfo.Resolution.Name,
                fileProgress,
                cts.Token);

            if (!string.IsNullOrEmpty(filePath))
            {
                var completedOldStatus = downloadInfo.Status;
                downloadInfo.Status = DownloadStatus.Completed;
                downloadInfo.FilePath = filePath;
                downloadInfo.ProgressPercentage = 100;
                downloadInfo.CompletedTime = DateTimeProvider.GetUtcNow().DateTime;
                OnDownloadStatusChanged(downloadInfo.DownloadId, completedOldStatus, downloadInfo.Status, downloadInfo);
                logger.LogInformation("壁纸下载成功: {Title} -> {FilePath}",
                    downloadInfo.Wallpaper.Title, filePath);
            }
            else
            {
                throw new InvalidOperationException("下载返回空文件路径");
            }
        }
        catch (OperationCanceledException)
        {
            var cancelledOldStatus = downloadInfo.Status;
            downloadInfo.Status = DownloadStatus.Cancelled;
            downloadInfo.CompletedTime = DateTimeProvider.GetUtcNow().DateTime;
            OnDownloadStatusChanged(downloadInfo.DownloadId, cancelledOldStatus, downloadInfo.Status, downloadInfo);
            logger.LogInformation("下载被取消: {Title}", downloadInfo.Wallpaper.Title);
        }
        catch (Exception ex)
        {
            var failedOldStatus = downloadInfo.Status;
            downloadInfo.Status = DownloadStatus.Failed;
            downloadInfo.ErrorMessage = ex.Message;
            downloadInfo.CompletedTime = DateTimeProvider.GetUtcNow().DateTime;
            OnDownloadStatusChanged(downloadInfo.DownloadId, failedOldStatus, downloadInfo.Status, downloadInfo);

            logger.LogError(ex, "下载壁纸失败: {Title}", downloadInfo.Wallpaper.Title);
        }
        finally
        {
            progressCallback?.Invoke(downloadInfo);
            _downloadCancellations.TryRemove(downloadInfo.DownloadId, out _);
            cts.Dispose();
        }
    }

    private static void UpdateDownloadProgress(DownloadInfoDto downloadInfo, FileDownloadProgress progress)
    {
        downloadInfo.ProgressPercentage = progress.PercentageComplete;
        downloadInfo.BytesDownloaded = progress.BytesDownloaded;
        downloadInfo.TotalBytes = progress.TotalBytes ?? 0L;
        downloadInfo.DownloadSpeed = progress.BytesPerSecond;
        downloadInfo.EstimatedTimeRemaining = progress.EstimatedTimeRemaining ?? TimeSpan.Zero;
        downloadInfo.ErrorMessage = progress.ErrorMessage;
    }

    /// <summary>
    /// 获取图片 URL
    /// </summary>
    /// <param name="wallpaperRepository">壁纸仓储（从 scope 中解析）</param>
    /// <param name="wallpaper">壁纸信息</param>
    /// <param name="resolution">分辨率信息</param>
    /// <returns>图片 URL</returns>
    protected async virtual Task<string> GetImageUrlAsync(
        IWallpaperRepository wallpaperRepository,
        WallpaperInfoDto wallpaper,
        ResolutionInfoDto resolution)
    {
        try
        {
            var entity = await wallpaperRepository.GetAsync(wallpaper.Id);
            var url = entity.Info.ImageResolutions.FirstOrDefault(x => x.Resolution == resolution.Code)?.Url;

            // 如果已有URL，直接使用
            if (!string.IsNullOrEmpty(url))
            {
                return url;
            }

            logger.LogWarning("壁纸缺少URL信息: {Title}", wallpaper.Title);
            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取图片URL失败: {Title}", wallpaper.Title);
            return string.Empty;
        }
    }

    /// <summary>
    /// 触发下载进度更新事件
    /// </summary>
    /// <param name="downloadInfo">下载信息</param>
    private void OnDownloadProgressUpdated(DownloadInfoDto downloadInfo)
    {
        DownloadProgressUpdated?.Invoke(this, new DownloadProgressEventArgs(downloadInfo));
    }

    /// <summary>
    /// 触发下载状态变更事件
    /// </summary>
    /// <param name="downloadId">下载ID</param>
    /// <param name="oldStatus">旧状态</param>
    /// <param name="newStatus">新状态</param>
    /// <param name="downloadInfo">下载信息</param>
    private void OnDownloadStatusChanged(
        Guid downloadId,
        DownloadStatus oldStatus,
        DownloadStatus newStatus,
        DownloadInfoDto downloadInfo)
    {
        DownloadStatusChanged?.Invoke(this,
            new DownloadStatusEventArgs(
                downloadId,
                oldStatus,
                newStatus,
                downloadInfo));
    }
}
