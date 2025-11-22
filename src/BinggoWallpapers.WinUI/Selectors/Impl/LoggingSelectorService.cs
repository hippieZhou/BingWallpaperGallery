// Copyright (c) hippieZhou. All rights reserved.

using System.Globalization;
using BinggoWallpapers.WinUI.Models;
using Serilog;

namespace BinggoWallpapers.WinUI.Selectors.Impl;

/// <summary>
/// 日志管理服务，负责日志文件的清理和统计
/// </summary>
public class LoggingSelectorService : ILoggingSelectorService
{
    private readonly string _logBaseDir;

    public long FolderSizeInBytes { get; private set; }

    public LoggingSelectorService()
    {
        _logBaseDir = AppSettings.Current.DefaulttLocalLogFolder;
    }

    public Task InitializeAsync()
    {
        CalculateLogFileSizeInBytes();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 清理旧日志文件
    /// </summary>
    public void CleanUpOldLogs()
    {
        CleanUpOldLogs(_logBaseDir);
        CalculateLogFileSizeInBytes();
    }

    /// <summary>
    /// 计算日志文件夹大小
    /// </summary>
    private void CalculateLogFileSizeInBytes()
    {
        FolderSizeInBytes = GetLogDirectorySize(_logBaseDir);
    }

    /// <summary>
    /// 清理旧日志文件
    /// </summary>
    /// <param name="logBaseDir">日志基础目录</param>
    /// <param name="daysToKeep">保留天数</param>
    private void CleanUpOldLogs(string logBaseDir, int daysToKeep = 30)
    {
        if (!Directory.Exists(logBaseDir))
        {
            return;
        }

        try
        {
            var directories = Directory.GetDirectories(logBaseDir);
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var dir in directories)
            {
                var dirName = Path.GetFileName(dir);

                if (DateTime.TryParseExact(
                    dirName,
                    ["yyyyMMdd", "yyyy-MM-dd"],
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var folderDate))
                {
                    if (folderDate < cutoffDate)
                    {
                        try
                        {
                            Directory.Delete(dir, true);
                            Log.Information("已删除旧日志目录: {Directory}", dir);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning(ex, "删除日志目录失败: {Directory}", dir);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "清理旧日志时发生错误");
        }
    }

    /// <summary>
    /// 获取日志目录大小
    /// </summary>
    /// <param name="logBaseDir">日志基础目录</param>
    /// <returns>目录大小（字节）</returns>
    private static long GetLogDirectorySize(string logBaseDir)
    {
        if (!Directory.Exists(logBaseDir))
        {
            return 0;
        }

        try
        {
            var totalSize = Directory.Exists(logBaseDir)
                ? Directory.EnumerateFiles(logBaseDir, "*", SearchOption.AllDirectories).Sum(file => new FileInfo(file).Length)
                : 0;
            return totalSize;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "计算日志目录大小时发生错误");
            return 0;
        }
    }
}
