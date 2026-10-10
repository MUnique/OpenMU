// <copyright file="DragonSlasherPveMultiplierTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that Dragon Slasher has its PvE multiplier (<see cref="Stats.SkillFinalMultiplierPvm"/>) in the season 6 data.
/// </summary>
[TestFixture]
internal class DragonSlasherPveMultiplierTest
{
    private const short DragonSlasherNumber = 265;

    private const float ExpectedPveMultiplier = 3.0f;

    /// <summary>
    /// Tests that a new season 6 database configures the PvE multiplier of Dragon Slasher.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasDragonSlasherPveMultiplierAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        AssertPveMultiplier(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the attribute and the relationship to a database which was created before they existed,
    /// and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsDragonSlasherPveMultiplierAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var dragonSlasher = GetDragonSlasher(gameConfiguration);
        foreach (var relationship in dragonSlasher.AttributeRelationships.Where(r => r.TargetAttribute == Stats.SkillFinalMultiplierPvm).ToList())
        {
            dragonSlasher.AttributeRelationships.Remove(relationship);
        }

        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.SkillFinalMultiplierPvm.Id));

        var update = new AddDragonSlasherPveMultiplierPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertPveMultiplier(gameConfiguration);
    }

    private static void AssertPveMultiplier(GameConfiguration gameConfiguration)
    {
        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.SkillFinalMultiplierPvm.Id), Is.EqualTo(1));
        var relationship = GetDragonSlasher(gameConfiguration).AttributeRelationships.Single(r => r.TargetAttribute == Stats.SkillFinalMultiplierPvm);
        Assert.That(relationship.InputAttribute, Is.EqualTo(Stats.SkillMultiplier));
        Assert.That(relationship.InputOperand, Is.EqualTo(ExpectedPveMultiplier));
    }

    private static Skill GetDragonSlasher(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Skills.Single(skill => skill.Number == DragonSlasherNumber);
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
