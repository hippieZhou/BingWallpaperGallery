// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess.Domains;
using BinggoWallpapers.Core.DTOs;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;
using BinggoWallpapers.Core.Http.Models;

namespace BinggoWallpapers.Core.Mappers;

public static class WallpaperMapper
{
    /// <summary>
    /// 将WallpaperEntity映射为BingWallpaperInfoDto
    /// </summary>
    public static WallpaperInfoDto MapToDto(WallpaperEntity wallpaper)
    {
        ArgumentNullException.ThrowIfNull(wallpaper);

        var wallpaperInfo = wallpaper.Info;
        var timeInfo = wallpaperInfo.TimeInfo;

        return new WallpaperInfoDto(
            Id: wallpaper.Id,
            Hash: wallpaperInfo.Hash,
            Startdate: timeInfo.StartDate,
            Enddate: timeInfo.EndDate,
            Fullstartdate: timeInfo?.FullStartDateTime,
            Market: MapToMarketDto(wallpaper.MarketCode),
            Title: wallpaperInfo.Title ?? string.Empty,
            Copyright: wallpaperInfo.Copyright ?? string.Empty,
            CopyrightOnly: wallpaperInfo.CopyrightOnly ?? string.Empty,
            CopyrightLink: wallpaperInfo.CopyrightLink ?? string.Empty,
            Caption: wallpaperInfo.Caption ?? string.Empty,
            Description: wallpaperInfo.Description,
            Url: GetBestResolutionUrl(wallpaperInfo.ImageResolutions)
        );
    }

    public static WallpaperEntity MapToEntity(
        BingWallpaperInfo wallpaperInfo,
        MarketCode marketCode,
        DateTime actualDate)
    {
        var entity = new WallpaperEntity
        {
            Hash = wallpaperInfo.Hash,
            ActualDate = actualDate,
            MarketCode = marketCode,
            Info = MapToStorage(wallpaperInfo, marketCode, DateTimeProvider.GetUtcNow().DateTime)
        };

        return entity;
    }

    public static WallpaperEntity MapToEntity(WallpaperInfoStorage wallpaperInfoStorage)
    {
        var entity = new WallpaperEntity
        {
            Hash = wallpaperInfoStorage.Hash,
            ActualDate = wallpaperInfoStorage.CreatedAt,
            ResolutionCode = ResolutionCode.Standard,
            MarketCode = wallpaperInfoStorage.MarketCode.GetMarketFromLanguageCode(),
            Info = wallpaperInfoStorage
        };

        return entity;
    }

    public static WallpaperInfoStorage MapToStorage(BingWallpaperInfo wallpaperInfo, MarketCode marketCode, DateTime createdAt)
    {
        return new WallpaperInfoStorage
        {
            Country = marketCode.ToString(),
            MarketCode = marketCode.GetMarketCode(),
            Date = wallpaperInfo.Date,
            Title = wallpaperInfo.Title,
            BsTitle = wallpaperInfo.BsTitle,
            Caption = wallpaperInfo.Caption,
            Copyright = wallpaperInfo.Copyright,
            CopyrightOnly = wallpaperInfo.CopyrightOnly,
            CopyrightLink = wallpaperInfo.CopyrightLink,
            Description = wallpaperInfo.Desc,
            Quiz = wallpaperInfo.Quiz,
            Hash = wallpaperInfo.Hash,
            OriginalUrlBase = wallpaperInfo.UrlBase,
            ImageResolutions = GenerateImageResolutions(wallpaperInfo.UrlBase),
            TimeInfo = WallpaperTimeInfo.FromBingApiFields(
                wallpaperInfo.StartDate,
                wallpaperInfo.FullStartDate,
                wallpaperInfo.EndDate),
            CreatedAt = createdAt,
        };
    }

    public static MarketInfoDto MapToMarketDto(MarketCode marketCode)
    {
        return new MarketInfoDto(marketCode);
    }

    /// <summary>
    /// 获取最佳分辨率的图片URL
    /// </summary>
    private static string GetBestResolutionUrl(List<ImageResolution> resolutions)
    {
        return resolutions == null || resolutions.Count == 0
            ? string.Empty
            : resolutions.First(x => x.Resolution == ResolutionCode.Standard).Url;
    }

    private static List<ImageResolution> GenerateImageResolutions(string urlBase)
    {
        ArgumentException.ThrowIfNullOrEmpty(urlBase);

        var resolutions = new List<ImageResolution>();

        foreach (var resolution in HTTPConstants.GetSupportedResolutions())
        {
            var suffix = resolution.GetSuffix();
            (var width, var height) = resolution.GetResolutionDimensions();
            var imageUrl = $"{HTTPConstants.BingBaseUrl}{urlBase}{suffix}";

            resolutions.Add(new ImageResolution
            {
                Resolution = resolution,
                Url = imageUrl,
                Size = $"{width}x{height}"
            });
        }

        return resolutions;
    }

    public static ResolutionInfoDto ResolutionInfoDto(ResolutionCode resolutionCode)
    {
        (var width, var height) = resolutionCode.GetResolutionDimensions();
        return new ResolutionInfoDto(
            Code: resolutionCode,
            Name: resolutionCode.GetName(),
            Suffix: $"{width}x{height}"
        );
    }
}
