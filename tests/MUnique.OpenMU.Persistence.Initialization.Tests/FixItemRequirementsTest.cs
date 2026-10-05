// <copyright file="FixItemRequirementsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the elf bow requirements are correct in the season 6 data,
/// and that the versioned update restores them on existing databases.
/// </summary>
[TestFixture]
internal class FixItemRequirementsTest
{
    /// <summary>
    /// Tests that a new season 6 database has the correct bow requirements.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasCorrectBowRequirementsAsync()
    {
        var (_, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        Assert.That(GetRequirement(gameConfiguration, 4, 0, Stats.TotalStrengthRequirementValue), Is.EqualTo(20));
        Assert.That(GetRequirement(gameConfiguration, 4, 0, Stats.TotalAgilityRequirementValue), Is.EqualTo(80));
        Assert.That(GetRequirement(gameConfiguration, 4, 6, Stats.TotalStrengthRequirementValue), Is.EqualTo(40));
        Assert.That(GetRequirement(gameConfiguration, 4, 6, Stats.TotalAgilityRequirementValue), Is.EqualTo(150));
    }

    /// <summary>
    /// Tests that the update restores the bow requirements on a database which was
    /// created before the base data was corrected, and that applying it twice
    /// doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateRestoresBowRequirementsOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var item in gameConfiguration.Items.Where(item => item.Group == 4 && item.Number <= 6))
        {
            item.Requirements.Clear();
        }

        var update = new FixItemRequirementsPlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(GetRequirement(gameConfiguration, 4, 0, Stats.TotalStrengthRequirementValue), Is.EqualTo(20));
        Assert.That(GetRequirement(gameConfiguration, 4, 0, Stats.TotalAgilityRequirementValue), Is.EqualTo(80));
        var shortBow = gameConfiguration.Items.Single(item => item.Group == 4 && item.Number == 0);
        Assert.That(shortBow.Requirements.Count(r => r.Attribute == Stats.TotalStrengthRequirementValue), Is.EqualTo(1));
    }

    private static double GetRequirement(GameConfiguration gameConfiguration, int group, int number, AttributeDefinition attribute)
    {
        var item = gameConfiguration.Items.Single(i => i.Group == group && i.Number == number);
        return item.Requirements.First(r => r.Attribute == attribute).MinimumValue;
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
