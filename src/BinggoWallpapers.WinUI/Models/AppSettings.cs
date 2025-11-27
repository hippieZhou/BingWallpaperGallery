// Copyright (c) hippieZhou. All rights reserved.

using System.Runtime.InteropServices;
using BinggoWallpapers.WinUI.Helpers;
using CommunityToolkit.WinUI;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Windows.Storage;

namespace BinggoWallpapers.WinUI.Models;

public partial class AppSettings
{
    public static ApplicationDataContainer LocalSettings => ApplicationData.Current.LocalSettings;

    public static string DefaultPicturesPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "BingWallpaper");

    public static string AppTitle => "AppDisplayName".GetLocalized();

    public static string AppVersion
    {
        get
        {
            var version = RuntimeHelper.GetAppVersion();
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

    public static string OSVersion
    {
        get
        {
            var (major, minor, build, desc) = RuntimeHelper.GetOSVersion();
            return $"Windows {major}.{minor} (Build {build})";
        }
    }

    public static string AppDataPath => RuntimeHelper.GetAppStatePath();

    public static string AppLogsPath => RuntimeHelper.GetAppLogsPath();

    private static string WinAppSdkDetails => $"Windows App SDK {ReleaseInfo.Major}.{ReleaseInfo.Minor}";

    private static string RuntimeInfoAsString => $"Windows App Runtime {RuntimeInfo.AsString}";

    private static string FrameworkDescription => RuntimeInformation.FrameworkDescription;

    public static string WinAppSdkRuntimeDetails => $"{WinAppSdkDetails}, {RuntimeInfoAsString}, {FrameworkDescription}";
}
