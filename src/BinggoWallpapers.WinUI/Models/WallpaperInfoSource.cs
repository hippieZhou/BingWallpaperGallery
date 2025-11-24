// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Services;
using BinggoWallpapers.WinUI.Selectors;
using CommunityToolkit.WinUI.Collections;

namespace BinggoWallpapers.WinUI.Models;

public class WallpaperInfoSource(IMarketSelectorService marketSelectorService, IManagementService managementService) : IIncrementalSource<WallpaperInfoDto>
{
    public async Task<IEnumerable<WallpaperInfoDto>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
        var market = marketSelectorService.Market;
        return market is not null
            ? await managementService.GetByMarketCodeAsync(market, pageIndex + 1, pageSize, cancellationToken: cancellationToken)
            : [];
    }
}
