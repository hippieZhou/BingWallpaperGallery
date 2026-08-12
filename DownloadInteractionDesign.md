# 基于 WinUI 3 与必应壁纸接口的下载功能设计方案

本方案采用 MVVM 架构，在 WinUI 3 中实现一个支持多分辨率、多画幅比例选择的必应壁纸下载器。

---

## 1. 必应 API 核心原理

必应每日壁纸的标准接口会返回一个基础图片 ID（例如 `OHR.MountRainier_EN-US1234567890`）。我们只需要通过**修改 URL 中的分辨率后缀**，即可让微软服务器返回特定尺寸的图片，无需前端手动裁剪。

### 常用分辨率映射表

| 画幅比例 | 目标分辨率 | 适用场景 | 后缀参数 |
| :--- | :--- | :--- | :--- |
| **横屏 (16:9)** | 3840x2160 | 4K 桌面显示器 | `_3840x2160.jpg` |
| **横屏 (16:9)** | 1920x1080 | 1080P 常用桌面 | `_1920x1080.jpg` |
| **竖屏 (9:16)** | 1080x1920 | 手机壁纸 / 侧屏 | `_1080x1920.jpg` |
| **传统 (4:3)**  | 1366x768  | 便携平板 / 旧显示器 | `_1366x768.jpg` |

---

## 2. UI 界面设计 (XAML)

在 WinUI 3 中，推荐使用 `ContentDialog` 作为下载弹窗。以下是弹窗的布局代码，包含了比例单选、分辨率下拉框以及路径选择。

```xml
<ContentDialog
    x:Class="BingWallpaperApp.Views.DownloadDialog"
    xmlns="http://microsoft.com"
    xmlns:x="http://microsoft.com"
    Title="下载壁纸"
    PrimaryButtonText="开始下载"
    CloseButtonText="取消"
    DefaultButton="Primary"
    Style="{StaticResource DefaultContentDialogStyle}">

    <StackPanel Spacing="16" Width="420" Margin="0,8,0,0">
        <!-- 1. 画幅比例选择 -->
        <StackPanel Spacing="6">
            <TextBlock Text="选择画幅比例" Style="{ThemeResource BodyStrongTextBlockStyle}"/>
            <RadioButtons x:Name="RatioRadioButtons" SelectedIndex="0" SelectionChanged="OnRatioSelectionChanged">
                <RadioButton Content="电脑横屏 (16:9)" Tag="Horizontal"/>
                <RadioButton Content="手机竖屏 (9:16)" Tag="Vertical"/>
            </RadioButtons>
        </StackPanel>

        <!-- 2. 分辨率选择 -->
        <StackPanel Spacing="6">
            <TextBlock Text="选择分辨率" Style="{ThemeResource BodyStrongTextBlockStyle}"/>
            <ComboBox x:Name="ResolutionComboBox" 
                      HorizontalAlignment="Stretch" 
                      DisplayMemberPath="DisplayName"/>
        </StackPanel>

        <!-- 3. 保存路径选择 -->
        <StackPanel Spacing="6">
            <TextBlock Text="保存位置" Style="{ThemeResource BodyStrongTextBlockStyle}"/>
            <Grid ColumnDefinitions="*, Auto">
                <TextBox x:Name="PathTextBox" 
                         IsReadOnly="True" 
                         Text="C:\Users\Public\Pictures" 
                         VerticalAlignment="Center"/>
                <Button Grid.Column="1" 
                        Content="浏览..." 
                        Margin="8,0,0,0" 
                        Click="OnSelectFolderClick"/>
            </Grid>
        </StackPanel>

        <!-- 4. 下载进度条 (默认隐藏) -->
        <StackPanel x:Name="ProgressPanel" Visibility="Collapsed" Spacing="6">
            <ProgressBar x:Name="DownloadProgressBar" Minimum="0" Maximum="100" Value="0"/>
            <TextBlock x:Name="ProgressStatusText" Text="正在准备下载..." Style="{ThemeResource CaptionTextBlockStyle}" Foreground="{ThemeResource SystemControlPageTextBaseMediumBrush}"/>
        </StackPanel>
    </StackPanel>
</ContentDialog>
```

---

## 3. 后端业务逻辑 (C#)

### 3.1 数据模型 (Model)
定义分辨率实体，方便进行 UI 绑定。
```csharp
public class ResolutionItem
{
    public string DisplayName { get; set; }     // UI 显示名称，如 "4K 超清 (3840x2160)"
    public string ResolutionCode { get; set; }  // 必应接口代码，如 "3840x2160"
}
```

