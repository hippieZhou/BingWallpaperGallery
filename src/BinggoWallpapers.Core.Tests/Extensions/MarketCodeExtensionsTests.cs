// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Http.Enums;
using BinggoWallpapers.Core.Http.Extensions;

namespace BinggoWallpapers.Core.Tests.Extensions;

public class MarketCodeExtensionsTests
{
    [Theory]
    [InlineData(MarketCode.UnitedStates, "en-US")]
    [InlineData(MarketCode.China, "zh-CN")]
    [InlineData(MarketCode.Japan, "ja-JP")]
    [InlineData(MarketCode.UnitedKingdom, "en-GB")]
    [InlineData(MarketCode.Germany, "de-DE")]
    public void GetMarketCode_WithValidMarketCode_ShouldReturnCorrectCode(MarketCode marketCode, string expected)
    {
        // Act
        var result = marketCode.GetMarketCode();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(MarketCode.UnitedStates)]
    [InlineData(MarketCode.China)]
    [InlineData(MarketCode.Japan)]
    [InlineData(MarketCode.UnitedKingdom)]
    [InlineData(MarketCode.Germany)]
    public void GetMarketName_WithValidMarketCode_ShouldReturnNonEmptyString(MarketCode marketCode)
    {
        // Act
        var result = marketCode.GetMarketCNName();

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(MarketCode.UnitedStates)]
    [InlineData(MarketCode.China)]
    [InlineData(MarketCode.Japan)]
    public void GetMarketDescription_WithValidMarketCode_ShouldReturnNonEmptyString(MarketCode marketCode)
    {
        // Act
        var result = marketCode.GetMarketENName();

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(MarketCode.UnitedStates, "🇺🇸")]
    [InlineData(MarketCode.China, "🇨🇳")]
    [InlineData(MarketCode.Japan, "🇯🇵")]
    public void GetMarketFlag_WithValidMarketCode_ShouldReturnFlag(MarketCode marketCode, string expected)
    {
        // Act
        var result = marketCode.GetMarketFlag();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(MarketCode.UnitedStates, "en-US")]
    [InlineData(MarketCode.China, "zh-CN")]
    [InlineData(MarketCode.Japan, "ja-JP")]
    [InlineData(MarketCode.Germany, "de-DE")]
    public void GetLanguageCodeFromMarket_WithValidMarketCode_ShouldReturnCorrectLanguageCode(MarketCode marketCode, string expected)
    {
        // Act
        var result = marketCode.GetLanguageCodeFromMarket();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(MarketCode.UnitedStates, "en")]
    [InlineData(MarketCode.China, "zh")]
    [InlineData(MarketCode.Japan, "ja")]
    [InlineData(MarketCode.Germany, "de")]
    public void GetSimpleLanguageCode_WithValidMarketCode_ShouldReturnSimpleCode(MarketCode marketCode, string expected)
    {
        // Act
        var result = marketCode.GetSimpleLanguageCode();

        // Assert
        result.Should().Be(expected);
    }
}

