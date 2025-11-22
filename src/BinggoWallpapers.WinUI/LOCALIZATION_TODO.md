# BinggoWallpapers 多语言支持 TODO List

> **项目目标**: 为 BinggoWallpapers.WinUI 实现完整的中文（zh-CN）和英文（en-US）多语言支持
>
> **资源文件位置**:
> - 英文: `Strings/en-US/Resources.resw`
> - 中文: `Strings/zh-CN/Resources.resw`

---

## 📊 总体进度

- **总任务数**: 16
- **已完成**: 1 (6.25%)
- **进行中**: 0 (0%)
- **待开始**: 15 (93.75%)

---

## 🏗️ 核心组件

### 1. 主窗口 (Shell)
- [ ] **ShellWindow.xaml** - 应用主窗口
  - 需要本地化的内容:
    - 窗口标题
    - 导航菜单项

---

## 📄 页面 (Views)

### 2. HomePage (首页/画廊页)
- [x] **HomePage.xaml** - 已有部分资源
  - 已完成:
    - ✅ `Shell_Gallery.Content`: Gallery / 首页
  - 待添加:
    - [ ] 页面标题
    - [ ] 搜索框提示文本
    - [ ] 筛选器标签
    - [ ] 加载提示
    - [ ] 空状态提示

### 3. DetailPage (详情页)
- [ ] **DetailPage.xaml** - 壁纸详情页
  - 需要本地化的内容:
    - [ ] 页面标题
    - [ ] 壁纸信息标签（地区、日期、版权等）
    - [ ] 操作按钮（下载、设置壁纸、分享等）
    - [ ] 图片质量选项
    - [ ] 错误提示信息

### 4. DownloadPage (下载页)
- [x] **DownloadPage.xaml** - 已有部分资源
  - 已完成:
    - ✅ `Shell_Download.Content`: Download / 下载
  - 待添加:
    - [ ] 页面标题
    - [ ] 下载列表列标题
    - [ ] 状态提示（下载中、已完成、失败等）
    - [ ] 操作按钮（暂停、继续、取消、重试）
    - [ ] 空状态提示

### 5. SettingsPage (设置页)
- [ ] **SettingsPage.xaml** - 应用设置页
  - 需要本地化的内容:
    - [ ] 页面标题
    - [ ] 设置分组标题（外观、下载、关于等）
    - [ ] 设置项标签和描述
    - [ ] 主题选项（浅色、深色、跟随系统）
    - [ ] 语言选项
    - [ ] 关于信息（版本、作者、许可证）
    - [ ] 按钮文本（清除缓存、重置设置等）

---

## 🧩 用户控件 (UserControls)

### 6. BackToTop
- [ ] **BackToTop.xaml** - 返回顶部按钮
  - 需要本地化的内容:
    - [ ] 按钮提示文本
    - [ ] 辅助功能文本

### 7. CustomCard
- [ ] **CustomCard.xaml** - 自定义卡片控件
  - 需要本地化的内容:
    - [ ] 操作按钮文本
    - [ ] 日期格式化

### 8. CustomFooter
- [ ] **CustomFooter.xaml** - 自定义页脚
  - 需要本地化的内容:
    - [ ] 版权信息
    - [ ] 链接文本

### 9. CustomHeader
- [ ] **CustomHeader.xaml** - 自定义页眉
  - 需要本地化的内容:
    - [ ] 标题文本
    - [ ] 副标题文本

### 10. HorizontalScrollContainer
- [ ] **HorizontalScrollContainer.xaml** - 水平滚动容器
  - 需要本地化的内容:
    - [ ] 滚动提示
    - [ ] 辅助功能文本

### 11. InAppNotification
- [ ] **InAppNotification.xaml** - 应用内通知
  - 需要本地化的内容:
    - [ ] 通知消息模板
    - [ ] 操作按钮（关闭、查看详情等）
    - [ ] 成功/警告/错误提示

### 12. LoadingButton
- [ ] **LoadingButton.xaml** - 加载按钮
  - 需要本地化的内容:
    - [ ] 加载中提示文本
    - [ ] 按钮状态文本

