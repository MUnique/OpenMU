// <copyright file="DatabaseSnapshotServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the database snapshot, which requires a running postgres database.
/// </summary>
[TestFixture]
internal class DatabaseSnapshotServiceTests
{
    /// <summary>
    /// Creates the initial data, creates a snapshot, re-creates the database, restores the snapshot
    /// and checks if the data is there again.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task SnapshotRoundTripAsync()
    {
        // These converters are registered by the host application (see Startup/Program.cs).
        JsonConverterRegistry.ClearConverters();
        JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());

        await ReCreateDatabaseAsync().ConfigureAwait(false);
        var sourceProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        await new VersionSeasonSix.DataInitialization(sourceProvider, new NullLoggerFactory())
            .CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var expected = await GetCountsAsync(sourceProvider).ConfigureAwait(false);

        var snapshotService = new DatabaseSnapshotService();
        using var snapshot = new MemoryStream();
        await snapshotService.CreateSnapshotAsync(snapshot).ConfigureAwait(false);
        Assert.That(snapshot.Length, Is.GreaterThan(0));

        snapshot.Position = 0;
        Assert.That(await snapshotService.GetRestoreBlockingReasonAsync(snapshot).ConfigureAwait(false), Is.Null);
        Assert.That(snapshot.Position, Is.Zero, "The stream position should be restored after the check.");

        // The restore re-creates the database on its own.
        await snapshotService.RestoreSnapshotAsync(snapshot).ConfigureAwait(false);

        var actual = await GetCountsAsync(new PersistenceContextProvider(new NullLoggerFactory(), null)).ConfigureAwait(false);
        Assert.That(actual, Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests if a snapshot of a newer server is rejected - we don't know how to create its schema.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task SnapshotOfNewerServerIsRejectedAsync()
    {
        await ReCreateDatabaseAsync().ConfigureAwait(false);
        var snapshotService = new DatabaseSnapshotService();
        using var snapshot = new MemoryStream();
        await snapshotService.CreateSnapshotAsync(snapshot).ConfigureAwait(false);

        // Pretend that the snapshot was created by a server which has one more migration:
        snapshot.Position = 0;
        using (var archive = new ZipArchive(snapshot, ZipArchiveMode.Update, leaveOpen: true))
        {
            var manifestEntry = archive.GetEntry("manifest.json")!;
            string manifest;
            await using (var readStream = manifestEntry.Open())
            {
                using var reader = new StreamReader(readStream);
                manifest = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            manifest = manifest.Replace("\"Migrations\":[", "\"Migrations\":[\"99999999999999_FromTheFuture\",", StringComparison.Ordinal);
            manifestEntry.Delete();
            var newEntry = archive.CreateEntry("manifest.json");
            await using var writeStream = newEntry.Open();
            await using var writer = new StreamWriter(writeStream);
            await writer.WriteAsync(manifest).ConfigureAwait(false);
        }

        snapshot.Position = 0;
        var reason = await snapshotService.GetRestoreBlockingReasonAsync(snapshot).ConfigureAwait(false);
        Assert.That(reason, Does.Contain("99999999999999_FromTheFuture"));
    }

    /// <summary>
    /// Tests if a file which is no snapshot is rejected, so that the database isn't dropped for nothing.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task OtherFilesAreRejectedAsync()
    {
        var snapshotService = new DatabaseSnapshotService();
        using var noZipStream = new MemoryStream("This is not a zip archive."u8.ToArray());

        Assert.That(await snapshotService.GetRestoreBlockingReasonAsync(noZipStream).ConfigureAwait(false), Is.Not.Null);
        Assert.That(noZipStream.Position, Is.Zero);
    }

    private static async ValueTask ReCreateDatabaseAsync()
    {
        var contextProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        using var update = await contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
    }

    private static async ValueTask<Dictionary<string, int>> GetCountsAsync(IPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();

        return new Dictionary<string, int>
        {
            [nameof(GameConfiguration.Maps)] = configuration.Maps.Count,
            [nameof(GameConfiguration.Items)] = configuration.Items.Count,
            [nameof(GameConfiguration.Monsters)] = configuration.Monsters.Count,
            [nameof(GameConfiguration.Skills)] = configuration.Skills.Count,
            [nameof(GameConfiguration.CharacterClasses)] = configuration.CharacterClasses.Count,
            [nameof(GameConfiguration.Attributes)] = configuration.Attributes.Count,
            [nameof(GameConfiguration.ItemSlotTypes)] = configuration.ItemSlotTypes.Sum(slotType => slotType.ItemSlots.Count),
            ["BaseAttributeValues"] = configuration.CharacterClasses.Sum(c => c.BaseAttributeValues.Count(a => a.Value != 0)),
            ["QualifiedCharacters"] = configuration.Items.Sum(item => item.QualifiedCharacters.Count),
            [nameof(Account)] = (await context.GetAsync<Account>().ConfigureAwait(false)).Count(),
            [nameof(SystemConfiguration)] = (await context.GetAsync<SystemConfiguration>().ConfigureAwait(false)).Count(),
            ["AppliedUpdates"] = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Count(),
        };
    }
}
