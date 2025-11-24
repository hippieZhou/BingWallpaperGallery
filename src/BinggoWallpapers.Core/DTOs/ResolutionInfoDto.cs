// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.DTOs;

/// <summary>
/// 分辨率信息数据传输对象
/// </summary>
/// <param name="Code">分辨率代码</param>
/// <param name="Name">分辨率名称</param>
/// <param name="Suffix">分辨率描述</param>
public record ResolutionInfoDto(
    ResolutionCode Code,
    string Name,
    string Suffix);
