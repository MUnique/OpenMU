// <copyright file="KanturuDataUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the versioned Kanturu update brings an existing season 6 database
/// to the current Kanturu state, and that applying it twice is a no-op.
/// </summary>
/// <remarks>
/// The map and monster numbers are literals, because the initializers which
/// define them as constants are internal to the initialization project.
/// </remarks>
[TestFixture]
internal class KanturuDataUpdateTest
{
    private const byte KanturuEventMapNumber = 39;

    private const byte KanturuRelicsMapNumber = 38;

    private const short MayaBodyNumber = 364;

    /// <summary>
    /// Tests that the update ensures the Kanturu configuration, bosses and wave
    /// spawns, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateEnsuresKanturuDataOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var update = new AddKanturuDataUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var waveSpawnCount = CountWaveSpawns(gameConfiguration);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.MiniGameDefinitions.Any(d => d.Type == MiniGameType.Kanturu), Is.True);
        Assert.That(gameConfiguration.Monsters.Any(m => m.Number == MayaBodyNumber), Is.True);
        Assert.That(CountWaveSpawns(gameConfiguration), Is.EqualTo(waveSpawnCount).And.GreaterThan(0));
        var eventMap = gameConfiguration.Maps.First(m => m.Number == KanturuEventMapNumber);
        Assert.That(eventMap.SafezoneMap?.Number, Is.EqualTo(KanturuRelicsMapNumber));
        var definition = gameConfiguration.MiniGameDefinitions.First(d => d.Type == MiniGameType.Kanturu && d.GameLevel == 1);
        Assert.That(definition.MaximumPlayerCount, Is.EqualTo(15));
    }

    private static int CountWaveSpawns(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Maps
            .First(m => m.Number == KanturuEventMapNumber)
            .MonsterSpawns
            .Count(s => s.SpawnTrigger == SpawnTrigger.OnceAtWaveStart);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
