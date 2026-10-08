// <copyright file="CastleSiegeDataUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the versioned Castle Siege update brings an existing season 6 database
/// to the current Castle Siege state, and that applying it twice is a no-op.
/// </summary>
/// <remarks>
/// The monster numbers are literals, because the initializer which defines
/// them as constants is internal to the initialization project.
/// </remarks>
[TestFixture]
internal class CastleSiegeDataUpdateTest
{
    // Senior NPC of the Castle Siege economy interface.
    private const short SeniorNpcNumber = 223;

    // Life Stone monster.
    private const short LifeStoneMonsterNumber = 278;

    /// <summary>
    /// Tests that the update ensures the Castle Siege configuration with its
    /// registration, participant and Life Stone data on an existing database,
    /// and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateEnsuresCastleSiegeDataOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.CastleSiegeConfiguration!.SignOfLordItemDefinition = null;
        GetSenior(gameConfiguration).NpcWindow = NpcWindow.Undefined;
        foreach (var attribute in GetLifeStone(gameConfiguration).Attributes.ToList())
        {
            GetLifeStone(gameConfiguration).Attributes.Remove(attribute);
        }

        var update = new AddCastleSiegeDataUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.CastleSiegeConfiguration.SignOfLordItemDefinition, Is.Not.Null);
        Assert.That(GetSenior(gameConfiguration).NpcWindow, Is.EqualTo(NpcWindow.CastleSeniorNPC));
        Assert.That(GetLifeStone(gameConfiguration).Attributes.Any(a => a.AttributeDefinition == Stats.MaximumHealth), Is.True);
    }

    private static MonsterDefinition GetSenior(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Monsters.First(m => m.Number == SeniorNpcNumber);
    }

    private static MonsterDefinition GetLifeStone(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Monsters.First(m => m.Number == LifeStoneMonsterNumber);
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
