// <copyright file="GensRewardItemsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the jewellery cases of the monthly gens rewards.
/// </summary>
[TestFixture]
internal class GensRewardItemsTest
{
    private static readonly short[] CaseNumbers = [141, 142, 143, 144];

    /// <summary>
    /// Tests that a new season 6 database has the jewellery cases, which give a jewel or money.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasTheJewelleryCasesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        foreach (var number in CaseNumbers)
        {
            var item = gameConfiguration.Items.Single(definition => definition is { Group: 14 } && definition.Number == number);
            Assert.That(item.DropItems.Single(group => group.ItemType == SpecialItemType.RandomItem).PossibleItems, Has.Count.EqualTo(6), item.Name);
            Assert.That(item.DropItems.Single(group => group.ItemType == SpecialItemType.Money).MoneyAmount, Is.GreaterThan(0), item.Name);
        }
    }

    /// <summary>
    /// Tests that the update adds the jewellery cases to an existing database, and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsTheJewelleryCasesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var item in gameConfiguration.Items.Where(definition => definition.Group == 14 && CaseNumbers.Contains(definition.Number)).ToList())
        {
            gameConfiguration.Items.Remove(item);
        }

        var update = new AddGensRewardItemsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        foreach (var number in CaseNumbers)
        {
            Assert.That(gameConfiguration.Items.Count(definition => definition.Group == 14 && definition.Number == number), Is.EqualTo(1));
        }
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
