// <copyright file="DatabaseSnapshotService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MUnique.OpenMU.Persistence.EntityFramework.AdminAuth;
using Npgsql;

/// <summary>
/// Implementation of the <see cref="IDatabaseSnapshotService"/> which uses the COPY command of postgres.
/// Each table is written into its own entry of a zip archive, in the binary format of postgres.
/// The archive also contains a manifest with the applied database migrations, so that a snapshot of
/// an older server can be restored: its schema is created first, and the migrations which came
/// afterwards are applied to the restored data.
/// </summary>
public class DatabaseSnapshotService : IDatabaseSnapshotService
{
    /// <summary>
    /// The name of the archive entry which describes the snapshot.
    /// </summary>
    private const string ManifestEntryName = "manifest.json";

    /// <summary>
    /// The version of the snapshot format. It's increased when the layout of the archive changes.
    /// </summary>
    private const int CurrentFormatVersion = 2;

    private const string TableEntryExtension = ".bin";

    private static readonly string[] IncludedSchemas =
    [
        SchemaNames.Configuration,
        SchemaNames.AccountData,
        SchemaNames.Guild,
        SchemaNames.Friend,
        SchemaNames.AdminPanel,
    ];

    /// <inheritdoc />
    public async Task CreateSnapshotAsync(Stream outputStream, CancellationToken cancellationToken = default)
    {
        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        using var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true);

        var tables = await GetTableNamesAsync(connection, cancellationToken).ConfigureAwait(false);
        var manifest = new SnapshotManifest(
            CurrentFormatVersion,
            DateTime.UtcNow,
            await GetAppliedMigrationsAsync<EntityDataContext>(cancellationToken).ConfigureAwait(false),
            await GetAppliedMigrationsAsync<AdminPanelContext>(cancellationToken).ConfigureAwait(false),
            tables.Select(table => table.ToString()).ToArray());

