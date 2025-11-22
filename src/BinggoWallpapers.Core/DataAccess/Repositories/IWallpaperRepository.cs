// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess.Domains;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.DataAccess.Repositories;

/// <summary>
/// 壁纸数据仓储接口
/// 提供纯粹的数据访问操作，不包含业务逻辑
/// </summary>
public interface IWallpaperRepository
{
    /// <summary>
    /// 原子性地保存壁纸实体（如果不存在的话）
    /// 使用数据库级别的并发控制防止重复插入
    /// </summary>
    /// <param name="wallpaper">壁纸实体</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>如果成功保存返回true，如果已存在返回false</returns>
    Task<bool> SaveIfNotExistsAsync(
        WallpaperEntity wallpaper,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量保存壁纸实体（如果不存在的话）
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> BulkSaveIfNotExistsAsync(
        IEnumerable<WallpaperEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据市场代码分页获取壁纸
    /// </summary>
    /// <param name="marketCode">市场代码</param>
    /// <param name="pageNumber">页码</param>
    /// <param name="pageSize">页大小</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>壁纸实体列表</returns>
    Task<IEnumerable<WallpaperEntity>> GetByMarketCodeAsync(
      MarketCode marketCode,
      int pageNumber,
      int pageSize,
      CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据日期范围获取壁纸
    /// </summary>
    /// <param name="marketCode">市场代码</param>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>壁纸实体列表</returns>
    Task<IEnumerable<WallpaperEntity>> GetByDateRangeAsync(
      MarketCode marketCode,
      DateTime startDate,
      DateTime endDate,
      CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取最新的壁纸
    /// </summary>
    /// <param name="marketCode">市场代码</param>
    /// <param name="count">获取数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>最新的壁纸实体列表</returns>
    Task<IEnumerable<WallpaperEntity>> GetLatestAsync(
      MarketCode marketCode,
      int count = 10,
      CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 来和获取对应壁纸
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<WallpaperEntity> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
