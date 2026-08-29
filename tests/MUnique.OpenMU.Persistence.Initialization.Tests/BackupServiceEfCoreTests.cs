// <copyright file="BackupServiceEfCoreTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Json;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the backup with the entity framework core, which requires a running postgres database.
/// </summary>
[TestFixture]
internal class BackupServiceEfCoreTests
{
    /// <summary>
    /// Creates the initial data, exports a backup, re-creates the database, restores the backup
    /// and checks if the data is there again.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically. It requires a database.")]
    public async Task ExportAndRestoreRoundTripAsync()
    {
        // These converters are registered by the host application (see Startup/Program.cs).
        JsonConverterRegistry.RegisterConverter(new LocalizedStringJsonConverter());
        JsonConverterRegistry.RegisterConverter(new BinaryAsHexJsonConverter());

        await ReCreateDatabaseAsync().ConfigureAwait(false);
        var sourceProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        await new VersionSeasonSix.DataInitialization(sourceProvider, new NullLoggerFactory())
            .CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var expected = await GetCountsAsync(sourceProvider).ConfigureAwait(false);

        using var backup = new MemoryStream();
        await new BackupService(sourceProvider, new InMemoryAdminUserRepository()).CreateBackupAsync(backup).ConfigureAwait(false);
        Assert.That(backup.Length, Is.GreaterThan(0));

        await ReCreateDatabaseAsync().ConfigureAwait(false);
        backup.Position = 0;
        var targetProvider = new PersistenceContextProvider(new NullLoggerFactory(), null);
        await new BackupService(targetProvider, new InMemoryAdminUserRepository()).RestoreBackupAsync(backup).ConfigureAwait(false);

        var actual = await GetCountsAsync(new PersistenceContextProvider(new NullLoggerFactory(), null)).ConfigureAwait(false);
        Assert.That(actual, Is.EqualTo(expected));
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

            // The character classes hold const value attributes, whose value can only be set by their constructor:
            ["BaseAttributeValues"] = configuration.CharacterClasses.Sum(c => c.BaseAttributeValues.Count(a => a.Value != 0)),

            // Many-to-many relations, which are stored in join entities by the entity framework:
            ["QualifiedCharacters"] = configuration.Items.Sum(item => item.QualifiedCharacters.Count),
            ["ItemDropGroups"] = configuration.Maps.Sum(map => map.DropItemGroups.Count),

            [nameof(Account)] = (await context.GetAsync<Account>().ConfigureAwait(false)).Count(),
            [nameof(GameServerDefinition)] = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(false)).Count(),
            [nameof(ConnectServerDefinition)] = (await context.GetAsync<ConnectServerDefinition>().ConfigureAwait(false)).Count(),
            [nameof(ChatServerDefinition)] = (await context.GetAsync<ChatServerDefinition>().ConfigureAwait(false)).Count(),
            [nameof(SystemConfiguration)] = (await context.GetAsync<SystemConfiguration>().ConfigureAwait(false)).Count(),
            ["AppliedUpdates"] = (await context.GetAsync<ConfigurationUpdate>().ConfigureAwait(false)).Count(),
        };
    }
}
