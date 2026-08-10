# 快速开始指南

本文档将指导您从零开始搭建开发环境、配置和运行 BinggoWallpapers 应用。

---

## 📋 系统要求

在开始之前，请确保您的系统满足以下要求：

-   **操作系统**：Windows 10 版本 17763.0 或更高版本（推荐 Windows 11）
-   **SDK 版本**：Windows App SDK 1.8+
-   **.NET 版本**：.NET 9.0 SDK 和 Runtime
-   **架构支持**：x86、x64、ARM64
-   **内存**：建议 4GB 或更多
-   **存储空间**：至少 200MB 可用空间（含数据库和缓存）
-   **IDE**：Visual Studio 2022（17.8+）

---

## 🛠️ 技术栈

### 前端技术

-   **UI 框架**：WinUI 3 (Windows App SDK 1.8)
-   **开发语言**：C# 12 (.NET 9.0)
-   **MVVM 框架**：CommunityToolkit.Mvvm 8.4.0
-   **UI 组件库**：CommunityToolkit.WinUI 8.2+
-   **图像处理**：Microsoft.Graphics.Win2D 1.3.2
-   **图片加载**：ImageEx.WinUI 4.0.0
-   **UI 行为**：Microsoft.Xaml.Behaviors.WinUI 3.0.0

### 后端技术

-   **运行时**：.NET 9.0
-   **依赖注入**：Microsoft.Extensions.DependencyInjection
-   **HTTP 客户端**：Microsoft.Extensions.Http.Resilience 9.9.0
-   **数据库 ORM**：Entity Framework Core 9.0.9
-   **数据库**：SQLite
-   **日志框架**：Serilog 9.0.0
-   **配置管理**：Microsoft.Extensions.Configuration

### 开发和测试工具

-   **IDE**：Visual Studio 2022
-   **包管理**：NuGet
-   **版本控制**：Git
-   **测试框架**：xUnit、Moq、FluentAssertions

---

## 🚀 快速开始

### 步骤 1：环境准备

#### 安装 Visual Studio 2022

