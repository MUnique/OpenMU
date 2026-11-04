// <copyright file="CraftingBonusPercentageAttributeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests that the <see cref="Stats.CraftingBonusPercentage"/> attribute is available in the configuration.
/// </summary>
[TestFixture]
internal class CraftingBonusPercentageAttributeTest
{
    /// <summary>
    /// Tests that a new database contains the attribute.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasCraftingBonusPercentageAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.CraftingBonusPercentage.Id), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the update adds the attribute to a database which was created before it existed,
    /// and that applying it twice doesn't add it twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsCraftingBonusPercentageAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.CraftingBonusPercentage.Id));

        var update = new AddCraftingBonusPercentageAttributePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Attributes.Count(a => a.Id == Stats.CraftingBonusPercentage.Id), Is.EqualTo(1));
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
