// Copyright (c) hippieZhou. All rights reserved.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;

namespace BinggoWallpapers.WinUI.Helpers;

public static class RuntimeHelper
{
    private const string BinggoWallpapers = nameof(BinggoWallpapers);
    private const string TempStateDir = "TempState";
    private const string LocalStateDir = "LocalState";
    private const string LocalCacheDir = "LocalCache";
    private const string LogsDir = "Logs";

    public const string SettingsFile = "settings.json";

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetCurrentPackageFullName(ref int packageFullNameLength, StringBuilder packageFullName);

    public static bool IsMSIX
    {
        get
        {
            var length = 0;

            return GetCurrentPackageFullName(ref length, null) != 15700L;
        }
    }

    [DllImport("ntdll.dll", EntryPoint = "RtlGetVersion", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int RtlGetVersion(ref OSVERSIONINFOEX versionInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct OSVERSIONINFOEX
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }

    public static (int Major, int Minor, int Build, string Desc) GetOSVersion()
    {
        var osVersionInfo = new OSVERSIONINFOEX();
        osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(osVersionInfo);

        _ = RtlGetVersion(ref osVersionInfo);

        var desc = $"{osVersionInfo.dwMajorVersion}.{osVersionInfo.dwMinorVersion}.{osVersionInfo.dwBuildNumber}";
        return (osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion, osVersionInfo.dwBuildNumber, desc);
    }

    public static Version GetAppVersion()
    {
        if (IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;
            return new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }

        return Assembly.GetExecutingAssembly().GetName().Version!;
    }

    public static string GetAppStatePath() => GetLocalStatePath();

    public static string GetAppLogsPath() => Path.Combine(GetTempStatePath(), LogsDir);

    public static string GetAppCachePath() => GetLocalCachePath();

    private static string GetTempStatePath() => IsMSIX ? ApplicationData.Current.TemporaryFolder.Path : GetUnpackagedPath(TempStateDir);

    private static string GetLocalStatePath() => IsMSIX ? ApplicationData.Current.LocalFolder.Path : GetUnpackagedPath(LocalStateDir);

    private static string GetLocalCachePath() => IsMSIX ? ApplicationData.Current.LocalCacheFolder.Path : GetUnpackagedPath(LocalCacheDir);

    private static string GetUnpackagedPath(params string[] paths)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), BinggoWallpapers, Path.Combine(paths));
    }
}
