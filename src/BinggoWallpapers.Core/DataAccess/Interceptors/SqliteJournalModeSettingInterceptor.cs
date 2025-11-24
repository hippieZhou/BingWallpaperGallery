// Copyright (c) hippieZhou. All rights reserved.

using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BinggoWallpapers.Core.DataAccess.Interceptors;

public class SqliteJournalModeSettingInterceptor : DbConnectionInterceptor
{
    private const string COMMAND_TEXT = "PRAGMA journal_mode = Wal";

    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (connection is not SqliteConnection)
        {
            return;
        }

        using var cmd = connection.CreateCommand();
        cmd.CommandText = COMMAND_TEXT;
        cmd.ExecuteScalar();
        base.ConnectionOpened(connection, eventData);
    }

    public async override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData,
        CancellationToken cancellationToken = new())
    {
        if (connection is not SqliteConnection)
        {
            return;
        }

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = COMMAND_TEXT;
        await cmd.ExecuteScalarAsync(cancellationToken);
        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}
