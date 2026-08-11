using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace BinggoWallpapers.WinUI.Helpers;

public partial class ScrollOffsetVisibilityBehavior : Behavior<FrameworkElement>
{
    [GeneratedDependencyProperty]
    public partial Button? TargetButton { get; set; }

    [GeneratedDependencyProperty(DefaultValue = 200.0)]
    public partial double Threshold { get; set; }

    private ScrollViewer? _scrollViewer;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnloaded;
        DetachScrollViewer();
        base.OnDetaching();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _scrollViewer = AssociatedObject.FindDescendant<ScrollViewer>();
        if (_scrollViewer != null)
        {
            _scrollViewer.ViewChanged += OnViewChanged;
            TargetButton?.Click += OnTargetButton;
            UpdateVisibility();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => DetachScrollViewer();

    private void OnTargetButton(object sender, RoutedEventArgs e)
    {
        _scrollViewer?.ChangeView(null, 0, null, false);
    }

    private void DetachScrollViewer()
    {
        _scrollViewer?.ViewChanged -= OnViewChanged;
        _scrollViewer = null;
        TargetButton?.Click -= OnTargetButton;
        TargetButton = null;
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e) => UpdateVisibility();

    private void UpdateVisibility()
    {
        if (_scrollViewer == null || TargetButton == null)
        {
            return;
        }

        TargetButton.Visibility = _scrollViewer.VerticalOffset > Threshold
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