        var manifestEntry = archive.CreateEntry(ManifestEntryName);
        await using (var manifestStream = manifestEntry.Open())
        {
            await JsonSerializer.SerializeAsync(manifestStream, manifest, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        foreach (var table in tables)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entry = archive.CreateEntry($"{table}{TableEntryExtension}");
            await using var entryStream = entry.Open();
            await using var copyStream = await connection
                .BeginRawBinaryCopyAsync($"COPY {table.ToQuotedString()} TO STDOUT (FORMAT BINARY)", cancellationToken)
                .ConfigureAwait(false);
            await copyStream.CopyToAsync(entryStream, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask<string?> GetRestoreBlockingReasonAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        var previousPosition = inputStream.Position;
        try
        {
            using var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true);
            if (await ReadManifestAsync(archive, cancellationToken).ConfigureAwait(false) is not { } manifest)
            {
                return "The selected file is no database snapshot.";
            }

            if (manifest.FormatVersion != CurrentFormatVersion)
            {
                return $"The snapshot was created in format version {manifest.FormatVersion}, but this server expects version {CurrentFormatVersion}.";
            }

            return GetIncompatibilityReason<EntityDataContext>(manifest.Migrations, "game database")
                   ?? GetIncompatibilityReason<AdminPanelContext>(manifest.AdminPanelMigrations, "admin panel database");
        }
        catch (InvalidDataException)
        {
            return "The selected file is no zip archive.";
        }
        finally
        {
            inputStream.Position = previousPosition;
        }
    }

    /// <inheritdoc />
    public async Task RestoreSnapshotAsync(Stream inputStream, CancellationToken cancellationToken = default)
    {
        using var archive = new ZipArchive(inputStream, ZipArchiveMode.Read, leaveOpen: true);
        var manifest = await ReadManifestAsync(archive, cancellationToken).ConfigureAwait(false)
                       ?? throw new ArgumentException($"The archive doesn't contain a {ManifestEntryName}, so it's no database snapshot.", nameof(inputStream));

        if ((GetIncompatibilityReason<EntityDataContext>(manifest.Migrations, "game database")
             ?? GetIncompatibilityReason<AdminPanelContext>(manifest.AdminPanelMigrations, "admin panel database")) is { } incompatibility)
        {
            throw new InvalidOperationException(incompatibility);
        }

        // The data of the snapshot fits to the database schema of the moment when it was created,
        // so we build exactly that schema first. Afterwards, the migrations which came later are
        // applied to the restored data, like it would happen for a running server.
        await DeleteDatabaseAsync(cancellationToken).ConfigureAwait(false);
        await MigrateToSnapshotStateAsync<EntityDataContext>(manifest.Migrations, cancellationToken).ConfigureAwait(false);
        await MigrateToSnapshotStateAsync<AdminPanelContext>(manifest.AdminPanelMigrations, cancellationToken).ConfigureAwait(false);

        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        var existingTables = (await GetTableNamesAsync(connection, cancellationToken).ConfigureAwait(false))
            .Select(table => table.ToString())
            .ToHashSet(StringComparer.Ordinal);
        if (manifest.Tables.FirstOrDefault(table => !existingTables.Contains(table)) is { } missingTable)
        {
            throw new InvalidOperationException($"The table '{missingTable}' of the snapshot doesn't exist in the created database. The snapshot can't be restored.");
        }

        await using var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        // The tables reference each other in circles, so we can't insert them in a dependency order.
        // Like pg_restore, we disable the foreign key checks for this transaction instead.
        await DisableForeignKeyChecksAsync(connection, cancellationToken).ConfigureAwait(false);

        foreach (var tableName in manifest.Tables)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (archive.GetEntry($"{tableName}{TableEntryExtension}") is not { } entry)
            {
                throw new InvalidOperationException($"The archive doesn't contain the data of table '{tableName}', which is listed in its manifest.");
            }

            var table = TableName.Parse(tableName);
            await using var entryStream = entry.Open();
            await using var copyStream = await connection
                .BeginRawBinaryCopyAsync($"COPY {table.ToQuotedString()} FROM STDIN (FORMAT BINARY)", cancellationToken)
                .ConfigureAwait(false);
            await entryStream.CopyToAsync(copyStream, cancellationToken).ConfigureAwait(false);
        }

        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        await connection.CloseAsync().ConfigureAwait(false);

        // Bring the restored data to the current state of this server.
        await MigrateToAsync<EntityDataContext>(null, cancellationToken).ConfigureAwait(false);
        await MigrateToAsync<AdminPanelContext>(null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines why a snapshot with the given migrations can't be restored by this server.
    /// The snapshot may be older than this server - the migrations which came later are applied
    /// to the restored data then. It can't be newer, because we don't know its schema.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="snapshotMigrations">The migrations which were applied when the snapshot was created.</param>
    /// <param name="databaseName">The name of the database, for the message.</param>
    /// <returns>The reason why it can't be restored; <c>null</c>, if it can be restored.</returns>
    private static string? GetIncompatibilityReason<TContext>(string[] snapshotMigrations, string databaseName)
        where TContext : DbContext, new()
    {
        using var context = new TContext();
        var knownMigrations = context.Database.GetMigrations().ToHashSet(StringComparer.Ordinal);
        var unknownMigrations = snapshotMigrations.Where(migration => !knownMigrations.Contains(migration)).ToList();
        if (unknownMigrations.Count > 0)
        {
            return $"The snapshot of the {databaseName} was created by a newer or different version of the server: "
                   + $"it contains the unknown database migration '{unknownMigrations[0]}'. "
                   + "Please use the data backup (json) to transfer the data.";
        }

        return null;
    }

    /// <summary>
    /// Creates the database schema of the moment when the snapshot was created.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="snapshotMigrations">The migrations which were applied when the snapshot was created.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private static async Task MigrateToSnapshotStateAsync<TContext>(string[] snapshotMigrations, CancellationToken cancellationToken)
        where TContext : DbContext, new()
    {
        if (snapshotMigrations.Length == 0)
        {
            // This database didn't exist when the snapshot was created; it's created below, when
            // the remaining migrations are applied.
            return;
        }

        await using var context = new TContext();

        // The migrations are applied in the order of their identifier. When a migration was added
        // later with an earlier identifier - which happens when branches are merged - it's applied
        // here, too. That's not a problem as long as it doesn't change a table of the snapshot;
        // otherwise the copy of that table fails, and the restore is rolled back.
        var target = context.Database.GetMigrations().Last(snapshotMigrations.Contains);
        await MigrateToAsync<TContext>(target, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Migrates the schema of the given context to the given migration.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="targetMigration">The migration which should be the last applied one; <c>null</c>, to apply all of them.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private static async Task MigrateToAsync<TContext>(string? targetMigration, CancellationToken cancellationToken)
        where TContext : DbContext, new()
    {
        await using var context = new TContext();
        if (targetMigration is null && !context.Database.GetMigrations().Any())
        {
            return;
        }

        await context.Database.GetService<IMigrator>()
            .MigrateAsync(targetMigration, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task DeleteDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var context = new EntityDataContext();
            await context.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (NpgsqlException)
        {
            // That's expected when there is no database yet.
        }
    }

    private static async Task<NpgsqlConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        // Creating the context also ensures that the connection settings are initialized.
        await using var context = new EntityDataContext();
        var connection = new NpgsqlConnection(context.Database.GetConnectionString());
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private static async Task DisableForeignKeyChecksAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SET LOCAL session_replication_role = replica";
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.InsufficientPrivilege)
        {
            throw new InvalidOperationException(
                "The database user needs superuser rights to restore a snapshot, because the foreign key checks "
                + "have to be disabled while the data is inserted. Please use the data backup (json) instead.",
                exception);
        }
    }

    private static async ValueTask<SnapshotManifest?> ReadManifestAsync(ZipArchive archive, CancellationToken cancellationToken)
    {
        if (archive.GetEntry(ManifestEntryName) is not { } entry)
        {
            return null;
        }

        await using var stream = entry.Open();
        return await JsonSerializer.DeserializeAsync<SnapshotManifest>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<System.Collections.Generic.List<TableName>> GetTableNamesAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        // The migration history is not part of the snapshot - the re-created database brings its own.
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT table_schema, table_name
            FROM information_schema.tables
            WHERE table_schema = ANY(@schemas)
              AND table_type = 'BASE TABLE'
              AND table_name <> '__EFMigrationsHistory'
            ORDER BY table_schema, table_name
            """;
        command.Parameters.AddWithValue("schemas", IncludedSchemas);

        var result = new System.Collections.Generic.List<TableName>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            result.Add(new TableName(reader.GetString(0), reader.GetString(1)));
        }

        return result;
    }

    private static async ValueTask<string[]> GetAppliedMigrationsAsync<TContext>(CancellationToken cancellationToken)
        where TContext : DbContext, new()
    {
        try
        {
            await using var context = new TContext();
            var applied = await context.Database.GetAppliedMigrationsAsync(cancellationToken).ConfigureAwait(false);

            // We keep the order in which they are defined, so that we can determine the last one.
            var knownMigrations = context.Database.GetMigrations().ToList();
            return knownMigrations.Where(applied.Contains).ToArray();
        }
        catch (PostgresException)
        {
            // The database of this context doesn't exist yet.
            return [];
        }
    }

    /// <summary>
    /// The description of a snapshot, so that we can check if it fits to the current database.
    /// </summary>
    /// <param name="FormatVersion">The version of the snapshot format.</param>
    /// <param name="CreatedAt">The point in time when the snapshot was created.</param>
    /// <param name="Migrations">The migrations of the game database which were applied when the snapshot was created.</param>
    /// <param name="AdminPanelMigrations">The migrations of the admin panel database which were applied when the snapshot was created.</param>
    /// <param name="Tables">The names of the tables which are contained in the snapshot.</param>
    private sealed record SnapshotManifest(int FormatVersion, DateTime CreatedAt, string[] Migrations, string[] AdminPanelMigrations, string[] Tables);

    /// <summary>
    /// The name of a table of the database.
    /// </summary>
    /// <param name="Schema">The name of the schema.</param>
    /// <param name="Name">The name of the table.</param>
    private sealed record TableName(string Schema, string Name)
    {
        /// <summary>
        /// Parses a table name of the format "schema.table".
        /// </summary>
        /// <param name="value">The value which should be parsed.</param>
        /// <returns>The parsed table name.</returns>
        public static TableName Parse(string value)
        {
            var separatorIndex = value.IndexOf('.', StringComparison.Ordinal);
            return separatorIndex < 0
                ? throw new ArgumentException($"'{value}' is no valid table name.", nameof(value))
                : new TableName(value[..separatorIndex], value[(separatorIndex + 1)..]);
        }

        /// <inheritdoc />
        public override string ToString() => $"{this.Schema}.{this.Name}";

        /// <summary>
        /// Gets the quoted name which can be used in a sql statement.
        /// </summary>
        /// <returns>The quoted name.</returns>
        public string ToQuotedString() => $"{Quote(this.Schema)}.{Quote(this.Name)}";

        private static string Quote(string identifier) => $"\"{identifier.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
