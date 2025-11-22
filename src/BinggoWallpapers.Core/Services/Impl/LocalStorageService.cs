// Copyright (c) hippieZhou. All rights reserved.

using System.Text;
using BinggoWallpapers.Core.Helpers;

namespace BinggoWallpapers.Core.Services.Impl;

public class LocalStorageService : ILocalStorageService
{
    public async Task<T> ReadAsync<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return await Json.ToObjectAsync<T>(json);
        }

        return default;
    }

    public async Task SaveAsync<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = await Json.StringifyAsync(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }
}
