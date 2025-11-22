// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Models;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Http.Network.Impl;

/// <summary>
/// 图片下载服务实现
/// </summary>
public sealed class ImageDownloadClient(HttpClient httpClient, ILogger<ImageDownloadClient> logger) : IImageDownloadClient
{
    /// <summary>
    /// 下载单张图片
    /// </summary>
    /// <param name="request">下载请求</param>
    /// <param name="progress">进度报告</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>下载的图片文件路径，失败则返回null</returns>
    public async Task<string> DownloadImageAsync(
        FileDownloadRequest request,
        IProgress<FileDownloadProgress> progress = null,
        CancellationToken cancellationToken = default)
    {
        var imageUrl = request.ImageUrl;
        var country = request.Country;
        var date = request.Date;
        var resolution = request.Resolution;
        var downloadDirectory = request.DownloadDirectory;

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            logger.LogWarning("图片URL为空，跳过下载");
            return null;
        }

        ArgumentException.ThrowIfNullOrEmpty(country);
        ArgumentException.ThrowIfNullOrEmpty(date);
        ArgumentException.ThrowIfNullOrEmpty(resolution);

        // 解析文件扩展名
        var uri = new Uri(imageUrl);
        var fileName = Path.GetFileName(uri.LocalPath);
        if (string.IsNullOrWhiteSpace(fileName) || !Path.HasExtension(fileName))
        {
            // 如果无法从URL获取文件名，使用默认命名
            var extension = imageUrl.Contains(".jpg", StringComparison.InvariantCultureIgnoreCase) ? ".jpg" :
                           imageUrl.Contains(".png", StringComparison.InvariantCultureIgnoreCase) ? ".png" : ".jpg";
            fileName = $"{resolution}_wallpaper{extension}";
        }

        // 创建进度对象
        var downloadProgress = new FileDownloadProgress
        {
            FileName = fileName,
            Resolution = resolution,
            Status = DownloadStatus.Pending
        };

        // 汇报初始状态
        progress?.Report(downloadProgress);

