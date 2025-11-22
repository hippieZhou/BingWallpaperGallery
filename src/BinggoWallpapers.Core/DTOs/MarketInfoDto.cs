// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;

namespace BinggoWallpapers.Core.DTOs;

/// <summary>
/// 市场信息数据传输对象
/// </summary>
/// <param name="Code">市场代码</param>
public record MarketInfoDto(MarketCode Code)
{
    public string CN { get; } = Code.GetMarketCNName();
    public string EN { get; } = Code.GetMarketENName();
    public string Flag { get; } = Code.GetMarketFlag();
}
