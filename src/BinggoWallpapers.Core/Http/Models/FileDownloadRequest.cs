// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 图片下载请求信息
/// </summary>
/// <param name="DownloadDirectory"></param>
/// <param name="ImageUrl"></param>
/// <param name="Country"></param>
/// <param name="Date"></param>
/// <param name="Resolution"></param>
public sealed record FileDownloadRequest(
    string DownloadDirectory,
    string ImageUrl,
    string Country,
    string Date,
    string Resolution);
