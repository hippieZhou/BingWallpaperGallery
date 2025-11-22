// Copyright (c) hippieZhou. All rights reserved.

namespace BinggoWallpapers.Core.Helpers;

public static class DateTimeProvider
{
    public static DateTimeOffset GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}
