// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess.Domains;
using BinggoWallpapers.Core.Http.Configuration;
using BinggoWallpapers.Core.Http.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace BinggoWallpapers.Core.DataAccess.Repositories.Impl;

/// <summary>
/// 壁纸数据仓储实现
/// 提供纯粹的数据访问操作，不包含业务逻辑
/// </summary>
public class WallpaperRepository(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<WallpaperRepository> logger) : IWallpaperRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory;
    private readonly ILogger<WallpaperRepository> _logger = logger;
    private readonly ResiliencePipeline _databaseRetryPipeline = ResilienceConfiguration.CreateDatabaseRetryPipeline();

    public async Task<bool> SaveIfNotExistsAsync(WallpaperEntity wallpaper, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(wallpaper);

        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            try
            {
                // 直接插入，让数据库约束来保证唯一性
                // 优点：
                // 1. 减少一次数据库查询，性能提升约 50%
                // 2. 利用数据库唯一约束保证数据一致性
                // 3. 无需事务，减少锁定时间
                // 4. 并发安全，无竞态条件
                await dbContext.Wallpapers.AddAsync(wallpaper, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                return true; // 成功插入新记录
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                // 唯一约束冲突 = 记录已存在
                // 这是正常的业务场景，使用 Debug 级别日志
                _logger.LogDebug("壁纸已存在，跳过保存: {MarketCode} - {ResolutionCode} - {Date}",
                    wallpaper.MarketCode, wallpaper.ResolutionCode, wallpaper.ActualDate);
                return false; // 记录已存在
            }
        }, cancellationToken);
    }

    public async Task<int> BulkSaveIfNotExistsAsync(IEnumerable<WallpaperEntity> entities, CancellationToken cancellationToken = default)
    {
        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var wallpapers = entities.ToList();
            if (wallpapers.Count == 0)
            {
                return 0;
            }

            // 优化策略：批量查询已存在的记录
            // 1. 收集所有要插入的记录的唯一标识 (MarketCode, ResolutionCode, Hash)
            var wallpaperKeys = wallpapers
                .Select(w => new { w.MarketCode, w.ResolutionCode, w.Hash })
                .ToList();

            // 2. 一次性查询数据库，获取所有已存在的记录
            // 使用 Contains 查询所有可能匹配的 Hash，然后在内存中精确匹配
            var hashes = wallpapers.Select(w => w.Hash).Distinct().ToList();
            var existingKeys = await dbContext.Wallpapers
                .AsNoTracking()
                .Where(w => hashes.Contains(w.Hash))
                .Select(w => new { w.MarketCode, w.ResolutionCode, w.Hash })
                .ToListAsync(cancellationToken);

            // 3. 构建 HashSet 用于快速查找（O(1) 复杂度）
            var existingSet = existingKeys.ToHashSet();

            // 4. 过滤出不存在的记录
            var newWallpapers = wallpapers
                .Where(w => !existingSet.Contains(new { w.MarketCode, w.ResolutionCode, w.Hash }))
                .ToList();

            if (newWallpapers.Count == 0)
            {
                _logger.LogDebug("批量保存：所有 {Count} 条记录均已存在，跳过", wallpapers.Count);
                return 0;
            }

            try
            {
                // 5. 批量添加新记录
                await dbContext.Wallpapers.AddRangeAsync(newWallpapers, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("批量保存成功：新增 {NewCount} 条记录（总共 {TotalCount} 条）",
                    newWallpapers.Count, wallpapers.Count);

                return newWallpapers.Count;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                // 并发冲突：在查询和插入之间，其他进程插入了相同的记录
                // 回退到逐个插入策略，跳过已存在的记录
                _logger.LogWarning("批量保存检测到并发冲突，回退到逐个插入模式");

                // 清理 ChangeTracker，移除失败的批量操作
                dbContext.ChangeTracker.Clear();

                int addedCount = 0;
                foreach (var wallpaper in newWallpapers)
                {
                    try
                    {
                        dbContext.Wallpapers.Add(wallpaper);
                        await dbContext.SaveChangesAsync(cancellationToken);
                        addedCount++;
                    }
                    catch (DbUpdateException innerEx) when (IsUniqueConstraintViolation(innerEx))
                    {
                        // 跳过已存在的记录
                        dbContext.Entry(wallpaper).State = EntityState.Detached;
                        _logger.LogDebug("跳过已存在的记录: {Hash}", wallpaper.Hash);
                    }
                }

                _logger.LogInformation("逐个插入完成：成功添加 {AddedCount}/{TotalCount} 条记录",
                    addedCount, newWallpapers.Count);

                return addedCount;
            }
        }, cancellationToken);
    }

    public async Task<IEnumerable<WallpaperEntity>> GetByMarketCodeAsync(
        MarketCode marketCode,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Wallpapers
                .AsNoTracking()
                .Where(w => w.MarketCode == marketCode)
                .OrderByDescending(w => w.ActualDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }, cancellationToken);
    }

    public async Task<IEnumerable<WallpaperEntity>> GetByDateRangeAsync(
        MarketCode marketCode,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Wallpapers
                .AsNoTracking()
                .Where(w => w.MarketCode == marketCode &&
                           w.ActualDate.Date >= startDate.Date &&
                           w.ActualDate.Date <= endDate.Date)
                .OrderByDescending(w => w.ActualDate)
                .ToListAsync(cancellationToken);
        }, cancellationToken);
    }

    public async Task<IEnumerable<WallpaperEntity>> GetLatestAsync(
        MarketCode marketCode,
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Wallpapers
                .AsNoTracking()
                .Where(w => w.MarketCode == marketCode)
                .OrderByDescending(w => w.ActualDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }, cancellationToken);
    }

    public async Task<WallpaperEntity> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _databaseRetryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await dbContext.Wallpapers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
        }, cancellationToken);
    }

    /// <summary>
    /// 检查异常是否为 SQLite 唯一约束违反
    /// </summary>
    /// <param name="ex">数据库更新异常</param>
    /// <returns>如果是唯一约束违反返回true</returns>
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // SQLite 错误码常量
        const int SqliteConstraintViolation = 19;           // SQLITE_CONSTRAINT
        const int SqliteConstraintUnique = 2067;            // SQLITE_CONSTRAINT_UNIQUE
        const int SqliteConstraintPrimaryKey = 1555;        // SQLITE_CONSTRAINT_PRIMARYKEY

        // 检查是否为 SQLite 异常
        if (ex.InnerException is Microsoft.Data.Sqlite.SqliteException sqliteEx)
        {
            // 通过错误码判断
            return sqliteEx.SqliteErrorCode == SqliteConstraintViolation ||
                   sqliteEx.SqliteExtendedErrorCode == SqliteConstraintUnique ||
                   sqliteEx.SqliteExtendedErrorCode == SqliteConstraintPrimaryKey;
        }

        // 通过错误消息判断（备用方案）
        var errorMessage = ex.InnerException?.Message ?? ex.Message;
        return errorMessage.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase) ||
               errorMessage.Contains("PRIMARY KEY must be unique", StringComparison.OrdinalIgnoreCase) ||
               errorMessage.Contains("constraint failed", StringComparison.OrdinalIgnoreCase);
    }
}
