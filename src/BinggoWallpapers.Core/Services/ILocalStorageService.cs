// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Services;

public interface ILocalStorageService
{
    Task<T> ReadAsync<T>(string folderPath, string fileName);

    Task SaveAsync<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);
}