### 13. MockupCanvasControl
- [ ] **MockupCanvasControl.xaml** - 预览画布控件
  - 需要本地化的内容:
    - [ ] 工具提示
    - [ ] 错误提示

### 14. MoreDetailsPanel
- [ ] **MoreDetailsPanel.xaml** - 详情面板
  - 需要本地化的内容:
    - [ ] 面板标题
    - [ ] 字段标签
    - [ ] 关闭按钮

### 15. SynchronizationDialog
- [ ] **SynchronizationDialog.xaml** - 同步对话框
  - 需要本地化的内容:
    - [ ] 对话框标题
    - [ ] 提示信息
    - [ ] 进度文本
    - [ ] 按钮（确定、取消、开始同步等）

---

## 🔔 通知和消息

### 16. 应用通知
- [x] **AppNotificationSamplePayload** - 已有基础模板
  - 待添加:
    - [ ] 各种通知场景的消息模板
      - [ ] 下载完成通知
      - [ ] 同步完成通知
      - [ ] 错误通知
      - [ ] 更新通知

---

## 📋 实施指南

### 资源命名规范

采用以下命名约定来组织资源键：

```
{ComponentName}_{ElementType}.{Property}
```

**示例**:
- `HomePage_SearchBox.PlaceholderText`
- `DetailPage_DownloadButton.Content`
- `SettingsPage_AppearanceSection.Header`
- `CustomCard_MoreButton.ToolTip`

### 特殊资源

对于通用消息和提示，使用以下前缀：

- `Common_*`: 通用文本（确定、取消、保存等）
- `Message_*`: 提示消息
- `Error_*`: 错误消息
- `Status_*`: 状态文本

**示例**:
```xml
<!-- 通用按钮 -->
<data name="Common_OK" xml:space="preserve">
  <value>OK</value>
</data>
<data name="Common_Cancel" xml:space="preserve">
  <value>Cancel</value>
</data>

<!-- 状态文本 -->
<data name="Status_Downloading" xml:space="preserve">
  <value>Downloading...</value>
</data>
<data name="Status_Completed" xml:space="preserve">
  <value>Completed</value>
</data>

<!-- 错误消息 -->
<data name="Error_NetworkConnection" xml:space="preserve">
  <value>Network connection failed. Please check your internet connection.</value>
</data>
```

### XAML 使用方法

#### 🏆 推荐方式（首选）- 使用 x:Uid

**优点**: 声明式、简洁、性能好、支持多属性自动绑定

```xml
<!-- 直接使用 x:Uid 绑定资源 -->
<Button x:Uid="HomePage_SearchButton" />
<TextBlock x:Uid="HomePage_Title" />
<TextBox x:Uid="HomePage_SearchBox" />

<!-- 资源文件中可以定义多个属性 -->
<!-- Resources.resw: -->
<!-- HomePage_SearchButton.Content = "Search" -->
<!-- HomePage_SearchButton.ToolTip = "Search for wallpapers" -->
<!-- 上面的 Button 会自动应用所有属性 -->
```

**使用场景**:
- ✅ 所有静态 UI 文本（页面标题、按钮、标签等）
- ✅ 导航菜单项
- ✅ 对话框内容
- ✅ 占比 90-95% 的本地化需求

#### ⚙️ 备选方式 - ResourceLoader（仅在必要时使用）

**适用场景**: 仅用于动态文本、格式化字符串等特殊情况

```csharp
// ViewModel 中使用
public partial class DownloadViewModel : ObservableObject
{
    private readonly ILocalizationService _localization;

    // 场景1: 动态消息组合
    private void UpdateProgress(int current, int total)
    {
        // "Downloaded {0} of {1} items" 或 "已下载 {0}/{1} 项"
        StatusMessage = _localization.GetFormattedString(
            "Status_DownloadProgress",
            current,
            total
        );
    }

    // 场景2: 条件性文本选择
    private void ShowStatus(bool isSuccess)
    {
        var key = isSuccess ? "Status_Success" : "Status_Error";
        StatusMessage = _localization.GetString(key);
    }
}
```

