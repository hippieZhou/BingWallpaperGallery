// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Http.Models;

public record CollectedWallpaperInfo(
    MarketCode MarketCode,
    ResolutionCode ResolutionCode,
    DateTimeOffset CollectionDate,
    BingWallpaperInfo WallpaperInfo);
