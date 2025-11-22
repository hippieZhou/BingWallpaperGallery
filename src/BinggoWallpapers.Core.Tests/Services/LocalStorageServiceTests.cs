// Copyright (c) hippieZhou. All rights reserved.

using BinggoWallpapers.Core.Services.Impl;

namespace BinggoWallpapers.Core.Tests.Services;

public class LocalStorageServiceTests : IDisposable
{
    private readonly LocalStorageService _service;
    private readonly string _testFolder;

    public LocalStorageServiceTests()
    {
        _service = new LocalStorageService();
        _testFolder = Path.Combine(Path.GetTempPath(), $"FileServiceTests_{Guid.NewGuid()}");
    }

    public void Dispose()
    {
        // 清理测试文件夹
        if (Directory.Exists(_testFolder))
        {
            Directory.Delete(_testFolder, true);
        }

        GC.SuppressFinalize(this);
    }

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_WithValidData_ShouldCreateFileAndFolder()
    {
        // Arrange
        var fileName = "test.json";
        var content = new TestData { Name = "Test", Value = 123 };

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        Directory.Exists(_testFolder).Should().BeTrue();
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WithComplexObject_ShouldSerializeCorrectly()
    {
        // Arrange
        var fileName = "complex.json";
        var content = new ComplexTestData
        {
            Id = Guid.NewGuid(),
            Name = "Complex Object",
            Value = 456,
            Date = DateTime.Now,
            Items = new List<string> { "Item1", "Item2", "Item3" },
            Metadata = new Dictionary<string, string>
            {
                { "Key1", "Value1" },
                { "Key2", "Value2" }
            }
        };

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();

        var fileContent = await File.ReadAllTextAsync(filePath);
        fileContent.Should().Contain("Complex Object");
        fileContent.Should().Contain("Item1");
        fileContent.Should().Contain("Key1");
    }

    [Fact]
    public async Task SaveAsync_WhenFolderDoesNotExist_ShouldCreateFolder()
    {
        // Arrange
        var fileName = "test.json";
        var content = new TestData { Name = "Test", Value = 123 };

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        Directory.Exists(_testFolder).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_OverwriteExistingFile_ShouldUpdateContent()
    {
        // Arrange
        var fileName = "test.json";
        var originalContent = new TestData { Name = "Original", Value = 100 };
        var updatedContent = new TestData { Name = "Updated", Value = 200 };

        // Act
        await _service.SaveAsync(_testFolder, fileName, originalContent);
        await _service.SaveAsync(_testFolder, fileName, updatedContent);

        // Assert
        var filePath = Path.Combine(_testFolder, fileName);
        var fileContent = await File.ReadAllTextAsync(filePath);
        fileContent.Should().Contain("Updated");
        fileContent.Should().Contain("200");
        fileContent.Should().NotContain("Original");
    }

    [Fact]
    public async Task SaveAsync_WithUtf8Characters_ShouldSaveAndReadCorrectly()
    {
        // Arrange
        var fileName = "utf8.json";
        var content = new TestData
        {
            Name = "测试数据 🎉",
            Value = 999
        };

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);
        var result = await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert - The important thing is that it round-trips correctly
        result.Should().NotBeNull();
        result.Name.Should().Be("测试数据 🎉");
        result.Value.Should().Be(999);
    }

    [Fact]
    public async Task SaveAsync_WithNestedFolders_ShouldCreateAllFolders()
    {
        // Arrange
        var nestedFolder = Path.Combine(_testFolder, "subfolder1", "subfolder2");
        var fileName = "nested.json";
        var content = new TestData { Name = "Nested", Value = 123 };

        // Act
        await _service.SaveAsync(nestedFolder, fileName, content);

        // Assert
        Directory.Exists(nestedFolder).Should().BeTrue();
        var filePath = Path.Combine(nestedFolder, fileName);
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WithNullObject_ShouldSaveNullOrEmptyJson()
    {
        // Arrange
        var fileName = "null.json";
        TestData? content = null;

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();
        var fileContent = await File.ReadAllTextAsync(filePath);
        fileContent.Trim().Should().Be("null");
    }

    [Fact]
    public async Task SaveAsync_WithEmptyString_ShouldSaveEmptyString()
    {
        // Arrange
        var fileName = "emptystring.json";
        var content = string.Empty;

        // Act
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();

        var result = await _service.ReadAsync<string>(_testFolder, fileName);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_WithSpecialCharactersInFolderPath_ShouldCreateFolder()
    {
        // Arrange
        var specialFolder = Path.Combine(_testFolder, "folder with spaces", "folder-with-dash");
        var fileName = "test.json";
        var content = new TestData { Name = "Special Path", Value = 999 };

        // Act
        await _service.SaveAsync(specialFolder, fileName, content);

        // Assert
        Directory.Exists(specialFolder).Should().BeTrue();
        var filePath = Path.Combine(specialFolder, fileName);
        File.Exists(filePath).Should().BeTrue();
    }

    [Fact]
    public async Task SaveAsync_WithValueTypes_ShouldSaveAndReadCorrectly()
    {
        // Arrange
        var fileName = "valueTypes.json";
        var intValue = 42;

        // Act
        await _service.SaveAsync(_testFolder, fileName, intValue);
        var result = await _service.ReadAsync<int>(_testFolder, fileName);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public async Task SaveAsync_WhenFolderAlreadyExists_ShouldNotRecreateFolder()
    {
        // Arrange
        Directory.CreateDirectory(_testFolder);
        var fileName = "existing_folder.json";
        var content = new TestData { Name = "Test", Value = 123 };
        var beforeTime = Directory.GetCreationTime(_testFolder);

        // Act
        await Task.Delay(100); // Small delay to ensure time difference if folder is recreated
        await _service.SaveAsync(_testFolder, fileName, content);

        // Assert
        var afterTime = Directory.GetCreationTime(_testFolder);
        afterTime.Should().Be(beforeTime); // Folder creation time should not change
        File.Exists(Path.Combine(_testFolder, fileName)).Should().BeTrue();
    }

    #endregion

    #region ReadAsync Tests

    [Fact]
    public async Task ReadAsync_WhenFileExists_ShouldReturnDeserializedObject()
    {
        // Arrange
        var fileName = "read.json";
        var content = new TestData { Name = "Read Test", Value = 789 };
        await _service.SaveAsync(_testFolder, fileName, content);

        // Act
        var result = await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Read Test");
        result.Value.Should().Be(789);
    }

    [Fact]
    public async Task ReadAsync_WhenFileDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var fileName = "nonexistent.json";

        // Act
        var result = await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_WithComplexObject_ShouldDeserializeCorrectly()
    {
        // Arrange
        var fileName = "complex_read.json";
        var content = new ComplexTestData
        {
            Id = Guid.NewGuid(),
            Name = "Complex Read",
            Value = 999,
            Date = new DateTime(2024, 1, 15, 10, 30, 0),
            Items = new List<string> { "A", "B", "C" },
            Metadata = new Dictionary<string, string>
            {
                { "Author", "Test" },
                { "Version", "1.0" }
            }
        };
        await _service.SaveAsync(_testFolder, fileName, content);

        // Act
        var result = await _service.ReadAsync<ComplexTestData>(_testFolder, fileName);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(content.Id);
        result.Name.Should().Be("Complex Read");
        result.Value.Should().Be(999);
        result.Items.Should().HaveCount(3);
        result.Items.Should().Contain(new[] { "A", "B", "C" });
        result.Metadata.Should().HaveCount(2);
        result.Metadata["Author"].Should().Be("Test");
    }

    [Fact]
    public async Task ReadAsync_SaveAndRead_ShouldRoundTripCorrectly()
    {
        // Arrange
        var fileName = "roundtrip.json";
        var original = new ComplexTestData
        {
            Id = Guid.NewGuid(),
            Name = "RoundTrip Test",
            Value = 555,
            Date = DateTime.Now,
            Items = new List<string> { "X", "Y", "Z" },
            Metadata = new Dictionary<string, string>
            {
                { "Key", "Value" }
            }
        };

        // Act
        await _service.SaveAsync(_testFolder, fileName, original);
        var result = await _service.ReadAsync<ComplexTestData>(_testFolder, fileName);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(original.Id);
        result.Name.Should().Be(original.Name);
        result.Value.Should().Be(original.Value);
        result.Items.Should().BeEquivalentTo(original.Items);
        result.Metadata.Should().BeEquivalentTo(original.Metadata);
    }

    [Fact]
    public async Task ReadAsync_WithEmptyFolder_ShouldReturnDefault()
    {
        // Arrange
        Directory.CreateDirectory(_testFolder);
        var fileName = "missing.json";

        // Act
        var result = await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_WhenFolderDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var nonExistentFolder = Path.Combine(_testFolder, "nonexistent", "folder");
        var fileName = "test.json";

        // Act
        var result = await _service.ReadAsync<TestData>(nonExistentFolder, fileName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ReadAsync_WithEmptyFile_ShouldThrowException()
    {
        // Arrange
        var fileName = "empty.json";
        var filePath = Path.Combine(_testFolder, fileName);
        Directory.CreateDirectory(_testFolder);
        await File.WriteAllTextAsync(filePath, string.Empty);

        // Act
        Func<Task> act = async () => await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ReadAsync_WithInvalidJson_ShouldThrowException()
    {
        // Arrange
        var fileName = "invalid.json";
        var filePath = Path.Combine(_testFolder, fileName);
        Directory.CreateDirectory(_testFolder);
        await File.WriteAllTextAsync(filePath, "{ invalid json content }");

        // Act
        Func<Task> act = async () => await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task ReadAsync_WithValueType_WhenFileNotExists_ShouldReturnDefault()
    {
        // Arrange
        var fileName = "valueType.json";

        // Act
        var result = await _service.ReadAsync<int>(_testFolder, fileName);

        // Assert
        result.Should().Be(0); // default(int) is 0
    }

    [Fact]
    public async Task ReadAsync_WithValueType_ShouldReturnCorrectValue()
    {
        // Arrange
        var fileName = "number.json";
        var value = 42;
        await _service.SaveAsync(_testFolder, fileName, value);

        // Act
        var result = await _service.ReadAsync<int>(_testFolder, fileName);

        // Assert
        result.Should().Be(42);
    }

    [Fact]
    public async Task ReadAsync_WithNullableType_ShouldHandleCorrectly()
    {
        // Arrange
        var fileName = "nullable.json";
        int? value = 100;
        await _service.SaveAsync(_testFolder, fileName, value);

        // Act
        var result = await _service.ReadAsync<int?>(_testFolder, fileName);

        // Assert
        result.Should().Be(100);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenFileExists_ShouldDeleteFile()
    {
        // Arrange
        var fileName = "delete.json";
        var content = new TestData { Name = "To Delete", Value = 123 };
        await _service.SaveAsync(_testFolder, fileName, content);
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();

        // Act
        _service.Delete(_testFolder, fileName);

        // Assert
        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public void Delete_WhenFileDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        var fileName = "nonexistent.json";

        // Act
        var act = () => _service.Delete(_testFolder, fileName);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Delete_WithNullFileName_ShouldNotThrow()
    {
        // Act
        var act = () => _service.Delete(_testFolder, null!);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Delete_AfterDeletion_ReadShouldReturnDefault()
    {
        // Arrange
        var fileName = "delete_read.json";
        var content = new TestData { Name = "Delete Test", Value = 456 };
        await _service.SaveAsync(_testFolder, fileName, content);

        // Act
        _service.Delete(_testFolder, fileName);
        var result = await _service.ReadAsync<TestData>(_testFolder, fileName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Delete_WithNonExistentFolder_ShouldNotThrow()
    {
        // Arrange
        var nonExistentFolder = Path.Combine(_testFolder, "nonexistent");
        var fileName = "test.json";

        // Act
        var act = () => _service.Delete(nonExistentFolder, fileName);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Delete_WithNestedFolderPath_ShouldDeleteFileCorrectly()
    {
        // Arrange
        var nestedFolder = Path.Combine(_testFolder, "nested", "path", "test");
        var fileName = "delete.json";
        var content = new TestData { Name = "Nested Delete", Value = 456 };
        await _service.SaveAsync(nestedFolder, fileName, content);
        var filePath = Path.Combine(nestedFolder, fileName);
        File.Exists(filePath).Should().BeTrue();

        // Act
        _service.Delete(nestedFolder, fileName);

        // Assert
        File.Exists(filePath).Should().BeFalse();
        Directory.Exists(nestedFolder).Should().BeTrue(); // Folder should still exist
    }

    [Fact]
    public async Task Delete_WithEmptyFileName_ShouldNotThrow()
    {
        // Arrange
        var content = new TestData { Name = "Test", Value = 123 };
        await _service.SaveAsync(_testFolder, "test.json", content);

        // Act
        var act = () => _service.Delete(_testFolder, string.Empty);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public async Task Delete_MultipleFilesInSameFolder_ShouldOnlyDeleteSpecifiedFile()
    {
        // Arrange
        var file1 = "file1.json";
        var file2 = "file2.json";
        var file3 = "file3.json";
        await _service.SaveAsync(_testFolder, file1, new TestData { Name = "File1", Value = 1 });
        await _service.SaveAsync(_testFolder, file2, new TestData { Name = "File2", Value = 2 });
        await _service.SaveAsync(_testFolder, file3, new TestData { Name = "File3", Value = 3 });

        // Act
        _service.Delete(_testFolder, file2);

        // Assert
        File.Exists(Path.Combine(_testFolder, file1)).Should().BeTrue();
        File.Exists(Path.Combine(_testFolder, file2)).Should().BeFalse();
        File.Exists(Path.Combine(_testFolder, file3)).Should().BeTrue();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task IntegrationTest_SaveReadDelete_ShouldWorkCorrectly()
    {
        // Arrange
        var fileName = "integration.json";
        var content = new TestData { Name = "Integration", Value = 999 };

        // Act & Assert - Save
        await _service.SaveAsync(_testFolder, fileName, content);
        var filePath = Path.Combine(_testFolder, fileName);
        File.Exists(filePath).Should().BeTrue();

        // Act & Assert - Read
        var readResult = await _service.ReadAsync<TestData>(_testFolder, fileName);
        readResult.Should().NotBeNull();
        readResult.Name.Should().Be("Integration");
        readResult.Value.Should().Be(999);

        // Act & Assert - Delete
        _service.Delete(_testFolder, fileName);
        File.Exists(filePath).Should().BeFalse();

        // Act & Assert - Read after delete
        var afterDeleteResult = await _service.ReadAsync<TestData>(_testFolder, fileName);
        afterDeleteResult.Should().BeNull();
    }

    [Fact]
    public async Task IntegrationTest_MultipleFiles_ShouldHandleIndependently()
    {
        // Arrange
        var file1 = "file1.json";
        var file2 = "file2.json";
        var file3 = "file3.json";
        var content1 = new TestData { Name = "File1", Value = 100 };
        var content2 = new TestData { Name = "File2", Value = 200 };
        var content3 = new TestData { Name = "File3", Value = 300 };

        // Act
        await _service.SaveAsync(_testFolder, file1, content1);
        await _service.SaveAsync(_testFolder, file2, content2);
        await _service.SaveAsync(_testFolder, file3, content3);

        // Assert - All files exist
        File.Exists(Path.Combine(_testFolder, file1)).Should().BeTrue();
        File.Exists(Path.Combine(_testFolder, file2)).Should().BeTrue();
        File.Exists(Path.Combine(_testFolder, file3)).Should().BeTrue();

        // Act - Delete middle file
        _service.Delete(_testFolder, file2);

        // Assert - Other files remain
        File.Exists(Path.Combine(_testFolder, file1)).Should().BeTrue();
        File.Exists(Path.Combine(_testFolder, file2)).Should().BeFalse();
        File.Exists(Path.Combine(_testFolder, file3)).Should().BeTrue();

        // Act - Read remaining files
        var result1 = await _service.ReadAsync<TestData>(_testFolder, file1);
        var result3 = await _service.ReadAsync<TestData>(_testFolder, file3);

        // Assert - Data is correct
        result1.Value.Should().Be(100);
        result3.Value.Should().Be(300);
    }

    #endregion

    #region Test Data Classes

    private class TestData
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private class ComplexTestData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public DateTime Date { get; set; }
        public List<string> Items { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    #endregion
}

