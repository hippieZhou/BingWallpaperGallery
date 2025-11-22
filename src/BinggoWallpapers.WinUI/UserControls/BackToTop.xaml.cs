using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinggoWallpapers.WinUI.UserControls;

public sealed partial class BackToTop : UserControl
{
    private ScrollViewer? _scrollViewer;
    private const double ScrollThreshold = 200.0; // 滚动超过200像素时显示按钮

    public BackToTop()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    [GeneratedDependencyProperty]
    public partial object? Target { get; set; }

    [GeneratedDependencyProperty]
    public partial bool? AutoHiding { get; set; }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // 当控件加载时，查找目标ScrollViewer
        FindScrollViewer();

        // 设置初始显示状态
        UpdateInitialVisibility();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // 清理事件监听
        if (_scrollViewer != null)
        {
            _scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
            _scrollViewer = null;
        }
    }

    private void FindScrollViewer()
    {
        if (Target == null)
        {
            return;
        }

        // 如果Target是ScrollViewer，直接使用
        if (Target is ScrollViewer scrollViewer)
        {
            SetupScrollViewer(scrollViewer);
            return;
        }

        // 如果Target是其他控件，尝试查找其内部的ScrollViewer
        if (Target is ListViewBase element)
        {
            var foundScrollViewer = element.FindDescendant<ScrollViewer>();
            if (foundScrollViewer != null)
            {
                SetupScrollViewer(foundScrollViewer);
            }
        }
    }

    private void SetupScrollViewer(ScrollViewer scrollViewer)
    {
        _scrollViewer = scrollViewer;
        _scrollViewer.ViewChanged += OnScrollViewerViewChanged;

        // 初始状态检查
        UpdateButtonVisibility();
    }

    private void UpdateInitialVisibility()
    {
        // 如果AutoHiding为false或null，始终显示按钮
        if (AutoHiding != true)
        {
            Visibility = Visibility.Visible;
        }
        else
        {
            // 如果AutoHiding为true，初始时隐藏按钮（因为还没有滚动）
            Visibility = Visibility.Collapsed;
        }
    }

    private void OnScrollViewerViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility()
    {
        if (_scrollViewer == null)
        {
            return;
        }

        // 如果AutoHiding为false或null，始终显示按钮
        if (AutoHiding != true)
        {
            Visibility = Visibility.Visible;
            return;
        }

        // 如果AutoHiding为true，根据滚动位置控制显示
        var verticalOffset = _scrollViewer.VerticalOffset;

        if (verticalOffset > ScrollThreshold)
        {
            // 滚动超过阈值时显示按钮
            Visibility = Visibility.Visible;
        }
        else
        {
            // 滚动未超过阈值时隐藏按钮
            Visibility = Visibility.Collapsed;
        }
    }

    private void OnBackToTop()
    {
        if (_scrollViewer != null)
        {
            // 平滑滚动到顶部
            _scrollViewer.ChangeView(null, 0, null, true);
        }
    }

    /// <summary>
    /// 当Target属性改变时重新查找ScrollViewer
    /// </summary>
    /// <param name="e"></param>
    partial void OnTargetPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        // 清理旧的事件监听
        if (_scrollViewer != null)
        {
            _scrollViewer.ViewChanged -= OnScrollViewerViewChanged;
            _scrollViewer = null;
        }

        // 设置新的事件监听
        if (IsLoaded)
        {
            FindScrollViewer();
        }
    }

    /// <summary>
    /// 当AutoHiding属性改变时更新按钮状态
    /// </summary>
    /// <param name="e"></param>
    partial void OnAutoHidingPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        // 属性变更时立即更新按钮状态
        UpdateButtonVisibility();
    }
}
