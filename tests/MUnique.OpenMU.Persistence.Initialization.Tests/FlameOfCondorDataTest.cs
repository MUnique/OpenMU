// <copyright file="FlameOfCondorDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the Flame of Condor, which drops in the Barracks of Balgass.
/// </summary>
[TestFixture]
internal class FlameOfCondorDataTest
{
    private const short BarracksNumber = 41;
    private const byte OriginalDropLevel = 120;

    /// <summary>
    /// Tests that the Flame of Condor of a new database can drop from the monsters of the Barracks of Balgass,
    /// which means that its drop level is not higher than their level.
    /// </summary>
    [Test]
    public async Task NewDatabaseLetsFlameDropFromBarracksMonstersAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        var barracks = gameConfiguration.Maps.Single(map => map.Number == BarracksNumber);
        var lowestMonsterLevel = barracks.MonsterSpawns
            .Where(spawn => spawn.MonsterDefinition is not null)
            .Select(spawn => spawn.MonsterDefinition!)
            .Where(monster => monster.ObjectKind == NpcObjectKind.Monster)
            .Min(monster => monster.Attributes.Single(a => a.AttributeDefinition == Stats.Level).Value);

        Assert.That(GetFlame(gameConfiguration).DropLevel, Is.LessThanOrEqualTo(lowestMonsterLevel));
    }

    /// <summary>
    /// Tests that the update removes the drop level of the Flame of Condor of an existing database,
    /// and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateRemovesDropLevelOfExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        GetFlame(gameConfiguration).DropLevel = OriginalDropLevel;

        var update = new FixFlameOfCondorDropLevelUpdatePlugIn();
        for (var i = 0; i < 2; i++)
        {
            await update.ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);
        }

        Assert.That(GetFlame(gameConfiguration).DropLevel, Is.Zero);
    }

    /// <summary>
    /// Tests that the update keeps a drop level which was configured otherwise.
    /// </summary>
    [Test]
    public async Task UpdateKeepsConfiguredDropLevelAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        GetFlame(gameConfiguration).DropLevel = 100;

        await new FixFlameOfCondorDropLevelUpdatePlugIn().ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);

        Assert.That(GetFlame(gameConfiguration).DropLevel, Is.EqualTo(100));
    }

    private static async Task<GameConfiguration> CreateConfigurationAsync(InMemoryPersistenceContextProvider contextProvider)
    {
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static ItemDefinition GetFlame(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Single(item => item.Group == 13 && item.Number == 52);
    }
}
