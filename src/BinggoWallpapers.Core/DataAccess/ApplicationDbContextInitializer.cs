// Copyright (c) hippieZhou. All rights reserved.

using Microsoft.EntityFrameworkCore;

namespace BinggoWallpapers.Core.DataAccess;

public static class ApplicationDbContextInitializer
{
    public static async Task InitializeAsync(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        using var dbContext = await dbFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
    }
}
