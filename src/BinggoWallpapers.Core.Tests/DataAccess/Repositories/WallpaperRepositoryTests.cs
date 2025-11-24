// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.DataAccess;
using BinggoWallpapers.Core.DataAccess.Domains;
using BinggoWallpapers.Core.DataAccess.Repositories.Impl;
using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BinggoWallpapers.Core.Tests.DataAccess.Repositories;

public class WallpaperRepositoryTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly Mock<ILogger<WallpaperRepository>> _mockLogger;
    private readonly WallpaperRepository _repository;
    private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;

    public WallpaperRepositoryTests()
    {
        // 使用 SQLite 内存数据库以支持唯一约束
        // InMemory 提供程序不支持唯一约束，会导致测试失败
        // 使用 Mode=Memory 和 Cache=Shared 允许多个连接共享同一个内存数据库
        var connectionString = $"DataSource={Guid.NewGuid()};Mode=Memory;Cache=Shared";

        // 创建并保持连接打开（关键！SQLite 内存数据库在最后一个连接关闭时会被销毁）
        _connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        // 初始化数据库架构
        using (var context = new ApplicationDbContext(_options))
        {
            context.Database.EnsureCreated();
        }

        // 创建 DbContextFactory mock
        var factoryMock = new Mock<IDbContextFactory<ApplicationDbContext>>();
        factoryMock.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ApplicationDbContext(_options));

        _contextFactory = factoryMock.Object;
        _mockLogger = new Mock<ILogger<WallpaperRepository>>();
        _repository = new WallpaperRepository(_contextFactory, _mockLogger.Object);
    }

    public void Dispose()
    {
        // 清理 SQLite 内存数据库
        _connection?.Close();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task SaveIfNotExistsAsync_WithNewWallpaper_ShouldReturnTrue()
    {
        // Arrange
        var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now, "hash1");

        // Act
        var result = await _repository.SaveIfNotExistsAsync(wallpaper);

        // Assert
        result.Should().BeTrue();

        // Verify it was saved
        using var context = new ApplicationDbContext(_options);
        var saved = await context.Wallpapers.FirstOrDefaultAsync(w => w.Id == wallpaper.Id);
        saved.Should().NotBeNull();
        saved!.Hash.Should().Be("hash1");
    }

    [Fact]
    public async Task SaveIfNotExistsAsync_WithDuplicateWallpaper_ShouldReturnFalse()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var wallpaper1 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        var wallpaper2 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"); // Same market, date, and hash

        // Act
        var result1 = await _repository.SaveIfNotExistsAsync(wallpaper1);
        var result2 = await _repository.SaveIfNotExistsAsync(wallpaper2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeFalse();

        // Verify only one was saved
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task SaveIfNotExistsAsync_WithNullWallpaper_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveIfNotExistsAsync(null!));
    }

    [Fact]
    public async Task GetByMarketCodeAsync_ShouldReturnWallpapersSortedByDate()
    {
        // Arrange
        var wallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-2), "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-1), "hash2"),
            CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now, "hash3"),
            CreateTestWallpaper(MarketCode.China, DateTime.Now, "hash4") // Different market
        };

        foreach (var wallpaper in wallpapers)
        {
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Act
        var result = await _repository.GetByMarketCodeAsync(MarketCode.UnitedStates, pageNumber: 1, pageSize: 10);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(w => w.ActualDate);
        result.First().Hash.Should().Be("hash3"); // Most recent
    }

    [Fact]
    public async Task GetByMarketCodeAsync_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-i), $"hash{i}");
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Act
        var page1 = await _repository.GetByMarketCodeAsync(MarketCode.UnitedStates, pageNumber: 1, pageSize: 5);
        var page2 = await _repository.GetByMarketCodeAsync(MarketCode.UnitedStates, pageNumber: 2, pageSize: 5);

        // Assert
        page1.Should().HaveCount(5);
        page2.Should().HaveCount(5);
        page1.First().Hash.Should().Be("hash0"); // Most recent
        page2.First().Hash.Should().Be("hash5");
    }

    [Fact]
    public async Task GetByDateRangeAsync_ShouldReturnWallpapersInRange()
    {
        // Arrange
        var baseDate = new DateTime(2024, 1, 1);
        var wallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, baseDate.AddDays(0), "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, baseDate.AddDays(5), "hash2"),
            CreateTestWallpaper(MarketCode.UnitedStates, baseDate.AddDays(10), "hash3"),
            CreateTestWallpaper(MarketCode.UnitedStates, baseDate.AddDays(15), "hash4")
        };

        foreach (var wallpaper in wallpapers)
        {
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Act
        var result = await _repository.GetByDateRangeAsync(
            MarketCode.UnitedStates,
            baseDate.AddDays(3),
            baseDate.AddDays(12));

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(w => w.Hash == "hash2");
        result.Should().Contain(w => w.Hash == "hash3");
    }

    [Fact]
    public async Task GetLatestAsync_ShouldReturnMostRecentWallpapers()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-i), $"hash{i}");
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Act
        var result = await _repository.GetLatestAsync(MarketCode.UnitedStates, count: 5);

        // Assert
        result.Should().HaveCount(5);
        result.Should().BeInDescendingOrder(w => w.ActualDate);
        result.First().Hash.Should().Be("hash0"); // Most recent
        result.Last().Hash.Should().Be("hash4");
    }

    [Fact]
    public async Task GetLatestAsync_WithDefaultCount_ShouldReturn10Items()
    {
        // Arrange
        for (int i = 0; i < 15; i++)
        {
            var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-i), $"hash{i}");
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Act
        var result = await _repository.GetLatestAsync(MarketCode.UnitedStates);

        // Assert
        result.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetAsync_WithValidId_ShouldReturnWallpaper()
    {
        // Arrange
        var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now, "hash1");
        await _repository.SaveIfNotExistsAsync(wallpaper);

        // Act
        var result = await _repository.GetAsync(wallpaper.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(wallpaper.Id);
        result.Hash.Should().Be("hash1");
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveIfNotExistsAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var wallpaper = CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now, "hash1");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.SaveIfNotExistsAsync(wallpaper, cts.Token));
    }

    #region BulkSaveIfNotExistsAsync Tests

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithNewWallpapers_ShouldSaveAll()
    {
        // Arrange
        var wallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-1), "hash2"),
            CreateTestWallpaper(MarketCode.China, DateTime.Now, "hash3")
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);

        // Assert
        result.Should().Be(3);

        // Verify all were saved
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithEmptyList_ShouldReturnZero()
    {
        // Arrange
        var wallpapers = Array.Empty<WallpaperEntity>();

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithAllDuplicates_ShouldReturnZero()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var existingWallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2"),
            CreateTestWallpaper(MarketCode.China, date, "hash3")
        };

        // Save existing wallpapers
        foreach (var wallpaper in existingWallpapers)
        {
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Create duplicates with same market, date, and hash
        var duplicateWallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2"),
            CreateTestWallpaper(MarketCode.China, date, "hash3")
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(duplicateWallpapers);

        // Assert
        result.Should().Be(0);

        // Verify count didn't change
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithPartialDuplicates_ShouldSaveOnlyNew()
    {
        // Arrange
        var date = DateTime.Now.Date;

        // Save some existing wallpapers
        var existingWallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2")
        };

        foreach (var wallpaper in existingWallpapers)
        {
            await _repository.SaveIfNotExistsAsync(wallpaper);
        }

        // Create mix of new and duplicate wallpapers
        var mixedWallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),     // Duplicate
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2"), // Duplicate
            CreateTestWallpaper(MarketCode.China, date, "hash3"),            // New
            CreateTestWallpaper(MarketCode.Japan, date, "hash4")             // New
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(mixedWallpapers);

        // Assert
        result.Should().Be(2); // Only 2 new ones

        // Verify total count
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(4); // 2 existing + 2 new
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithLargeDataset_ShouldPerformEfficiently()
    {
        // Arrange
        var wallpapers = Enumerable.Range(0, 100)
            .Select(i => CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-i), $"hash{i}"))
            .ToList();

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);
        sw.Stop();

        // Assert
        result.Should().Be(100);
        sw.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete in less than 5 seconds

        // Verify all were saved
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(100);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithDifferentMarketsSameHash_ShouldSaveAll()
    {
        // Arrange - Same hash but different markets (allowed by unique constraint)
        var date = DateTime.Now.Date;
        var wallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.China, date, "hash1"),
            CreateTestWallpaper(MarketCode.Japan, date, "hash1")
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);

        // Assert
        result.Should().Be(3); // All should be saved (different markets)

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithDifferentResolutionsSameHash_ShouldSaveAll()
    {
        // Arrange - Same hash and market but different resolutions
        var date = DateTime.Now.Date;
        var wallpaper1 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        wallpaper1.ResolutionCode = ResolutionCode.FullHD;

        var wallpaper2 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        wallpaper2.ResolutionCode = ResolutionCode.UHD4K;

        var wallpapers = new[] { wallpaper1, wallpaper2 };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);

        // Assert
        result.Should().Be(2); // Both should be saved (different resolutions)

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var wallpapers = Enumerable.Range(0, 10)
            .Select(i => CreateTestWallpaper(MarketCode.UnitedStates, DateTime.Now.AddDays(-i), $"hash{i}"))
            .ToList();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.BulkSaveIfNotExistsAsync(wallpapers, cts.Token));
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_AfterPartialSave_ShouldOnlySaveRemaining()
    {
        // Arrange
        var date = DateTime.Now.Date;

        // First batch
        var firstBatch = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-2), "hash3")
        };

        await _repository.BulkSaveIfNotExistsAsync(firstBatch);

        // Second batch (overlapping with first)
        var secondBatch = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2"), // Duplicate
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-2), "hash3"), // Duplicate
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-3), "hash4"), // New
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-4), "hash5")  // New
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(secondBatch);

        // Assert
        result.Should().Be(2); // Only 2 new ones

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(5); // 3 from first batch + 2 from second batch
    }

    #endregion

    #region Concurrent Operations Tests

    [Fact]
    public async Task SaveIfNotExistsAsync_WithConcurrentSameWallpaper_ShouldHandleGracefully()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var wallpaper1 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        var wallpaper2 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"); // Same key

        // Act - Execute concurrently
        var tasks = new[]
        {
            _repository.SaveIfNotExistsAsync(wallpaper1),
            _repository.SaveIfNotExistsAsync(wallpaper2)
        };

        var results = await Task.WhenAll(tasks);

        // Assert - One should succeed, one should fail
        results.Should().Contain(true);
        results.Should().Contain(false);
        results.Count(r => r).Should().Be(1); // Only one success

        // Verify only one was saved
        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithConcurrentSameBatch_ShouldHandleGracefully()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var wallpapers1 = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2")
        };

        var wallpapers2 = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),     // Duplicate
            CreateTestWallpaper(MarketCode.UnitedStates, date.AddDays(-1), "hash2") // Duplicate
        };

        // Act - Execute concurrently
        var tasks = new[]
        {
            _repository.BulkSaveIfNotExistsAsync(wallpapers1),
            _repository.BulkSaveIfNotExistsAsync(wallpapers2)
        };

        var results = await Task.WhenAll(tasks);

        // Assert - Total saved should be 2 (no duplicates)
        results.Sum().Should().BeLessThanOrEqualTo(2);

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(2); // Only unique records
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public async Task SaveIfNotExistsAsync_WithSameHashDifferentMarket_ShouldSaveBoth()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var wallpaper1 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        var wallpaper2 = CreateTestWallpaper(MarketCode.China, date, "hash1"); // Same hash, different market

        // Act
        var result1 = await _repository.SaveIfNotExistsAsync(wallpaper1);
        var result2 = await _repository.SaveIfNotExistsAsync(wallpaper2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue(); // Should succeed (different market)

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task SaveIfNotExistsAsync_WithSameHashDifferentResolution_ShouldSaveBoth()
    {
        // Arrange
        var date = DateTime.Now.Date;
        var wallpaper1 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        wallpaper1.ResolutionCode = ResolutionCode.FullHD;

        var wallpaper2 = CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1");
        wallpaper2.ResolutionCode = ResolutionCode.UHD4K;

        // Act
        var result1 = await _repository.SaveIfNotExistsAsync(wallpaper1);
        var result2 = await _repository.SaveIfNotExistsAsync(wallpaper2);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue(); // Should succeed (different resolution)

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task BulkSaveIfNotExistsAsync_WithDuplicatesInSameBatch_ShouldSaveOnlyUnique()
    {
        // Arrange - Duplicate within the same batch
        var date = DateTime.Now.Date;
        var wallpapers = new[]
        {
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"),
            CreateTestWallpaper(MarketCode.UnitedStates, date, "hash1"), // Duplicate in batch
            CreateTestWallpaper(MarketCode.China, date, "hash2")
        };

        // Act
        var result = await _repository.BulkSaveIfNotExistsAsync(wallpapers);

        // Assert
        // Note: The behavior depends on whether the batch contains duplicates
        // The database should reject the duplicate via unique constraint
        result.Should().BeLessThanOrEqualTo(2);

        using var context = new ApplicationDbContext(_options);
        var count = await context.Wallpapers.CountAsync();
        count.Should().BeLessThanOrEqualTo(2);
    }

    #endregion

    private static WallpaperEntity CreateTestWallpaper(MarketCode marketCode, DateTime date, string hash)
    {
        return new WallpaperEntity
        {
            Id = Guid.NewGuid(),
            Hash = hash,
            ActualDate = date,
            MarketCode = marketCode,
            Info = new WallpaperInfoStorage
            {
                Title = $"Test Wallpaper {hash}",
                Copyright = "Test Copyright",
                Description = "Test Description",
                ImageResolutions = new List<ImageResolution>
                {
                    new() { Resolution = ResolutionCode.UHD4K, Url = "https://test.com/image.jpg" }
                }
            }
        };
    }
}

