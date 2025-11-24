# Serilog 日志系统配置说明

## 📋 概述

BinggoWallpapers 使用 Serilog 作为日志框架，集成了 Microsoft.Extensions.Logging，支持灵活的配置和多种输出格式。

## 🏗️ 架构说明

### 核心组件

-   **`SerilogConfigurationHelper`**（`Helpers/SerilogConfigurationHelper.cs`）

    -   静态辅助类，负责 Serilog 的初始化和配置
    -   在应用启动时通过 `Bootstrapper` 调用

-   **`LogSettingService`**（`Services/Impl/LogSettingService.cs`）

    -   DI 服务，提供日志管理功能
    -   支持清理旧日志、计算日志文件夹大小

-   **`LoggingOptions`**（`Models/LoggingOptions.cs`）
    -   日志配置选项类
    -   从 `appsettings.json` 读取配置

### 日志文件结构

日志文件按日期组织，每天创建一个文件夹（格式：`yyyyMMdd`）：

```
Logs/
├── 20251005/
│   ├── app.log              # 主日志文件（所有级别）
│   ├── error.log            # 错误日志（Error 和 Fatal）
│   └── structured.json      # 结构化日志（可选）
├── 20251006/
│   ├── app.log
│   ├── error.log
│   └── structured.json
└── ...
```

## ⚙️ 配置说明

### appsettings.json 配置

在 `appsettings.json` 中配置日志选项：

```json
{
    "LoggingOptions": {
        "RetainedDays": 30,
        "FileSizeLimitBytes": 10485760,
        "RetainedFileCountLimit": 5,
        "MinimumLevel": "Verbose",
        "EnableStructuredLogging": true,
        "EnableDebugOutput": true
    }
}
```

### 配置参数说明

| 参数                      | 类型   | 默认值          | 说明                                                                         |
| ------------------------- | ------ | --------------- | ---------------------------------------------------------------------------- |
| `RetainedDays`            | int    | 30              | 日志保留天数，超过此天数的日志文件夹会被自动清理                             |
| `FileSizeLimitBytes`      | long   | 10485760 (10MB) | 单个日志文件的最大大小（字节）                                               |
| `RetainedFileCountLimit`  | int    | 5               | 当日志文件超过大小限制时，保留的滚动文件数量                                 |
| `MinimumLevel`            | string | "Verbose"       | 最小日志级别：`Verbose`、`Debug`、`Information`、`Warning`、`Error`、`Fatal` |
| `EnableStructuredLogging` | bool   | true            | 是否启用结构化日志（JSON 格式）                                              |
| `EnableDebugOutput`       | bool   | true            | 是否启用调试输出（输出到 Debug 窗口）                                        |

### 日志级别说明

| 级别            | 用途                               | 建议使用场景           |
| --------------- | ---------------------------------- | ---------------------- |
| **Verbose**     | 最详细的日志，包含所有信息         | 开发调试、深度排查问题 |
| **Debug**       | 调试信息                           | 开发环境、问题诊断     |
| **Information** | 一般信息，记录应用的正常行为       | 生产环境推荐           |
| **Warning**     | 警告信息，可能存在问题但不影响运行 | 生产环境（精简模式）   |
| **Error**       | 错误信息，功能执行失败             | 仅记录错误             |
| **Fatal**       | 致命错误，应用即将崩溃             | 仅记录严重错误         |

## 🎯 使用场景配置

### 开发环境配置

```json
{
    "LoggingOptions": {
        "RetainedDays": 7,
        "FileSizeLimitBytes": 10485760,
        "RetainedFileCountLimit": 3,
        "MinimumLevel": "Verbose",
        "EnableStructuredLogging": false,
        "EnableDebugOutput": true
    }
}
```

**特点：**

-   记录所有级别的日志，便于调试
-   保留时间较短（7 天），节省磁盘空间
-   启用调试输出，可在 IDE 中实时查看日志
-   关闭结构化日志，减少不必要的文件

