// Copyright (c) hippieZhou. All rights reserved.

using System.Diagnostics;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.DTOs;

/// <summary>
/// 下载信息数据传输对象
/// 包含下载任务的完整信息和进度状态
/// </summary>
[DebuggerDisplay("Download {DownloadId}: {TotalBytes} - {BytesDownloaded}")]
public record DownloadInfoDto
{
    /// <summary>
    /// 下载任务唯一标识符
    /// </summary>
    public Guid DownloadId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 壁纸信息
    /// </summary>
    public WallpaperInfoDto Wallpaper { get; set; } = null!;

    /// <summary>
    /// 分辨率信息
    /// </summary>
    public ResolutionInfoDto Resolution { get; set; } = null!;

    /// <summary>
    /// 下载状态
    /// </summary>
    public DownloadStatus Status { get; set; } = DownloadStatus.Pending;

    /// <summary>
    /// 下载进度百分比 (0-100)
    /// </summary>
    public double ProgressPercentage { get; set; }

    /// <summary>
    /// 已下载字节数
    /// </summary>
    public long BytesDownloaded { get; set; }

    /// <summary>
    /// 总字节数
    /// </summary>
    public long TotalBytes { get; set; }

    /// <summary>
    /// 下载速度 (字节/秒)
    /// </summary>
    public double DownloadSpeed { get; set; }

    /// <summary>
    /// 预计剩余时间
    /// </summary>
    public TimeSpan EstimatedTimeRemaining { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 完成时间
    /// </summary>
    public DateTime? CompletedTime { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 下载文件路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName => string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);
}
