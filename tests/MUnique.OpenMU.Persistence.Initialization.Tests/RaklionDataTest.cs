// <copyright file="RaklionDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the raklion event.
/// </summary>
[TestFixture]
internal class RaklionDataTest
{
    private const short HatcheryNumber = 58;

    /// <summary>
    /// Tests that the monsters of the hatchery of a new database are spawned by the event.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventSpawnsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        AssertEventSpawns(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update changes the automatic spawns of the hatchery of an existing database to the waves of the event.
    /// </summary>
    [Test]
    public async Task UpdateChangesSpawnsOfExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        foreach (var spawn in GetHatchery(gameConfiguration).MonsterSpawns)
        {
            spawn.SpawnTrigger = SpawnTrigger.Automatic;
            spawn.WaveNumber = 0;
        }

        await new AddRaklionEventUpdatePlugIn().ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);

        AssertEventSpawns(gameConfiguration);
    }

    private static async Task<GameConfiguration> CreateConfigurationAsync(InMemoryPersistenceContextProvider contextProvider)
    {
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static GameMapDefinition GetHatchery(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Maps.Single(map => map.Number == HatcheryNumber);
    }

    private static void AssertEventSpawns(GameConfiguration gameConfiguration)
    {
        var definition = new RaklionEventDefinition();
        var spawns = GetHatchery(gameConfiguration).MonsterSpawns;
        Assert.That(spawns.Where(spawn => spawn.SpawnTrigger != SpawnTrigger.OnceAtWaveStart), Is.Empty);

        var selupan = spawns.Where(spawn => spawn.WaveNumber == definition.SelupanWaveNumber).ToList();
        Assert.That(selupan, Has.Count.EqualTo(1));
        Assert.That(selupan[0].MonsterDefinition!.Number, Is.EqualTo(459));
        Assert.That((selupan[0].X1, selupan[0].Y1), Is.EqualTo((145, 31)));

        var eggs = spawns.Where(spawn => spawn.WaveNumber == definition.SpiderEggWaveNumber).ToList();
        Assert.That(eggs.Sum(spawn => spawn.Quantity), Is.EqualTo(15));
        Assert.That(eggs.Select(spawn => spawn.MonsterDefinition!.Number), Is.All.InRange(460, 462));

        var summons = spawns.Where(spawn => spawn.WaveNumber == definition.SummonWaveNumber).ToList();
        Assert.That(summons.Sum(spawn => spawn.Quantity), Is.EqualTo(10));
        Assert.That(summons.Select(spawn => spawn.MonsterDefinition!.Number), Is.All.EqualTo(457));
    }
}