### 生产环境配置

```json
{
    "LoggingOptions": {
        "RetainedDays": 30,
        "FileSizeLimitBytes": 20971520,
        "RetainedFileCountLimit": 10,
        "MinimumLevel": "Information",
        "EnableStructuredLogging": true,
        "EnableDebugOutput": false
    }
}
```

**特点：**

-   只记录 Information 及以上级别，减少日志量
-   保留时间较长（30 天），便于问题追溯
-   启用结构化日志，便于日志分析和监控
-   关闭调试输出，减少性能开销
-   更大的文件大小限制和滚动文件数量

### 测试环境配置

```json
{
    "LoggingOptions": {
        "RetainedDays": 14,
        "FileSizeLimitBytes": 10485760,
        "RetainedFileCountLimit": 5,
        "MinimumLevel": "Debug",
        "EnableStructuredLogging": true,
        "EnableDebugOutput": false
    }
}
```

**特点：**

-   记录 Debug 及以上级别，平衡详细度和性能
-   中等保留时间（14 天）
-   启用结构化日志，便于自动化测试分析

### 性能敏感场景配置

```json
{
    "LoggingOptions": {
        "RetainedDays": 7,
        "FileSizeLimitBytes": 5242880,
        "RetainedFileCountLimit": 3,
        "MinimumLevel": "Warning",
        "EnableStructuredLogging": false,
        "EnableDebugOutput": false
    }
}
```

**特点：**

-   只记录警告和错误，最小化性能影响
-   较小的文件大小限制
-   关闭所有可选功能

## 📝 日志文件说明

### app.log

**用途：** 主日志文件，包含所有配置级别及以上的日志

**格式：**

```
{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}
```

**示例：**

```
2025-10-05 10:30:45.123 [INF] [BinggoWallpapers.App] The app has been launched successfully.
2025-10-05 10:30:46.456 [DBG] [BinggoWallpapers.ViewModels.HomeViewModel] 开始加载壁纸列表
2025-10-05 10:30:47.789 [WRN] [BinggoWallpapers.Services.Impl.MessageService] 发送警告消息: 网络连接不稳定
2025-10-05 10:30:48.012 [ERR] [BinggoWallpapers.Services.Impl.ImageExportService] Export failed: Access denied
System.UnauthorizedAccessException: Access to the path is denied.
   at System.IO.FileStream..ctor(String path, FileMode mode, FileAccess access)
```

**适用场景：**

-   日常开发和调试
-   问题排查和行为追踪
-   了解应用完整运行流程

### error.log

**用途：** 仅包含错误和致命错误（Error 和 Fatal 级别）

**格式：** 与 `app.log` 相同

**示例：**

```
2025-10-05 10:35:12.789 [ERR] [BinggoWallpapers.Services.Impl.ImageExportService] Export failed: Access denied
System.UnauthorizedAccessException: Access to the path is denied.
2025-10-05 10:36:15.456 [FTL] [BinggoWallpapers.App] Application crashed
System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown.
```

**适用场景：**

-   快速定位生产环境问题
-   错误监控和告警
-   生成错误报告

### structured.json

**用途：** 结构化日志（JSON 格式），便于机器解析和分析

**格式：** Compact JSON（每行一个 JSON 对象）

**示例：**

```json
{"@t":"2025-10-05T02:30:45.123Z","@mt":"The app has been launched successfully.","@l":"Information","Application":"BinggoWallpapers","SourceContext":"BinggoWallpapers.App"}
{"@t":"2025-10-05T02:30:46.456Z","@mt":"开始加载壁纸列表","@l":"Debug","Application":"BinggoWallpapers","SourceContext":"BinggoWallpapers.ViewModels.HomeViewModel"}
{"@t":"2025-10-05T02:30:48.012Z","@mt":"Export failed: {ErrorMessage}","@l":"Error","ErrorMessage":"Access denied","Application":"BinggoWallpapers","SourceContext":"BinggoWallpapers.Services.Impl.ImageExportService","@x":"System.UnauthorizedAccessException: Access to the path is denied."}
```

