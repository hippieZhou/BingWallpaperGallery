// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Network;

/// <summary>
/// 图片下载客户端接口
/// </summary>
public interface IImageDownloadClient
{
    Task<string> DownloadImageAsync(
        FileDownloadRequest request,
        IProgress<FileDownloadProgress> progress,
        CancellationToken cancellationToken);
}
