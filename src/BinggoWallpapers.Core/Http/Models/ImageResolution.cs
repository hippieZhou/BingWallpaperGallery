// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 图片分辨率信息
/// </summary>
public sealed class ImageResolution
{
    public ResolutionCode Resolution { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
}