**适用场景：**

-   日志聚合和分析（如 Seq、Elasticsearch、Splunk）
-   监控和告警系统集成
-   自动化日志分析和报表生成

## 💻 代码使用示例

### 在类中使用日志

```csharp
using Microsoft.Extensions.Logging;

public class MyViewModel
{
    private readonly ILogger<MyViewModel> _logger;

    public MyViewModel(ILogger<MyViewModel> logger)
    {
        _logger = logger;
    }

    public void DoSomething()
    {
        _logger.LogInformation("开始执行操作");

        try
        {
            // 业务逻辑
            _logger.LogDebug("处理中: {ItemCount} 个项目", itemCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "操作失败: {ErrorMessage}", ex.Message);
        }

        _logger.LogInformation("操作完成");
    }
}
```

### 日志级别使用建议

```csharp
// Verbose - 非常详细的调试信息
_logger.LogTrace("进入方法: {MethodName}, 参数: {Parameters}", methodName, parameters);

// Debug - 调试信息
_logger.LogDebug("缓存命中: {CacheKey}", cacheKey);

// Information - 一般信息
_logger.LogInformation("用户登录成功: {UserName}", userName);

// Warning - 警告
_logger.LogWarning("API 调用超时，使用缓存数据");

// Error - 错误
_logger.LogError(ex, "保存文件失败: {FilePath}", filePath);

// Fatal - 致命错误
_logger.LogCritical(ex, "数据库连接失败，应用即将退出");
```

## 🛠️ 日志管理

### 手动清理旧日志

通过 `ILogSettingService` 服务：

```csharp
var logSettingService = App.GetService<ILogSettingService>();
logSettingService.CleanUpOldLogs();
```

### 获取日志文件夹大小

```csharp
var logSettingService = App.GetService<ILogSettingService>();
long sizeInBytes = logSettingService.FolderSizeInBytes;
double sizeInMB = sizeInBytes / 1024.0 / 1024.0;
```

### 日志自动清理

应用启动时会自动清理超过 `RetainedDays` 天数的日志文件夹。

## ⚠️ 注意事项

1. **日志路径**：日志默认存储在 `ApplicationData.Current.LocalCacheFolder.Path/Log`
2. **文件滚动**：当日志文件超过 `FileSizeLimitBytes` 时，会自动创建新文件（如 `app001.log`、`app002.log`）
3. **性能影响**：日志级别越低（如 Verbose），性能开销越大，建议生产环境使用 Information 或更高级别
4. **磁盘空间**：定期监控日志文件夹大小，避免占用过多磁盘空间
5. **敏感信息**：避免在日志中记录密码、Token 等敏感信息
6. **时区**：日志时间戳使用本地时间，结构化日志使用 UTC 时间

## 🔍 故障排查

### 日志文件未生成

1. 检查应用是否有写入权限
2. 检查日志目录路径是否正确
3. 查看调试输出窗口是否有 Serilog 错误信息

### 日志文件过大

1. 降低日志级别（如从 Verbose 改为 Information）
2. 减小 `FileSizeLimitBytes` 和 `RetainedFileCountLimit`
3. 减少 `RetainedDays`，更频繁地清理旧日志

### 找不到特定日志

1. 确认日志级别配置是否正确
2. 检查是否被第三方库日志过滤规则影响
3. 查看 `error.log` 是否有相关错误信息

## 📚 参考资料

-   [Serilog 官方文档](https://serilog.net/)
-   [Serilog.Extensions.Hosting](https://github.com/serilog/serilog-extensions-hosting)
-   [Serilog 最佳实践](https://github.com/serilog/serilog/wiki/Configuration-Basics)
-   [.NET Logging 指南](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
