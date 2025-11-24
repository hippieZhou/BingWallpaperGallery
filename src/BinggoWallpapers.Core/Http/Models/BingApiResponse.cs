// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Json.Serialization;

namespace BinggoWallpapers.Core.Http.Models;

/// <summary>
/// 必应API响应类
/// </summary>
public sealed class BingApiResponse
{
    [JsonPropertyName("images")]
    public List<BingWallpaperInfo> Images { get; set; } = [];

    [JsonPropertyName("tooltips")]
    public object Tooltips { get; set; }
}