**使用场景**:
- ✅ 带参数的格式化字符串
- ✅ 运行时条件加载
- ✅ 动态消息组合
- ✅ 占比 5-10% 的特殊需求

#### 📋 服务接口定义

```csharp
// Services/ILocalizationService.cs
namespace BinggoWallpapers.WinUI.Services;

/// <summary>
/// 本地化资源服务
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// 获取本地化字符串
    /// </summary>
    string GetString(string key);

    /// <summary>
    /// 获取格式化的本地化字符串
    /// </summary>
    string GetFormattedString(string key, params object[] args);
}
```

### 实施步骤

1. **准备阶段**
   - [ ] 审查所有 XAML 文件，提取需要本地化的文本
   - [ ] 制定完整的资源键列表

2. **资源文件创建**
   - [ ] 更新 `en-US/Resources.resw`，添加所有英文资源
   - [ ] 更新 `zh-CN/Resources.resw`，添加所有中文资源

3. **XAML 更新**
   - [ ] 将硬编码的文本替换为资源引用
   - [ ] 使用 `x:Uid` 或资源加载器

4. **测试验证**
   - [ ] 测试英文语言环境
   - [ ] 测试中文语言环境
   - [ ] 验证语言切换功能
   - [ ] 检查 UI 布局是否适配不同语言

5. **完善优化**
   - [ ] 调整文本长度导致的 UI 问题
   - [ ] 优化翻译质量
   - [ ] 添加缺失的资源

---

## 🎯 优先级

### 高优先级（P0）
必须尽快完成，影响核心用户体验：
- [ ] HomePage (首页/画廊页)
- [ ] DetailPage (详情页)
- [ ] SettingsPage (设置页)
- [ ] ShellWindow (主窗口导航)
- [ ] 应用通知模板

### 中优先级（P1）
重要但不紧急：
- [ ] DownloadPage (下载页)
- [ ] InAppNotification (应用内通知)
- [ ] CustomCard (卡片控件)
- [ ] MoreDetailsPanel (详情面板)
- [ ] SynchronizationDialog (同步对话框)

### 低优先级（P2）
可以后续完善：
- [ ] CustomHeader/CustomFooter
- [ ] LoadingButton
- [ ] BackToTop
- [ ] HorizontalScrollContainer
- [ ] MockupCanvasControl

---

## 📝 注意事项

1. **文本长度**: 注意不同语言的文本长度差异，确保 UI 布局能够适应
2. **日期格式**: 使用 `CultureInfo` 格式化日期和时间
3. **数字格式**: 注意千位分隔符和小数点的不同
4. **复数形式**: 英文需要处理单复数形式
5. **从右到左语言**: 虽然目前只支持中英文，但需要考虑未来扩展
6. **辅助功能**: 确保屏幕阅读器能够正确读取本地化文本
7. **图片文本**: 如果图片中包含文本，需要提供本地化版本

---

## ✅ 更新日志

### 2025-11-14
- 📄 创建多语言支持 TODO List
- ✅ 确认现有资源：
  - `AppDisplayName`: 应用显示名称
  - `AppDescription`: 应用描述
  - `Shell_Gallery.Content`: 画廊导航项
  - `Shell_Download.Content`: 下载导航项
- 📋 识别 16 个需要本地化的组件
- 🎯 定义优先级和实施计划
- 📝 更新 XAML 使用方法：
  - 明确推荐使用 `x:Uid` 作为首选方式（90-95% 场景）
  - 说明 `ResourceLoader` 仅用于动态文本等特殊场景（5-10%）
  - 添加详细的使用场景和代码示例
  - 定义 `ILocalizationService` 服务接口

---

## 🔗 相关资源

- [WinUI 3 本地化文档](https://learn.microsoft.com/windows/apps/windows-app-sdk/localize-strings)
- [.resx 文件格式](https://learn.microsoft.com/dotnet/framework/resources/creating-resource-files-for-desktop-apps)
- [项目 EditorConfig](.editorconfig)
- [项目 README](README.md)

---

**最后更新**: 2025-11-14
**维护者**: @hippieZhou