### 3.2 弹窗后台交互 (View 代码隐藏)
处理 WinUI 3 中特殊的 `FolderPicker` 句柄初始化，以及根据单选框动态切换下拉框内容。
```csharp
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;

namespace BingWallpaperApp.Views
{
    public partial class DownloadDialog : ContentDialog
    {
        private List<ResolutionItem> _horizontalResolutions;
        private List<ResolutionItem> _verticalResolutions;

        public ResolutionItem SelectedResolution => ResolutionComboBox.SelectedItem as ResolutionItem;
        public string SelectedPath => PathTextBox.Text;

        public DownloadDialog()
        {
            this.InitializeComponent();
            InitializeResolutionData();
            UpdateResolutionComboBox();
        }

        private void InitializeResolutionData()
        {
            _horizontalResolutions = new List<ResolutionItem>
            {
                new ResolutionItem { DisplayName = "4K 极清 (3840x2160)", ResolutionCode = "3840x2160" },
                new ResolutionItem { DisplayName = "1080P 高清 (1920x1080)", ResolutionCode = "1920x1080" }
            };

            _verticalResolutions = new List<ResolutionItem>
            {
                new ResolutionItem { DisplayName = "手机竖屏 (1080x1920)", ResolutionCode = "1080x1920" }
            };
        }

        private void OnRatioSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResolutionComboBox();
        }

        private void UpdateResolutionComboBox()
        {
            if (RatioRadioButtons == null || ResolutionComboBox == null) return;

            var selectedRadio = RatioRadioButtons.SelectedItem as RadioButton;
            if (selectedRadio?.Tag?.ToString() == "Horizontal")
            {
                ResolutionComboBox.ItemsSource = _horizontalResolutions;
            }
            else
            {
                ResolutionComboBox.ItemsSource = _verticalResolutions;
            }
            ResolutionComboBox.SelectedIndex = 0;
        }

        // WinUI 3 中使用 Picker 必须绑定窗口句柄 (HWND)
        private async void OnSelectFolderClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            // 获取当前应用主窗口的 HWND
            var window = (Application.Current as App)?.m_window; 
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                PathTextBox.Text = folder.Path;
            }
        }

        public void ShowProgress(bool show)
        {
            ProgressPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            IsPrimaryButtonEnabled = !show; // 下载时禁用确定按钮
        }

        public void UpdateProgress(double value)
        {
            DownloadProgressBar.Value = value;
            ProgressStatusText.Text = $"下载进度: {value:F0}%";
        }
    }
}
```

### 3.3 核心下载服务 (Service)
使用 `HttpClient` 流式下载文件，并通过 `IProgress<double>` 实时将下载百分比回传给 WinUI 界面，避免界面假死。
```csharp
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BingWallpaperApp.Services
{
    public class WallpaperDownloadService
    {
        private readonly HttpClient _httpClient;

        public WallpaperDownloadService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 异步下载壁纸并报告进度
        /// </summary>
        /// <param name="baseId">必应壁纸基础ID</param>
        /// <param name="resolutionCode">分辨率代码</param>
        /// <param name="targetFolder">保存的目标文件夹路径</param>
        /// <param name="progress">进度回调接口</param>
        public async Task DownloadWithProgressAsync(string baseId, string resolutionCode, string targetFolder, IProgress<double> progress)
        {
            string url = $"https://bing.com{baseId}_{resolutionCode}.jpg";
            string fileName = $"Bing_{baseId}_{resolutionCode}.jpg";
            string fullPath = Path.Combine(targetFolder, fileName);

            // 使用 ResponseHeadersRead 开启流式读取，以便计算进度
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long totalReadBytes = 0;
            int readBytes;

            while ((readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, readBytes);
                totalReadBytes += readBytes;

                if (totalBytes != -1 && progress != null)
                {
                    double progressPercentage = (double)totalReadBytes / totalBytes * 100;
                    progress.Report(progressPercentage);
                }
            }
        }
    }
}
```

---

## 4. 完美串联：触发弹窗与执行下载

在你的主页面（如 `MainWindow.xaml.cs` 或 `MainPage.xaml.cs`）中，通过以下方式调用并执行整个下载流程：

```csharp
private async void OnDownloadButtonClick(object sender, RoutedEventArgs e)
{
    // 假设当前正在查看的壁纸 ID
    string currentWallpaperBaseId = "OHR.MountRainier_EN-US1234567890"; 

    var dialog = new DownloadDialog();
    // 必须设置 XamlRoot 才能在 WinUI 3 中弹出对话框
    dialog.XamlRoot = this.Content.XamlRoot; 

    ContentDialogResult result = await dialog.ShowAsync();

    if (result == ContentDialogResult.Primary)
    {
        var selectedRes = dialog.SelectedResolution;
        var savePath = dialog.SelectedPath;

        if (selectedRes == null || string.IsNullOrEmpty(savePath)) return;

        // 重新显示弹窗并展示进度条
        dialog.ShowProgress(true);
        var dialogTask = dialog.ShowAsync(); 

        var downloadService = new WallpaperDownloadService();
        var progressReporter = new Progress<double>(percent =>
        {
            dialog.UpdateProgress(percent);
        });

        try
        {
            await downloadService.DownloadWithProgressAsync(
                currentWallpaperBaseId, 
                selectedRes.ResolutionCode, 
                savePath, 
                progressReporter
            );
            dialog.Hide();// 提示：此处可自行调用系统 Toast 通知下载成功}
        }
        catch (Exception ex)
        {
            dialog.Hide();// 提示：此处可自行处理异常弹窗，如：App.ShowError(ex.Message);
        }
    }
}
```
