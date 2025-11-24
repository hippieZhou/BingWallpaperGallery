// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Http.Network;

public interface IBingWallpaperClient
{
    Task<IEnumerable<BingWallpaperInfo>> GetWallpapersAsync(
        int count,
        MarketCode marketCode,
        ResolutionCode resolution,
        CancellationToken cancellationToken = default);
}
