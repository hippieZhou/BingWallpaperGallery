// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Services;

/// <summary>
/// 下载服务接口
/// 提供壁纸下载队列管理和批量下载功能
/// </summary>
public interface IDownloadService
{
    /// <summary>
    /// 下载进度更新事件
    /// </summary>
    event EventHandler<DownloadProgressEventArgs> DownloadProgressUpdated;

    /// <summary>
    /// 下载状态变更事件
    /// </summary>
    event EventHandler<DownloadStatusEventArgs> DownloadStatusChanged;
    /// <summary>
    /// 默认下载路径
    /// </summary>
    string DownloadPath { get; }

    /// <summary>
    /// 获取所有下载项
    /// </summary>
    /// <returns>下载项列表</returns>
    IReadOnlyList<DownloadInfoDto> GetAllDownloads();

    /// <summary>
    /// 根据下载ID获取下载项
    /// </summary>
    /// <param name="downloadId">下载ID</param>
    /// <returns>下载项，如果不存在返回null</returns>
    DownloadInfoDto GetDownloadById(Guid downloadId);

    /// <summary>
    /// 设置下载路径
    /// </summary>
    /// <param name="downloadPath">下载路径</param>
    /// <returns></returns>
    Task SetRequestedDownloadPathAsync(string downloadPath);

    /// <summary>
    /// 下载单张壁纸（简单版本，不需要进度回调）
    /// 适用于 WallpaperDetailViewModel
    /// </summary>
    /// <param name="wallpaper">壁纸信息</param>
    /// <param name="resolution">分辨率信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>下载任务ID</returns>
    Task<Guid> DownloadAsync(
        WallpaperInfoDto wallpaper,
        ResolutionInfoDto resolution,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消下载任务
    /// </summary>
    /// <param name="downloadId">下载任务ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task CancelDownloadAsync(
        Guid downloadId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理下载队列
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task ClearDownloadQueueAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除下载任务
    /// </summary>
    /// <param name="downloadId">下载任务ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task DeleteDownloadAsync(
        Guid downloadId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 下载进度更新事件参数
/// </summary>
public class DownloadProgressEventArgs : EventArgs
{
    /// <summary>
    /// 下载信息
    /// </summary>
    public DownloadInfoDto DownloadInfo { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="downloadInfo">下载信息</param>
    public DownloadProgressEventArgs(DownloadInfoDto downloadInfo)
    {
        DownloadInfo = downloadInfo ?? throw new ArgumentNullException(nameof(downloadInfo));
    }
}

/// <summary>
/// 下载状态变更事件参数
/// </summary>
public class DownloadStatusEventArgs : EventArgs
{
    /// <summary>
    /// 下载ID
    /// </summary>
    public Guid DownloadId { get; }

    /// <summary>
    /// 旧状态
    /// </summary>
    public DownloadStatus OldStatus { get; }

    /// <summary>
    /// 新状态
    /// </summary>
    public DownloadStatus NewStatus { get; }

    /// <summary>
    /// 下载信息
    /// </summary>
    public DownloadInfoDto DownloadInfo { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="downloadId">下载ID</param>
    /// <param name="oldStatus">旧状态</param>
    /// <param name="newStatus">新状态</param>
    /// <param name="downloadInfo">下载信息</param>
    public DownloadStatusEventArgs(Guid downloadId, DownloadStatus oldStatus, DownloadStatus newStatus, DownloadInfoDto downloadInfo)
    {
        DownloadId = downloadId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        DownloadInfo = downloadInfo ?? throw new ArgumentNullException(nameof(downloadInfo));
    }
}

