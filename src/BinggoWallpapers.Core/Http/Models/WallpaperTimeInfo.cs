// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 时间信息类
/// </summary>
public sealed class WallpaperTimeInfo
{
    /// <summary>
    /// 原始开始日期 (YYYYMMDD)
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// 原始完整开始时间 (YYYYMMDDHHMM)
    /// </summary>
    public DateTime FullStartDateTime { get; set; }

    /// <summary>
    /// 原始结束日期 (YYYYMMDD)
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// 从Bing API时间字段创建WallpaperTimeInfo
    /// </summary>
    public static WallpaperTimeInfo FromBingApiFields(DateOnly startDate, DateTime fullStartDate, DateOnly endDate)
    {
        return new WallpaperTimeInfo
        {
            StartDate = startDate,
            FullStartDateTime = fullStartDate,
            EndDate = endDate
        };
    }
}
