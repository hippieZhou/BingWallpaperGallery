// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;

namespace BinggoWallpapers.WinUI.Selectors;

public interface IMarketSelectorService
{
    IReadOnlyList<MarketInfoDto> SupportedMarkets { get; }
    MarketInfoDto Market { get; }

    Task InitializeAsync();
    Task SetMarketAsync(MarketInfoDto market);
}
