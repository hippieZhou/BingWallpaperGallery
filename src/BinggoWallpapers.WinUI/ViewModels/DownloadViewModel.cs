// Copyright (c) hippieZhou. All rights reserved.

using System.Collections.ObjectModel;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Services;
using BinggoWallpapers.WinUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.WinUI.ViewModels;

public partial class DownloadViewModel : ObservableRecipient, INavigationAware
{
    private readonly IDownloadService _downloadService;
    private readonly ILogger<DownloadViewModel> _logger;

    public ObservableCollection<DownloadModel> Downloads = [];

    public DownloadViewModel(
        IDownloadService downloadService,
        ILogger<DownloadViewModel> logger)
    {
        _downloadService = downloadService;
        _logger = logger;

        // 订阅下载事件
        _downloadService.DownloadProgressUpdated += OnDownloadProgressUpdated;
        _downloadService.DownloadStatusChanged += OnDownloadStatusChanged;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
    }

    [RelayCommand]
    private void OnLoaded()
    {
        var downloads = _downloadService.GetAllDownloads();
        foreach (var download in downloads)
        {
            if (Downloads.Any(d => d.DownloadId == download.DownloadId))
            {
                continue;
            }

            var downloadModel = new DownloadModel(download);
            Downloads.Add(downloadModel);
        }
    }

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task OnClearDownloadQueue(CancellationToken cancellationToken = default)
    {
        await _downloadService.ClearDownloadQueueAsync(cancellationToken);
        Downloads.Clear();
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task OnCancelDownload(Guid downloadId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _downloadService.CancelDownloadAsync(downloadId, cancellationToken);
            _logger.LogInformation("取消下载任务: {DownloadId}", downloadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "取消下载任务失败: {DownloadId}", downloadId);
        }
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task OnRetryDownload(Guid downloadId, CancellationToken cancellationToken = default)
    {
        try
        {
            // 获取现有的下载信息
            var existingDownload = _downloadService.GetDownloadById(downloadId);
            if (existingDownload == null)
            {
                _logger.LogWarning("未找到下载任务: {DownloadId}", downloadId);
                return;
            }

            // 检查是否可以重试（只有失败或取消的任务才能重试）
            if (existingDownload.Status is not (DownloadStatus.Failed or DownloadStatus.Cancelled))
            {
                _logger.LogWarning("下载任务状态不允许重试: {DownloadId}, 状态: {Status}", downloadId, existingDownload.Status);
                return;
            }

            _logger.LogInformation("开始重试下载任务: {DownloadId}", downloadId);

            // 重新创建下载任务
            var newDownloadId = await _downloadService.DownloadAsync(
                existingDownload.Wallpaper,
                existingDownload.Resolution,
                cancellationToken);

            _logger.LogInformation("重试下载任务创建成功: 原ID {OldId} -> 新ID {NewId}", downloadId, newDownloadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重试下载任务失败: {DownloadId}", downloadId);
        }
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task OnDeleteDownload(Guid downloadId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _downloadService.DeleteDownloadAsync(downloadId, cancellationToken);

            // 从 UI 集合中移除
            var downloadModel = Downloads.FirstOrDefault(d => d.DownloadId == downloadId);
            if (downloadModel != null)
            {
                Downloads.Remove(downloadModel);
            }

            _logger.LogInformation("删除下载任务: {DownloadId}", downloadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除下载任务失败: {DownloadId}", downloadId);
        }
    }

    [RelayCommand]
    private void OnViewOriginalImage(Guid downloadId)
    {
        try
        {
            var downloadModel = Downloads.FirstOrDefault(d => d.DownloadId == downloadId);
            var filePath = downloadModel?.FilePath;
            if (File.Exists(filePath))
            {
                // 使用系统默认浏览器打开原图链接
                var argument = $"/select, \"{filePath}\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
                _logger.LogInformation("打开文件所在目录: {FilePath}", filePath);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "打开原图链接失败: {DownloadId}", downloadId);
        }
    }

    /// <summary>
    /// 处理下载进度更新事件
    /// </summary>
    private void OnDownloadProgressUpdated(object sender, DownloadProgressEventArgs e)
    {
        // 确保在 UI 线程上更新
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            var downloadModel = Downloads.FirstOrDefault(d => d.DownloadId == e.DownloadInfo.DownloadId);
            if (downloadModel is not null)
            {
                // 更新现有的 DownloadModel
                downloadModel.UpdateFromDownloadInfo();
            }
            else
            {
                // 创建新的 DownloadModel 并添加到集合中
                downloadModel = new DownloadModel(e.DownloadInfo);
                Downloads.Add(downloadModel);
            }
        });
    }

    /// <summary>
    /// 处理下载状态变更事件
    /// </summary>
    private void OnDownloadStatusChanged(object sender, DownloadStatusEventArgs e)
    {
        // 确保在 UI 线程上更新
        App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            var downloadModel = Downloads.FirstOrDefault(d => d.DownloadId == e.DownloadInfo.DownloadId);
            if (downloadModel is not null)
            {
                // 更新现有的 DownloadModel
                downloadModel.UpdateFromDownloadInfo();
            }
            else
            {
                // 创建新的 DownloadModel 并添加到集合中
                downloadModel = new DownloadModel(e.DownloadInfo);
                Downloads.Add(downloadModel);
            }
        });
    }
}
