// Copyright (c) hippieZhou. All rights reserved.

using System.Text.Json;
using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;
using BinggoWallpapers.Core.Http.Models;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Http.Network.Impl;

public class BingWallpaperClient(HttpClient httpClient, ILogger<BingWallpaperClient> logger) : IBingWallpaperClient
{
    /// <summary>
    /// 获取指定地区的壁纸画廊
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <param name="marketCode">市场代码</param>
    /// <param name="resolution">分辨率类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>壁纸信息集合</returns>
    public async Task<IEnumerable<BingWallpaperInfo>> GetWallpapersAsync(
        int count,
        MarketCode marketCode,
        ResolutionCode resolution,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("正在获取 {Region} 地区的 {Count} 张壁纸...", marketCode, count);

            var apiUrl = BuildApiUrl(0, count, marketCode, resolution);

            logger.LogDebug("正在获取 {Country} 的壁纸信息...", marketCode.ToString());

            // 创建带有特定语言头的请求
            using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            // 设置Accept-Language头以获取对应语言的内容
            var languageCode = marketCode.GetLanguageCodeFromMarket();
            request.Headers.Add("Accept-Language", $"{languageCode},en;q=0.9");

            // 设置User-Agent以模拟浏览器请求
            request.Headers.Add("User-Agent", HTTPConstants.HttpHeaders.UserAgent);

            // 添加其他可能有用的请求头
            request.Headers.Add("Accept", HTTPConstants.HttpHeaders.Accept);
            request.Headers.Add("Cache-Control", HTTPConstants.HttpHeaders.CacheControl);

            logger.LogInformation("🌐 正在发送请求到 Bing API: {ApiUrl}", apiUrl);
            var response = await httpClient.SendAsync(request, cancellationToken);
            logger.LogInformation("✅ 收到 Bing API 响应，状态码: {StatusCode}", response.StatusCode);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = await Json.ToObjectAsync<BingApiResponse>(content);

            if (apiResponse?.Images?.Count > 0)
            {
                logger.LogInformation("✅ 成功获取 {Region} 地区的 {Count} 张壁纸", marketCode, apiResponse.Images.Count);
                return apiResponse.Images;
            }

            logger.LogWarning("⚠️ 未获取到 {Region} 地区的壁纸信息", marketCode);
            return [];
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "获取 {Region} 地区壁纸信息失败: {Message}", marketCode, ex.Message);
            return [];
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "解析 {Region} 地区壁纸JSON数据失败: {Message}", marketCode, ex.Message);
            return [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取 {Region} 地区壁纸信息时发生未知错误: {Message}", marketCode, ex.Message);
            return [];
        }
    }

    /// <summary>
    /// 构建必应壁纸API请求URL
    /// </summary>
    /// <param name="dayIndex">天数索引 (0=今天, 1=昨天, 2=前天...)</param>
    /// <param name="count">获取数量 (1-8)</param>
    /// <param name="marketCode">市场代码</param>
    /// <param name="resolution">分辨率类型</param>
    /// <returns>完整的API请求URL</returns>
    /// <exception cref="ArgumentException">当 dayIndex 或 count 参数无效时抛出</exception>
    private static string BuildApiUrl(int dayIndex, int count, MarketCode marketCode, ResolutionCode resolution)
    {
        if (!IsValidDayIndex(dayIndex))
        {
            throw new ArgumentException($"无效的天数索引: {dayIndex}。有效范围: 0-{HTTPConstants.MaxHistoryDays - 1}", nameof(dayIndex));
        }

        if (!IsValidCount(count))
        {
            throw new ArgumentException($"无效的获取数量: {count}。有效范围: 1-{HTTPConstants.MaxHistoryDays}", nameof(count));
        }

        var marketCodeStr = marketCode.GetMarketCode();
        var languageCodeStr = marketCode.GetSimpleLanguageCode();
        (var width, var height) = resolution.GetResolutionDimensions();
        return string.Format(HTTPConstants.BingApiUrlTemplate, dayIndex, count, marketCodeStr, languageCodeStr, width, height);
    }

    /// <summary>
    /// 验证天数索引是否有效
    /// </summary>
    /// <param name="dayIndex">天数索引</param>
    /// <returns>是否有效</returns>
    private static bool IsValidDayIndex(int dayIndex) => dayIndex is >= 0 and < HTTPConstants.MaxHistoryDays;

    /// <summary>
    /// 验证获取数量是否有效
    /// </summary>
    /// <param name="count">获取数量</param>
    /// <returns>是否有效</returns>
    private static bool IsValidCount(int count) => count is > 0 and <= HTTPConstants.MaxHistoryDays;
}
