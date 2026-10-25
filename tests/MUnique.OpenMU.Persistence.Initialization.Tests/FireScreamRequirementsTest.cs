// <copyright file="FireScreamRequirementsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the energy and leadership requirements of the fire scream skill, which are 150 and 70.
/// </summary>
[TestFixture]
internal class FireScreamRequirementsTest
{
    /// <summary>
    /// Tests that a new season 6 database requires 150 energy and 70 leadership for fire scream.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasCorrectRequirementsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var fireScream = GetFireScream(gameConfiguration);

        Assert.That(GetRequirement(fireScream, Stats.TotalEnergy).MinimumValue, Is.EqualTo(150));
        Assert.That(GetRequirement(fireScream, Stats.TotalLeadership).MinimumValue, Is.EqualTo(70));
    }

    /// <summary>
    /// Tests that the update swaps the requirements of an existing database back, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateCorrectsSwappedRequirementsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var fireScream = GetFireScream(gameConfiguration);
        var energy = GetRequirement(fireScream, Stats.TotalEnergy);
        var leadership = GetRequirement(fireScream, Stats.TotalLeadership);
        energy.MinimumValue = 70;
        leadership.MinimumValue = 150;

        var update = new FixFireScreamRequirementsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(energy.MinimumValue, Is.EqualTo(150));
        Assert.That(leadership.MinimumValue, Is.EqualTo(70));
    }

    /// <summary>
    /// Tests that the update keeps requirements which were changed on purpose.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomRequirementsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var fireScream = GetFireScream(gameConfiguration);
        var energy = GetRequirement(fireScream, Stats.TotalEnergy);
        var leadership = GetRequirement(fireScream, Stats.TotalLeadership);
        energy.MinimumValue = 100;
        leadership.MinimumValue = 150;

        await new FixFireScreamRequirementsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(energy.MinimumValue, Is.EqualTo(100));
        Assert.That(leadership.MinimumValue, Is.EqualTo(150));
    }

    private static Skill GetFireScream(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Skills.Single(skill => skill.Number == (short)SkillNumber.FireScream);
    }

    private static AttributeRequirement GetRequirement(Skill skill, AttributeDefinition stat)
    {
        return skill.Requirements.Single(requirement => requirement.Attribute == stat);
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
