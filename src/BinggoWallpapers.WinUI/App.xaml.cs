// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.UI.Xaml;

namespace BinggoWallpapers.WinUI;

public partial class App : Application
{
    private readonly Bootstrapper _bootstrapper;
    public static WindowEx MainWindow { get; } = new ShellWindow();

    public static T GetService<T>() where T : class => (Current as App)!._bootstrapper.GetService<T>();

    public App()
    {
        InitializeComponent();

        _bootstrapper = new Bootstrapper();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        _bootstrapper.HandleException(e);
        e.Handled = true;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        await _bootstrapper.StartAsync(args);
    }
}
