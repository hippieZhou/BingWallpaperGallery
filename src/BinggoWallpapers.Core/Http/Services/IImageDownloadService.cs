// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Services;

/// <summary>
/// 图片下载服务接口
/// 提供壁纸图片下载功能的高级抽象
/// </summary>
public interface IImageDownloadService
{
    /// <summary>
    /// 下载单张壁纸图片
    /// </summary>
    /// <param name="downloadDirectory">图片根路径</param>
    /// <param name="imageUrl">图片URL</param>
    /// <param name="country">国家名称</param>
    /// <param name="date">日期</param>
    /// <param name="resolution">分辨率</param>
    /// <param name="progress">进度报告</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>下载成功返回文件路径，失败返回null</returns>
    Task<string> DownloadWallpaperAsync(
        string downloadDirectory,
        string imageUrl,
        string country,
        string date,
        string resolution,
        IProgress<FileDownloadProgress> progress = null,
        CancellationToken cancellationToken = default);
}
