// Copyright (c) hippieZhou. All rights reserved.

using System.Windows.Input;
using BinggoWallpapers.Core.DTOs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BinggoWallpapers.WinUI.Helpers;

public static class MenuFlyoutHelper
{
    public static object GetItemSource(MenuFlyout obj)
    {
        return obj.GetValue(ItemSourceProperty);
    }

    public static void SetItemSource(MenuFlyout obj, object value)
    {
        obj.SetValue(ItemSourceProperty, value);
    }

    // Using a DependencyProperty as the backing store for ItemSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ItemSourceProperty =
        DependencyProperty.RegisterAttached("ItemSource", typeof(object), typeof(MenuFlyoutHelper), new PropertyMetadata(default, OnItemSourceChanged));

    public static ICommand GetCommand(MenuFlyout obj)
    {
        return (ICommand)obj.GetValue(CommandProperty);
    }

    public static void SetCommand(MenuFlyout obj, ICommand value)
    {
        obj.SetValue(CommandProperty, value);
    }

    // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MenuFlyoutHelper), new PropertyMetadata(default));

    private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MenuFlyout handler && e.NewValue is IList<ResolutionInfoDto> resolutions)
        {
            handler.Items.Clear();
            foreach (var resolution in resolutions)
            {
                handler.Items.Add(new MenuFlyoutItem()
                {
                    Command = GetCommand(handler),
                    CommandParameter = resolution,
                    DataContext = resolution,
                    Text = $"{resolution.Name} - {resolution.Suffix}",
                });
            }
        }
    }
}
