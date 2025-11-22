// Copyright (c) hippieZhou. All rights reserved.

using System.Reflection;
using BinggoWallpapers.Core.Http.Attributes;
using BinggoWallpapers.Core.Http.Enums;

namespace BinggoWallpapers.Core.Http.Extensions;

public static class ResolutionCodeExtensions
{
    private static ResolutionInfoAttribute GetResolutionInfo(this ResolutionCode resolutionCode)
    {
        var fieldInfo = resolutionCode.GetType().GetField(resolutionCode.ToString());
        return fieldInfo?.GetCustomAttribute<ResolutionInfoAttribute>();
    }

    public static string GetName(this ResolutionCode resolutionCode)
    {
        return resolutionCode.GetResolutionInfo()?.Name;
    }

    public static string GetSuffix(this ResolutionCode resolutionCode)
    {
        return resolutionCode.GetResolutionInfo()?.Suffix;
    }

    public static (int width, int height) GetResolutionDimensions(this ResolutionCode resolution)
    {
        var width = resolution.GetResolutionInfo().Width;
        var height = resolution.GetResolutionInfo().Height;
        return (width, height);
    }
}