下载并安装 [Visual Studio 2022](https://visualstudio.microsoft.com/)，确保选择以下工作负载：

-   ✅ .NET 桌面开发
-   ✅ 通用 Windows 平台开发

#### 安装 Windows App SDK

-   下载并安装 [Windows App SDK](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/)

### 步骤 2：克隆项目

```bash
git clone https://github.com/hippiezhou/BinggoWallpapers.git
cd BinggoWallpapers
```

### 步骤 3：构建和运行

#### 方式一：使用命令行

```bash
# 还原 NuGet 包
dotnet restore

# 构建项目
dotnet build

# 运行应用
dotnet run --project src/BinggoWallpapers.WinUI
```

#### 方式二：使用 Visual Studio

1. 打开 `BinggoWallpapers.slnx` 解决方案文件
2. 等待 Visual Studio 自动还原 NuGet 包
3. 按 `F5` 或点击"开始调试"按钮运行应用

---

## 🔧 配置说明

### 应用配置文件

应用配置文件位于 `src/BinggoWallpapers.WinUI/appsettings.json`：

```json
{
    "LocalSettingsOptions": {
        "ApplicationDataFolder": "BinggoWallpapers/ApplicationData",
        "LocalSettingsFile": "LocalSettings.json"
    },
    "LoggingOptions": {
        "RetainedDays": 30,
        "FileSizeLimitBytes": 10485760,
        "RetainedFileCountLimit": 5,
        "MinimumLevel": "Verbose",
        "EnableStructuredLogging": true,
        "EnableDebugOutput": true
    },
    "CollectionOptions": {
        "MarketCode": "China",
        "ResolutionCode": "FullHD",
        "CollectAllCountries": true,
        "CollectDays": 8,
        "MaxConcurrentRequests": 3,
        "PrettyJsonFormat": true
    }
}
```

### 配置项说明

#### LocalSettingsOptions

-   `ApplicationDataFolder`：本地应用数据存储目录
-   `LocalSettingsFile`：本地设置文件名

#### LoggingOptions

-   `RetainedDays`：日志保留天数
-   `FileSizeLimitBytes`：单个日志文件大小限制（字节）
-   `RetainedFileCountLimit`：保留的日志文件数量
-   `MinimumLevel`：最小日志级别（Verbose/Debug/Information/Warning/Error/Fatal）

#### CollectionOptions

-   `MarketCode`：默认市场代码
-   `ResolutionCode`：默认分辨率代码
-   `CollectAllCountries`：是否收集所有国家/地区的壁纸
-   `CollectDays`：收集天数（1-8 天）
-   `MaxConcurrentRequests`：最大并发请求数

### 支持的地区

| 地区        | 代码  | 语言       | 说明                                           |
| ----------- | ----- | ---------- | ---------------------------------------------- |
| 🇨🇳 中国     | zh-CN | Chinese    | 中国版必应壁纸，侧重中国文化、风景和节庆       |
| 🇺🇸 美国     | en-US | English    | 美国版必应每日壁纸，涵盖美国本土风景和节日主题 |
| 🇬🇧 英国     | en-GB | English    | 英国版壁纸，英国文化与风光的精选集合           |
| 🇯🇵 日本     | ja-JP | Japanese   | 日本地区壁纸，包含日本名胜、季节性风景         |
| 🇩🇪 德国     | de-DE | German     | 德国版壁纸，包含德国城市景观和自然风光         |
| 🇫🇷 法国     | fr-FR | French     | 法国版壁纸，突出法国历史遗迹及浪漫风情         |
| 🇪🇸 西班牙   | es-ES | Spanish    | 西班牙版壁纸，展现热情的西班牙风情及历史遗址   |
| 🇮🇹 意大利   | it-IT | Italian    | 意大利版壁纸，汇集意大利艺术古迹和风景         |
| 🇷🇺 俄罗斯   | ru-RU | Russian    | 俄罗斯版壁纸，展现俄罗斯广袤的自然风光         |
| 🇰🇷 韩国     | ko-KR | Korean     | 韩国版壁纸，包含韩国现代与传统文化的融合       |
| 🇧🇷 巴西     | pt-BR | Portuguese | 巴西版壁纸，展示热带风光和节日庆典             |
| 🇦🇺 澳大利亚 | en-AU | English    | 澳大利亚版壁纸，展现澳洲独特的自然景观         |
| 🇨🇦 加拿大   | en-CA | English    | 加拿大版壁纸，展现枫叶国自然美景和多元文化     |
| 🇮🇳 印度     | en-IN | English    | 印度地区壁纸，反映印度地域特色与文化           |

### 支持的分辨率

| 名称     | 分辨率    | 文件后缀        | 适用场景        |
| -------- | --------- | --------------- | --------------- |
| Standard | 1366×768  | \_1366x768.jpg  | 标准笔记本屏幕  |
| Full HD  | 1920×1080 | \_1920x1080.jpg | 全高清显示器    |
| HD       | 1920×1200 | \_1920x1200.jpg | 16:10 显示器    |
| UHD 4K   | 3840×2160 | \_UHD.jpg       | 4K 超高清显示器 |

---

## 💾 数据库管理

项目使用 **Entity Framework Core** 和 **SQLite** 进行数据持久化。

### 数据库架构

-   **表结构**：`gallery.wallpapers` 表存储壁纸信息
-   **索引优化**：复合索引 `(MarketCode, ActualDate)` 和单独索引优化查询性能
-   **JSON 存储**：壁纸详细信息以 JSON 格式存储在 `InfoJson` 字段中
-   **审计字段**：包含创建时间、修改时间等审计信息

### 自动迁移

应用在启动时会自动检测并应用待处理的数据库迁移，无需手动干预。相关代码位于：

-   `BinggoWallpapers.Core/DataAccess/ApplicationDbContext.cs`
-   `BinggoWallpapers.Core/DataAccess/ApplicationDbContextInitializer.cs`

### 手动迁移（开发者）

如果你需要修改数据库结构，请按以下步骤操作。

> **前置要求**：需要安装 `dotnet-ef` 工具：`dotnet tool install --global dotnet-ef`

#### 创建新迁移

```bash
dotnet ef migrations add MigrationName \
    --project src/BinggoWallpapers.Core \
    --startup-project src/BinggoWallpapers.WinUI \
    --output-dir DataAccess/Migrations
```

#### 应用迁移

```bash
dotnet ef database update \
    --project src/BinggoWallpapers.Core \
    --startup-project src/BinggoWallpapers.WinUI
```

#### 回滚迁移

```bash
# 回滚到指定迁移
dotnet ef database update PreviousMigrationName \
    --project src/BinggoWallpapers.Core \
    --startup-project src/BinggoWallpapers.WinUI

# 回滚所有迁移（回到初始状态）
dotnet ef database update 0 \
    --project src/BinggoWallpapers.Core \
    --startup-project src/BinggoWallpapers.WinUI
```

#### 删除最后一个迁移

```bash
dotnet ef migrations remove \
    --project src/BinggoWallpapers.Core \
    --startup-project src/BinggoWallpapers.WinUI
```

### 数据库工具命令

```bash
# 查看迁移列表
dotnet ef migrations list --project src/BinggoWallpapers.Core

# 生成 SQL 脚本
dotnet ef migrations script --project src/BinggoWallpapers.Core

# 查看数据库上下文信息
dotnet ef dbcontext info --project src/BinggoWallpapers.Core
```

---

## 🧪 本地运行测试

### 运行测试

```bash
# 运行所有测试
dotnet test

# 运行测试并生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"

# 运行测试并显示详细输出
dotnet test --logger "console;verbosity=detailed"
```

### 生成覆盖率报告

```bash
# 安装 ReportGenerator 工具（仅需一次）
dotnet tool install -g dotnet-reportgenerator-globaltool

# 运行测试并收集覆盖率数据
dotnet test --collect:"XPlat Code Coverage"

# 生成 HTML 格式的覆盖率报告
reportgenerator \
    -reports:"**/coverage.cobertura.xml" \
    -targetdir:"coveragereport" \
    -reporttypes:Html

# 打开覆盖率报告（Windows）
start coveragereport/index.html
```

### 测试框架和工具

项目使用以下测试框架和工具：

-   **xUnit**：主要的单元测试框架
-   **Moq**：Mock 对象框架，用于模拟依赖项
-   **FluentAssertions**：提供流畅的断言语法
-   **Entity Framework InMemory**：内存数据库用于数据访问层测试
-   **coverlet**：代码覆盖率收集工具

---

## 📚 相关文档

-   [README](../README.md) - 项目概览和功能特性
-   [日志配置说明](LoggingConfig.md) - 详细的日志配置指南
-   [编辑器配置说明](EditorConfig.md) - 代码风格和编辑器配置
-   [GitHub Actions 说明](../.github/ACTIONS.md) - CI/CD 工作流配置

---

## ❓ 常见问题

### 构建失败

**问题**：运行 `dotnet build` 时出错

**解决方案**：

1. 确保安装了 .NET 9.0 SDK
2. 运行 `dotnet restore` 恢复 NuGet 包
3. 清理构建缓存：`dotnet clean`

### 应用无法启动

**问题**：应用启动时崩溃或无响应

**解决方案**：

1. 检查系统是否满足要求（Windows 10 17763.0+）
2. 确认安装了 Windows App SDK 1.8+
3. 查看日志文件排查问题（位于应用数据目录）

### 数据库连接错误

**问题**：提示无法连接数据库

**解决方案**：

1. 检查应用数据目录是否有写权限
2. 删除现有数据库文件，让应用重新创建
3. 确认 Entity Framework Core 包已正确安装

---

**最后更新时间**：2025-10-16
