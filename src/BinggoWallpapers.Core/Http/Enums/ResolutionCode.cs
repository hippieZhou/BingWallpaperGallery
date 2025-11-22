// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Attributes;

namespace BinggoWallpapers.Core.Http.Enums;

/// <summary>
/// API请求支持的分辨率类型
/// </summary>
[Flags]
public enum ResolutionCode
{
    /// <summary>
    /// 标准分辨率 - 1366x768
    /// </summary>
    [ResolutionInfo("Standard", "_1366x768.jpg", 1366, 768)]
    Standard,

    /// <summary>
    /// Full HD - 1920x1080
    /// </summary>
    [ResolutionInfo("Full HD", "_1920x1080.jpg", 1920, 1080)]
    FullHD,

    /// <summary>
    /// HD - 1920x1200
    /// </summary>
    [ResolutionInfo("HD", "_1920x1200.jpg", 1920, 1200)]
    HD,

    /// <summary>
    /// 4K Ultra HD - 3840x2160
    /// </summary>
    [ResolutionInfo("UHD", "_UHD.jpg", 3840, 2160)]
    UHD4K,
}
