// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Helpers;

namespace BinggoWallpapers.Core.Tests.Helpers;

public class DateTimeProviderTests
{
    [Fact]
    public void GetUtcNow_ShouldReturnCurrentUtcTime()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var result = DateTimeProvider.GetUtcNow();

        // Assert
        var after = DateTimeOffset.UtcNow;
        result.Should().BeOnOrAfter(before);
        result.Should().BeOnOrBefore(after);
        result.Offset.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void GetUtcNow_CalledMultipleTimes_ShouldReturnIncreasingValues()
    {
        // Act
        var time1 = DateTimeProvider.GetUtcNow();
        Thread.Sleep(10); // Small delay to ensure time difference
        var time2 = DateTimeProvider.GetUtcNow();

        // Assert
        time2.Should().BeOnOrAfter(time1);
    }
}

