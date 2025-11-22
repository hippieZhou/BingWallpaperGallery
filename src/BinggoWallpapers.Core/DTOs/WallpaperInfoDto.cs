// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.DTOs;

/// <summary>
/// 必应壁纸信息数据传输对象
/// </summary>
/// <param name="Id">壁纸唯一标识符</param>
/// <param name="Hash">壁纸唯一 Hash 值</param>
/// <param name="Startdate">壁纸开始日期</param>
/// <param name="Enddate">壁纸结束日期</param>
/// <param name="Fullstartdate">壁纸完整开始日期时间</param>
/// <param name="Market">地区代码</param>
/// <param name="Title">壁纸标题</param>
/// <param name="Copyright">版权信息</param>
/// <param name="CopyrightOnly">版权</param>
/// <param name="CopyrightLink">版权链接信息</param>
/// <param name="Caption">壁纸说明</param>
/// <param name="Description">详细描述</param>
/// <param name="Url">壁纸图片URL</param>
public record WallpaperInfoDto(
    Guid Id,
    string Hash,
    DateOnly? Startdate,
    DateOnly? Enddate,
    DateTime? Fullstartdate,
    MarketInfoDto Market,
    string Title,
    string Copyright,
    string CopyrightOnly,
    string CopyrightLink,
    string Caption,
    string Description,
    string Url);
