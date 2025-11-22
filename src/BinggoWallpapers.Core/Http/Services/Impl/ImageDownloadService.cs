// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Models;
using BinggoWallpapers.Core.Http.Network;
using BinggoWallpapers.Core.Http.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BinggoWallpapers.Core.Http.Services.Impl;

/// <summary>
/// 图片下载服务实现
/// 封装 ImageDownloadClient，提供高级的图片下载功能
/// </summary>
public sealed class ImageDownloadService(
    IImageDownloadClient httpClient,
    IOptions<CollectionOptions> options,
    ILogger<ImageDownloadService> logger) : IImageDownloadService
{
    // 图片下载并发控制信号量
    private readonly SemaphoreSlim _downloadSemaphore = new(options.Value.MaxConcurrentDownloads, options.Value.MaxConcurrentDownloads);

    public async Task<string> DownloadWallpaperAsync(
        string downloadDirectory,
        string imageUrl,
        string country,
        string date,
        string resolution,
        IProgress<FileDownloadProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("开始下载壁纸: {Country} - {Date} - {Resolution}", country, date, resolution);

            await _downloadSemaphore.WaitAsync(cancellationToken);

            var request = new FileDownloadRequest(downloadDirectory,
                imageUrl,
                country,
                date,
                resolution);
            var filePath = await httpClient.DownloadImageAsync(
                request,
                progress,
                cancellationToken);

            if (!string.IsNullOrEmpty(filePath))
            {
                logger.LogInformation("壁纸下载成功: {FilePath}", filePath);
            }
            else
            {
                logger.LogWarning("壁纸下载失败: {ImageUrl}", imageUrl);
            }

            return filePath;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "下载壁纸时发生错误: {ImageUrl}", imageUrl);
            return null;
        }
        finally
        {
            _downloadSemaphore.Release();
        }
    }
}
