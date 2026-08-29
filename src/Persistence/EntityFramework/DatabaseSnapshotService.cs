// <copyright file="DatabaseSnapshotService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Npgsql;

/// <summary>
/// Implementation of the <see cref="IDatabaseSnapshotService"/> which uses the COPY command of postgres.
/// Each table is written into its own entry of a zip archive, in the binary format of postgres.
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
    private const int CurrentFormatVersion = 1;

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
            await GetAppliedMigrationsAsync(connection, cancellationToken).ConfigureAwait(false),
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

            await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            var currentMigrations = await GetAppliedMigrationsAsync(connection, cancellationToken).ConfigureAwait(false);
            if (!currentMigrations.SequenceEqual(manifest.Migrations))
            {
                return "The snapshot was created with another database schema than the one of this server. "
                       + "Please use the data backup (json) to transfer data between different versions.";
            }

            return null;
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

        await using var connection = await CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        var existingTables = (await GetTableNamesAsync(connection, cancellationToken).ConfigureAwait(false))
            .Select(table => table.ToString())
            .ToHashSet(StringComparer.Ordinal);
        if (manifest.Tables.FirstOrDefault(table => !existingTables.Contains(table)) is { } missingTable)
        {
            throw new InvalidOperationException($"The table '{missingTable}' of the snapshot doesn't exist in this database. The snapshot can't be restored.");
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

    private static async ValueTask<string[]> GetAppliedMigrationsAsync(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT table_schema
            FROM information_schema.tables
            WHERE table_name = '__EFMigrationsHistory'
            ORDER BY table_schema
            """;
        var historySchemas = new System.Collections.Generic.List<string>();
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
        {
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                historySchemas.Add(reader.GetString(0));
            }
        }

        var migrations = new System.Collections.Generic.List<string>();
        foreach (var schema in historySchemas)
        {
            await using var migrationCommand = connection.CreateCommand();
            migrationCommand.CommandText = $"""SELECT "MigrationId" FROM "{schema.Replace("\"", "\"\"", StringComparison.Ordinal)}"."__EFMigrationsHistory" """;
            await using var migrationReader = await migrationCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await migrationReader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                migrations.Add($"{schema}.{migrationReader.GetString(0)}");
            }
        }

        migrations.Sort(StringComparer.Ordinal);
        return migrations.ToArray();
    }

    /// <summary>
    /// The description of a snapshot, so that we can check if it fits to the current database.
    /// </summary>
    /// <param name="FormatVersion">The version of the snapshot format.</param>
    /// <param name="CreatedAt">The point in time when the snapshot was created.</param>
    /// <param name="Migrations">The database migrations which were applied when the snapshot was created.</param>
    /// <param name="Tables">The names of the tables which are contained in the snapshot.</param>
    private sealed record SnapshotManifest(int FormatVersion, DateTime CreatedAt, string[] Migrations, string[] Tables);

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
