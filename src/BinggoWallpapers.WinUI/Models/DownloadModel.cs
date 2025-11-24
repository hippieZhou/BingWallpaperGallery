// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Http.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.WinUI.Models;

public partial class DownloadModel : ObservableObject
{
    private readonly DownloadInfoDto _download;
    private readonly ILogger<DownloadModel> _logger;

    public DownloadModel(DownloadInfoDto download)
    {
        _download = download ?? throw new ArgumentNullException(nameof(download));
        _logger = App.GetService<ILogger<DownloadModel>>();

        // 初始化属性
        UpdateFromDownloadInfo();
    }

    /// <summary>
    /// 下载任务唯一标识符
    /// </summary>
    public Guid DownloadId => _download.DownloadId;

    /// <summary>
    /// 壁纸详情
    /// </summary>
    public WallpaperInfoDto Details => _download.Wallpaper;

    /// <summary>
    /// 分辨率信息
    /// </summary>
    public ResolutionInfoDto Resolution => _download.Resolution;

    /// <summary>
    /// 下载进度百分比 (0-100)
    /// </summary>
    [ObservableProperty]
    public partial double Progress { get; set; }

    [ObservableProperty]
    public partial double DownloadSpeed { get; set; }

    [ObservableProperty]
    public partial TimeSpan EstimatedTimeRemaining { get; set; }

    [ObservableProperty]
    public partial long TotalBytes { get; set; }

    /// <summary>
    /// 是否可以下载
    /// </summary>
    [ObservableProperty]
    public partial bool CanDelete { get; set; } = true;

    /// <summary>
    /// 是否可以取消下载
    /// </summary>
    [ObservableProperty]
    public partial bool CanCancel { get; set; } = true;

    /// <summary>
    /// 是否可以重试下载
    /// </summary>
    [ObservableProperty]
    public partial bool CanRetry { get; set; } = true;

    /// <summary>
    /// 下载状态
    /// </summary>
    [ObservableProperty]
    public partial DownloadStatus Status { get; set; } = DownloadStatus.Pending;

    /// <summary>
    /// 下载文件路径
    /// </summary>
    [ObservableProperty]
    public partial string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 文件名
    /// </summary>
    [ObservableProperty]
    public partial string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 错误信息
    /// </summary>
    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 从 DownloadInfoDto 更新属性
    /// 当下载进度发生变化时调用此方法
    /// </summary>
    public void UpdateFromDownloadInfo()
    {
        Progress = _download.ProgressPercentage;
        DownloadSpeed = _download.DownloadSpeed;
        EstimatedTimeRemaining = _download.EstimatedTimeRemaining;
        TotalBytes = _download.TotalBytes;
        Status = _download.Status;
        FilePath = _download.FilePath;
        FileName = _download.FileName;
        ErrorMessage = _download.ErrorMessage;

        // 更新删除状态
        CanDelete = Status is not DownloadStatus.InProgress;

        // 更新可取消状态
        CanCancel = Status is DownloadStatus.Pending or DownloadStatus.InProgress;

        // 更新可重试状态
        CanRetry = Status is DownloadStatus.Failed or DownloadStatus.Cancelled;

        _logger.LogInformation("下载任务 {DownloadId} 状态更新为 {Status}, 进度: {Progress}%", DownloadId, Status, Progress);
    }
}