        try
        {
            // 创建目录结构：Country/Date/Images/
            var countryDir = Path.Combine(downloadDirectory, country);
            var dateDir = Path.Combine(countryDir, date);
            var imagesDir = Path.Combine(dateDir, HTTPConstants.ImagesSubDirectoryName);
            Directory.CreateDirectory(imagesDir);

            // 生成完整的文件路径
            var filePath = Path.Combine(imagesDir, fileName);

            // 检查文件是否已存在
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 0)
                {
                    logger.LogDebug("📁 图片文件已存在，跳过下载: {FilePath}", filePath);

                    // 汇报跳过状态
                    downloadProgress.ErrorMessage = "文件已存在，跳过下载";
                    downloadProgress.Status = DownloadStatus.Completed;
                    downloadProgress.PercentageComplete = 100.0;
                    downloadProgress.BytesDownloaded = fileInfo.Length;
                    downloadProgress.TotalBytes = fileInfo.Length;
                    progress?.Report(downloadProgress);

                    return filePath;
                }
                else
                {
                    // 如果文件存在但大小为0，删除并重新下载
                    File.Delete(filePath);
                    logger.LogWarning("🗑️ 删除损坏的图片文件: {FilePath}", filePath);
                }
            }

            logger.LogInformation("📥 开始下载图片: {Resolution} - {FileName}", resolution, fileName);

            // 更新进度状态为下载中
            downloadProgress.Status = DownloadStatus.InProgress;
            progress?.Report(downloadProgress);

            // 创建HTTP请求
            using var payload = new HttpRequestMessage(HttpMethod.Get, imageUrl);
            payload.Headers.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);
            payload.Headers.Add("Accept", HTTPConstants.HttpHeaders.AcceptImage);
            payload.Headers.Add("Accept-Encoding", HTTPConstants.HttpHeaders.AcceptEncoding);
            payload.Headers.Add("Cache-Control", HTTPConstants.HttpHeaders.CacheControl);

            // 发送请求并下载
            using var response = await httpClient.SendAsync(payload, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = $"HTTP状态码: {response.StatusCode}";
                logger.LogError("❌ 下载图片失败，HTTP状态码: {StatusCode}, URL: {ImageUrl}", response.StatusCode, imageUrl);

                // 汇报失败状态
                downloadProgress.Status = DownloadStatus.Failed;
                downloadProgress.ErrorMessage = errorMsg;
                progress?.Report(downloadProgress);

                return null;
            }

            // 检查内容类型
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType != null && !contentType.StartsWith("image/"))
            {
                logger.LogWarning("⚠️ 响应内容类型不是图片: {ContentType}, URL: {ImageUrl}", contentType, imageUrl);
                // 但仍然尝试下载，因为有些服务器可能返回错误的Content-Type
            }

            // 获取文件大小
            var contentLength = response.Content.Headers.ContentLength;
            downloadProgress.TotalBytes = contentLength;
            var fileSizeText = contentLength.HasValue ? $"{contentLength.Value / 1024.0 / 1024.0:F2} MB" : "未知大小";

            logger.LogInformation("📊 图片信息: 大小 {FileSize}", fileSizeText);

            // 下载并保存文件（带进度汇报）
            using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

            await CopyStreamWithProgressAsync(contentStream, fileStream, downloadProgress, progress, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);

            // 验证下载的文件
            var downloadedFileInfo = new FileInfo(filePath);
            if (downloadedFileInfo.Length == 0)
            {
                File.Delete(filePath);
                logger.LogError("❌ 下载的文件大小为0，删除文件: {FilePath}", filePath);

                // 汇报失败状态
                downloadProgress.Status = DownloadStatus.Failed;
                downloadProgress.ErrorMessage = "下载的文件大小为0";
                progress?.Report(downloadProgress);

                return null;
            }

            logger.LogInformation("✅ 图片下载成功: {FilePath} ({FileSize})", filePath, $"{downloadedFileInfo.Length / 1024.0 / 1024.0:F2} MB");

            // 汇报完成状态
            downloadProgress.Status = DownloadStatus.Completed;
            downloadProgress.PercentageComplete = 100.0;
            downloadProgress.BytesDownloaded = downloadedFileInfo.Length;
            downloadProgress.TotalBytes = downloadedFileInfo.Length;
            progress?.Report(downloadProgress);

            return filePath;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "🌐 网络请求失败: {Message}, URL: {ImageUrl}", ex.Message, imageUrl);

            // 汇报失败状态
            downloadProgress.Status = DownloadStatus.Failed;
            downloadProgress.ErrorMessage = $"网络请求失败: {ex.Message}";
            progress?.Report(downloadProgress);

            return null;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            logger.LogError(ex, "⏱️ 下载超时: {Message}, URL: {ImageUrl}", ex.Message, imageUrl);

            // 汇报失败状态
            downloadProgress.Status = DownloadStatus.Failed;
            downloadProgress.ErrorMessage = $"下载超时: {ex.Message}";
            progress?.Report(downloadProgress);

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "💥 下载图片时发生未知错误: {Message}, URL: {ImageUrl}", ex.Message, imageUrl);

            // 汇报失败状态
            downloadProgress.Status = DownloadStatus.Failed;
            downloadProgress.ErrorMessage = $"未知错误: {ex.Message}";
            progress?.Report(downloadProgress);

            return null;
        }
    }

    /// <summary>
    /// 带进度汇报的流复制方法
    /// </summary>
    private static async Task CopyStreamWithProgressAsync(
        Stream source,
        FileStream destination,
        FileDownloadProgress progress,
        IProgress<FileDownloadProgress> progressReporter,
        CancellationToken cancellationToken = default)
    {
        var buffer = new byte[81920]; // 80KB 缓冲区
        var totalBytesRead = 0L;
        var startTime = DateTime.UtcNow;
        var lastReportTime = startTime;

        int bytesRead;
        while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            totalBytesRead += bytesRead;

            var currentTime = DateTime.UtcNow;

            // 更新进度（每100ms汇报一次，避免过于频繁）
            if (currentTime - lastReportTime >= TimeSpan.FromMilliseconds(HTTPConstants.ProgressReportIntervalMs))
            {
                progress.BytesDownloaded = totalBytesRead;

                // 计算进度百分比
                if (progress.TotalBytes.HasValue && progress.TotalBytes > 0)
                {
                    progress.PercentageComplete = (double)totalBytesRead / progress.TotalBytes.Value * 100.0;
                }

                // 计算下载速度
                var elapsedTime = currentTime - startTime;
                if (elapsedTime.TotalSeconds > 0)
                {
                    progress.BytesPerSecond = totalBytesRead / elapsedTime.TotalSeconds;

                    // 估算剩余时间
                    if (progress.TotalBytes.HasValue && progress.BytesPerSecond > 0)
                    {
                        var remainingBytes = progress.TotalBytes.Value - totalBytesRead;
                        progress.EstimatedTimeRemaining = TimeSpan.FromSeconds(remainingBytes / progress.BytesPerSecond);
                    }
                }

                progressReporter?.Report(progress);
                lastReportTime = currentTime;
            }
        }

        // 最终状态更新
        progress.BytesDownloaded = totalBytesRead;
        if (progress.TotalBytes.HasValue && progress.TotalBytes > 0)
        {
            progress.PercentageComplete = (double)totalBytesRead / progress.TotalBytes.Value * 100.0;
        }

        var finalElapsedTime = DateTime.UtcNow - startTime;
        if (finalElapsedTime.TotalSeconds > 0)
        {
            progress.BytesPerSecond = totalBytesRead / finalElapsedTime.TotalSeconds;
        }

        progress.EstimatedTimeRemaining = TimeSpan.Zero;
        progressReporter?.Report(progress);
    }
}
