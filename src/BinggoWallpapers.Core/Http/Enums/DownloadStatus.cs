// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Http.Enums;

/// <summary>
/// 下载状态枚举
/// </summary>
[Flags]
public enum DownloadStatus
{
    /// <summary>
    /// 准备开始
    /// </summary>
    Pending,

    /// <summary>
    /// 正在下载
    /// </summary>
    InProgress,

    /// <summary>
    /// 下载完成
    /// </summary>
    Completed,

    /// <summary>
    /// 下载失败
    /// </summary>
    Failed,

    /// <summary>
    /// 下载取消
    /// </summary>
    Cancelled,
}
