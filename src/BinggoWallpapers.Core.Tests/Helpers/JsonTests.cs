// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Helpers;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Tests.Helpers;

public class JsonTests
{
    [Fact]
    public void Stringify_WithSimpleObject_ShouldReturnFormattedJson()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 123 };

        // Act
        var result = Json.Stringify(obj);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"name\"");
        result.Should().Contain("\"value\"");
        result.Should().Contain("Test");
        result.Should().Contain("123");
    }

    [Fact]
    public async Task StringifyAsync_WithSimpleObject_ShouldReturnFormattedJson()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 123 };

        // Act
        var result = await Json.StringifyAsync(obj);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"name\"");
        result.Should().Contain("Test");
    }

    [Fact]
    public void ToObject_WithValidJson_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = "{\"name\":\"Test\",\"value\":123}";

        // Act
        var result = Json.ToObject<TestObject>(json);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Value.Should().Be(123);
    }

    [Fact]
    public async Task ToObjectAsync_WithValidJson_ShouldDeserializeCorrectly()
    {
        // Arrange
        var json = "{\"name\":\"Test\",\"value\":123}";

        // Act
        var result = await Json.ToObjectAsync<TestObject>(json);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Value.Should().Be(123);
    }

    [Fact]
    public void ToObject_WithNullProperties_ShouldIgnoreNullValues()
    {
        // Arrange
        var obj = new TestObject { Name = "Test", Value = 123 };
        var json = Json.Stringify(obj);

        // Act
        var result = Json.ToObject<TestObject>(json);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Value.Should().Be(123);
    }

    [Fact]
    public void Stringify_WithDateOnly_ShouldFormatCorrectly()
    {
        // Arrange
        var date = new DateOnly(2024, 10, 4);
        var obj = new { TestDate = date };

        // Act
        var result = Json.Stringify(obj);

        // Assert
        result.Should().Contain("20241004");
    }

    [Fact]
    public void Stringify_WithEnumValue_ShouldConvertToString()
    {
        // Arrange
        var obj = new { Market = MarketCode.UnitedStates };

        // Act
        var result = Json.Stringify(obj);

        // Assert
        result.Should().Contain("UnitedStates");
    }

    [Fact]
    public void ToObject_WithCaseInsensitiveProperty_ShouldDeserialize()
    {
        // Arrange
        var json = "{\"NAME\":\"Test\",\"VALUE\":456}";

        // Act
        var result = Json.ToObject<TestObject>(json);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Value.Should().Be(456);
    }

    private class TestObject
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}

