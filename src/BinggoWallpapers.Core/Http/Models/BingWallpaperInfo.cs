// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Json.Serialization;

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 必应壁纸信息类（原始API数据）
/// </summary>
public sealed class BingWallpaperInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("urlbase")]
    public string UrlBase { get; set; } = string.Empty;

    [JsonPropertyName("copyright")]
    public string Copyright { get; set; } = string.Empty;

    [JsonPropertyName("copyrightonly")]
    public string CopyrightOnly { get; set; } = string.Empty;

    [JsonPropertyName("copyrightlink")]
    public string CopyrightLink { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("bsTitle")]
    public string BsTitle { get; set; } = string.Empty;

    [JsonPropertyName("caption")]
    public string Caption { get; set; } = string.Empty;

    [JsonPropertyName("desc")]
    public string Desc { get; set; } = string.Empty;

    [JsonPropertyName("quiz")]
    public string Quiz { get; set; } = string.Empty;

    [JsonPropertyName("wp")]
    public bool Wp { get; set; }

    [JsonPropertyName("hsh")]
    public string Hash { get; set; } = string.Empty;

    [JsonPropertyName("drk")]
    public int Drk { get; set; }

    [JsonPropertyName("top")]
    public int Top { get; set; }

    [JsonPropertyName("bot")]
    public int Bot { get; set; }

    [JsonPropertyName("hs")]
    public object[] Hs { get; set; }

    [JsonPropertyName("startdate")]
    public DateOnly StartDate { get; set; }

    [JsonPropertyName("fullstartdate")]
    public DateTime FullStartDate { get; set; }

    [JsonPropertyName("enddate")]
    public DateOnly EndDate { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;
}
