// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 单个文件下载进度信息
/// </summary>
public sealed class FileDownloadProgress
{
    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 分辨率标识
    /// </summary>
    public string Resolution { get; set; } = string.Empty;

    /// <summary>
    /// 下载进度百分比 (0-100)
    /// </summary>
    public double PercentageComplete { get; set; }

    /// <summary>
    /// 已下载字节数
    /// </summary>
    public long BytesDownloaded { get; set; }

    /// <summary>
    /// 文件总大小（如果已知）
    /// </summary>
    public long? TotalBytes { get; set; }

    /// <summary>
    /// 下载速度 (字节/秒)
    /// </summary>
    public double BytesPerSecond { get; set; }

    /// <summary>
    /// 剩余时间估算
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// 下载状态
    /// </summary>
    public DownloadStatus Status { get; set; } = DownloadStatus.Pending;

    /// <summary>
    /// 错误信息（如果有）
    /// </summary>
    public string ErrorMessage { get; set; }
}
