// Copyright (c) hippieZhou. All rights reserved.

using System.Runtime.InteropServices;
using System.Text;

namespace BinggoWallpapers.WinUI.Helpers;

public static class RuntimeHelper
{
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
}
