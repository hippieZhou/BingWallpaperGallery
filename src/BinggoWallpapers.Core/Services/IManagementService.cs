// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DTOs;

namespace BinggoWallpapers.Core.Services;

/// <summary>
/// 必应壁纸管理服务接口
/// 提供壁纸收集、查询和统计功能
/// </summary>
public interface IManagementService
{
    /// <summary>
    /// 运行壁纸收集流程
    /// 初始化数据库并启动壁纸收集服务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task RunCollectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定市场代码的今日壁纸信息
    /// </summary>
    /// <param name="market"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WallpaperInfoDto> GetLatestAsync(
        MarketInfoDto market,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定市场代码的所有壁纸信息
    /// </summary>
    /// <param name="market">市场代码</param>
    /// <param name="pageNumber">页码（从1开始）</param>
    /// <param name="pageSize">每页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>壁纸信息列表</returns>
    Task<IEnumerable<WallpaperInfoDto>> GetByMarketCodeAsync(
        MarketInfoDto market,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有支持的市场信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>市场信息集合</returns>
    Task<IEnumerable<MarketInfoDto>> GetSupportedMarketCodesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有支持的壁纸分辨率信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task<IEnumerable<ResolutionInfoDto>> GetSupportedResolutionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定壁纸的详细信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string> GetMoreDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
