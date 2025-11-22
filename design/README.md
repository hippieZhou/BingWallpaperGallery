# 应用图标设计文档

本文档描述了 BinggoWallpapers.WinUI 应用图标的生成过程和管理方法。

## 📁 文件结构

```
design/
├── app_icon.svg          # 原始 SVG 矢量图标文件
├── app_icon.png          # 256x256 像素 PNG 图标
├── app_icon_64.png       # 64x64 像素 PNG 图标
├── app_icon_128.png      # 128x128 像素 PNG 图标
├── app_icon_512.png      # 512x512 像素 PNG 图标
└── README.md             # 本文档
```

## 🎨 图标设计理念

### Fluent Design 2 风格
- **透明背景**：支持各种主题和背景色
- **几何简洁**：清晰的几何形状和简洁的线条
- **材质感**：半透明元素和柔和的光影效果
- **色彩和谐**：使用微软品牌蓝色作为主色调

### 设计元素
- **相框设计**：主框架模拟显示器/相框，体现壁纸展示功能
- **多彩渐变**：图像区域使用丰富的渐变色彩，象征壁纸的多样性
- **装饰图形**：几何图形代表不同风格的壁纸内容
- **高光效果**：增加视觉层次和现代感

## 🛠️ 生成工具

### 必需工具
- **Sharp-CLI**：高性能图像处理命令行工具
- **Node.js**：JavaScript 运行时环境

### 安装 Sharp-CLI
```bash
npm install -g sharp-cli
```

## 📋 生成步骤

### 1. 从 SVG 生成基础 PNG 文件

```bash
# 生成 256x256 标准尺寸
sharp -i app_icon.svg -o app_icon.png --format png resize 256 256

# 生成 64x64 小图标
sharp -i app_icon.svg -o app_icon_64.png --format png resize 64 64

# 生成 128x128 中等图标
sharp -i app_icon.svg -o app_icon_128.png --format png resize 128 128

# 生成 512x512 高分辨率图标
sharp -i app_icon.svg -o app_icon_512.png --format png resize 512 512
```

### 2. 批量生成所有尺寸

```bash
# 使用循环批量生成
for size in 64 128 256 512; do
  sharp -i app_icon.svg -o "app_icon_${size}.png" --format png resize $size $size
done
```

### 3. 生成带阴影效果的版本

```bash
# 生成带白色背景和边距的版本（适合应用商店）
sharp -i app_icon.svg -o app_icon_512_shadow.png --format png resize 512 512 -- extend 40 40 40 40 -- flatten "#ffffff"
```

## 🎯 不同尺寸的用途

| 尺寸 | 文件名 | 用途 | 说明 |
|------|--------|------|------|
| 64x64 | `app_icon_64.png` | 小图标 | 任务栏、通知区域、小尺寸显示 |
| 128x128 | `app_icon_128.png` | 中等图标 | 开始菜单、桌面快捷方式、中等尺寸显示 |
| 256x256 | `app_icon.png` | 标准图标 | 应用列表、设置页面、标准显示 |
| 512x512 | `app_icon_512.png` | 高分辨率图标 | 应用商店、大尺寸显示、高 DPI 屏幕 |

## 🔧 高级处理选项

### 质量优化
```bash
# 高质量 PNG（更大文件，更好质量）
sharp -i app_icon.svg -o app_icon_hq.png --format png --compressionLevel 9 resize 256 256

# 优化文件大小
sharp -i app_icon.png -o app_icon_optimized.png --format png --effort 9
```

### 格式转换
```bash
# 转换为 WebP 格式（更小文件）
sharp -i app_icon.png -o app_icon.webp --format webp --quality 90

# 转换为 JPEG 格式
sharp -i app_icon.png -o app_icon.jpg --format jpeg --quality 95
```

### 特殊效果
```bash
# 生成灰度版本
sharp -i app_icon.png -o app_icon_gray.png -- greyscale

# 添加圆角效果（需要遮罩文件）
sharp -i app_icon.png -o app_icon_rounded.png -- composite rounded_mask.png
```

## 📱 在 WinUI 3 应用中使用

### 1. 复制文件到 Assets 文件夹
```bash
# 复制到应用资源文件夹
cp design/app_icon*.png BinggoWallpapers/Assets/
```

### 2. 更新 Package.appxmanifest
在 `Package.appxmanifest` 文件中引用图标：

```xml
<Package>
  <Applications>
    <Application>
      <VisualElements>
        <!-- 应用图标 -->
        <Square150x150Logo>Assets\app_icon_128.png</Square150x150Logo>
        <Square44x44Logo>Assets\app_icon_64.png</Square44x44Logo>
        <!-- 其他尺寸... -->
      </VisualElements>
    </Application>
  </Applications>
</Package>
```

### 3. 支持的图标尺寸
- **Square44x44Logo**: 44x44 像素（任务栏）
- **Square150x150Logo**: 150x150 像素（开始菜单）
- **Wide310x150Logo**: 310x150 像素（宽磁贴）
- **Square310x310Logo**: 310x310 像素（大磁贴）

## 🎨 设计规范

### 颜色规范
- **主色调**：微软品牌蓝色 (#0078D4)
- **渐变色彩**：橙红到青绿的丰富渐变
- **装饰色**：白色半透明元素
- **背景**：透明或白色

### 尺寸规范
- **最小尺寸**：16x16 像素（确保可识别性）
- **推荐尺寸**：64x64 到 512x512 像素
- **最大尺寸**：1024x1024 像素（应用商店）

### 设计原则
- **简洁明了**：避免过多细节
- **高对比度**：确保在各种背景下清晰可见
- **一致性**：保持品牌形象统一
- **可扩展性**：矢量设计支持任意缩放

## 🔄 更新流程

### 修改图标设计
1. 编辑 `app_icon.svg` 文件
2. 使用上述命令重新生成所有 PNG 文件
3. 测试在不同尺寸下的显示效果
4. 更新应用中的图标引用

### 版本控制
- SVG 文件作为主设计文件，应优先保存
- PNG 文件可以重新生成，不需要版本控制
- 记录重要的设计变更和生成参数

## 📚 相关资源

- [Sharp-CLI 官方文档](https://sharp.pixelplumbing.com/)
- [Fluent Design 2 设计指南](https://docs.microsoft.com/en-us/windows/uwp/design/fluent-design-system/)
- [WinUI 3 应用图标指南](https://docs.microsoft.com/en-us/windows/apps/design/style/app-icons-and-logos/)
- [SVG 规范](https://www.w3.org/TR/SVG2/)

## 🤝 贡献指南

如果您需要修改图标设计：

1. **设计修改**：编辑 `app_icon.svg` 文件
2. **重新生成**：使用上述命令生成新的 PNG 文件
3. **测试验证**：确保在不同尺寸下显示正常
4. **文档更新**：更新本 README 文档

---

**注意**：本图标设计遵循 Fluent Design 2 规范，专为 BinggoWallpapers 应用定制。如需用于其他项目，请确保符合相应的设计规范。
