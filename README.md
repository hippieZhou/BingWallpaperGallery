<div align="center">

# BinggoWallpapers

  <img src="design/app_icon.svg" alt="BinggoWallpapers" width="128" height="128">

[![Microsoft Store](https://img.shields.io/badge/Microsoft_Store-Download-blue?style=for-the-badge&logo=microsoft)](https://apps.microsoft.com/store/detail/bing-wallpaper-gallery/9NBLGGH5X8FV)
[![GitHub Release](https://img.shields.io/badge/GitHub-Download-black?style=for-the-badge&logo=github)](https://github.com/hippiezhou/BinggoWallpapers/releases)

</div>

> 一个基于 WinUI 3 的必应壁纸应用，让您轻松浏览、下载和设置来自世界各地的精美壁纸。

[![Build and Test](https://github.com/hippiezhou/BinggoWallpapers/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/hippiezhou/BinggoWallpapers/actions/workflows/build-and-test.yml)
[![codecov](https://codecov.io/gh/hippiezhou/BinggoWallpapers/branch/main/graph/badge.svg?token=SX3PU5ZP2I)](https://codecov.io/gh/hippiezhou/BinggoWallpapers)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![WinUI 3](https://img.shields.io/badge/WinUI-3.0-blue)
![.NET 9](https://img.shields.io/badge/.NET-9.0-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## ✨ 功能特性

### 🌍 多地区支持

-   **14 个地区**：中国 🇨🇳、美国 🇺🇸、英国 🇬🇧、日本 🇯🇵、德国 🇩🇪、法国 🇫🇷、西班牙 🇪🇸、意大利 🇮🇹、俄罗斯 🇷🇺、韩国 🇰🇷、巴西 🇧🇷、澳大利亚 🇦🇺、加拿大 🇨🇦、印度 🇮🇳
-   **本地化内容**：每个地区提供独特的壁纸内容，反映当地文化和风景
-   **一键切换**：轻松在不同地区间切换，发现更多精彩壁纸
-   **智能语言适配**：自动根据地区设置对应的语言标头
-   **并发收集**：支持多地区并发数据收集，提升效率

### 🖼️ 壁纸管理

-   **实时浏览**：查看最新的必应每日壁纸
-   **历史归档**：浏览历史壁纸，发现经典作品，支持增量加载
-   **多分辨率下载**：支持 4 种分辨率（1366x768、1920x1080、1920x1200、3840x2160 4K）
-   **本地数据库**：SQLite 存储壁纸信息，支持离线浏览
-   **增量加载**：使用 IncrementalLoadingCollection 实现流畅的无限滚动

### 🎨 图像编辑与特效

-   **实时特效预览**：7 种图像特效（曝光、色温、色调、模糊、对比度、饱和度、像素化）
-   **Win2D 图形引擎**：基于 Microsoft.Graphics.Win2D 的高性能图像处理
-   **Mockup 预览**：在设备模型中预览壁纸效果
-   **高质量导出**：支持 2x 高分辨率导出为 PNG/JPEG 格式
-   **实时渲染**：CanvasControl 实现流畅的实时特效预览

### 💎 用户体验

-   **现代化界面**：基于 WinUI 3 的 Fluent Design 设计语言
-   **响应式布局**：适配不同屏幕尺寸，优雅的动画过渡
-   **加载动画**：Shimmer 加载效果、优雅的加载状态提示
-   **错误处理**：友好的错误提示和重试机制，完整的日志记录
-   **应用内通知**：StackedNotificationsBehavior 实现优雅的消息提示
-   **系统通知**：支持 Windows 系统级通知

## 🚀 快速开始

查看 [快速开始指南](docs/QuickStart.md) 了解详细的技术栈、系统要求、开发环境搭建、配置和运行应用，以及数据库管理相关操作。

## 🔄 持续集成

本项目使用 GitHub Actions 进行持续集成和自动化。查看 [GitHub Actions 说明文档](.github/ACTIONS.md) 了解详细的工作流配置和使用方式。

-   **构建状态**：点击顶部的 "Build and Test" 徽章查看最新构建状态
-   **测试覆盖率**：点击 "codecov" 徽章查看详细的代码覆盖率报告
-   **工作流历史**：访问 [Actions](https://github.com/hippiezhou/BinggoWallpapers/actions) 页面查看所有构建历史

## 🤝 贡献指南

欢迎所有形式的贡献！

### 如何贡献

1. **Fork 项目**
2. **创建功能分支** (`git checkout -b feature/AmazingFeature`)
3. **提交更改** (`git commit -m 'Add some AmazingFeature'`)
4. **推送到分支** (`git push origin feature/AmazingFeature`)
5. **创建 Pull Request**

### 开发规范

-   **代码风格**：遵循 C# 编码规范和 .editorconfig 配置
-   **命名规范**：使用有意义的变量和函数名
-   **注释规范**：添加 XML 文档注释
-   **测试要求**：为新功能添加单元测试，确保测试覆盖率不降低
-   **提交规范**：使用清晰的提交信息（推荐使用 Conventional Commits）
-   **代码审查**：确保代码通过所有测试和 CI 检查

## 🔒 隐私策略

我们非常重视您的隐私。本应用：

-   ✅ **不收集**任何个人信息
-   ✅ **不跟踪**用户行为
-   ✅ **不上传**任何数据到服务器
-   ✅ 所有数据**仅本地存储**

查看完整的 [隐私策略](PRIVACY_POLICY.md) 或 [在线版本](https://hippiezhou.github.io/BinggoWallpapers/docs/privacy-policy.html)。

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE.txt) 文件了解详情。

## 🙏 致谢

-   [Microsoft WinUI](https://github.com/microsoft/microsoft-ui-xaml) - 现代化的 Windows UI 框架
-   [CommunityToolkit](https://github.com/CommunityToolkit) - 优秀的 WinUI 工具包
-   [Bing Wallpaper API](https://www.bing.com/HPImageArchive.aspx) - 必应壁纸数据源
-   [Microsoft.Graphics.Win2D](https://github.com/Microsoft/Win2D) - 高性能 2D 图形渲染
-   [Entity Framework Core](https://github.com/dotnet/efcore) - 数据访问 ORM 框架
-   [Serilog](https://github.com/serilog/serilog) - 结构化日志记录框架

## 📞 联系我

-   **项目主页**：[GitHub Repository](https://github.com/hippiezhou/BinggoWallpapers)
-   **问题反馈**：[Issues](https://github.com/hippiezhou/BinggoWallpapers/issues)
-   **功能建议**：[Discussions](https://github.com/hippiezhou/BinggoWallpapers/discussions)

---

⭐ 如果这个项目对您有帮助，请给我一个星标！
