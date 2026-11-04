// <copyright file="ItemPriceDefinitionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the <see cref="ItemPriceDefinition"/>s of the initialized data, and the update which adds them to existing databases.
/// </summary>
[TestFixture]
internal class ItemPriceDefinitionsTest
{
    /// <summary>
    /// Tests that a new database assigns the price definitions to the special items.
    /// </summary>
    /// <param name="version">The version of the data initialization.</param>
    /// <returns>The task.</returns>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task NewDatabaseHasPriceDefinitionsAsync(string version)
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(version).ConfigureAwait(false);
        using var _ = context;

        var jewelOfBless = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 13 });
        Assert.That(jewelOfBless.PriceDefinition, Is.Not.Null);
        Assert.That(jewelOfBless.PriceDefinition!.BasePriceFormula, Is.EqualTo("9000000"));
        Assert.That(jewelOfBless.PriceDefinition.CraftingReferencePrice, Is.EqualTo(100_000));
        Assert.That(gameConfiguration.ItemPriceDefinitions, Does.Contain(jewelOfBless.PriceDefinition));
        Assert.That(gameConfiguration.ItemPriceDefinitions.Select(d => d.Name.ValueInNeutralLanguage), Is.Unique);
    }

    /// <summary>
    /// Tests that the update assigns the same price definitions to the items of a database which was created before
    /// they existed, as a new database has, and that applying it twice doesn't add anything twice.
    /// </summary>
    /// <param name="version">The version of the data initialization.</param>
    /// <returns>The task.</returns>
    [TestCase("075")]
    [TestCase("095d")]
    [TestCase("Season6")]
    public async Task UpdateAddsPriceDefinitionsToExistingDatabaseAsync(string version)
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync(version).ConfigureAwait(false);
        using var _ = context;
        var expectedPrices = GetPriceDefinitionNames(gameConfiguration);
        var expectedDefinitionCount = gameConfiguration.ItemPriceDefinitions.Count;
        foreach (var item in gameConfiguration.Items)
        {
            item.PriceDefinition = null;
        }

        gameConfiguration.ItemPriceDefinitions.Clear();

        AddItemPriceDefinitionsPlugInBase update = version switch
        {
            "075" => new AddItemPriceDefinitionsPlugIn075(),
            "095d" => new AddItemPriceDefinitionsPlugIn095d(),
            _ => new AddItemPriceDefinitionsPlugInSeason6(),
        };
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(GetPriceDefinitionNames(gameConfiguration), Is.EquivalentTo(expectedPrices));
        Assert.That(gameConfiguration.ItemPriceDefinitions, Has.Count.EqualTo(expectedDefinitionCount));
    }

    /// <summary>
    /// Tests that the update keeps a price definition which a server owner assigned to an item.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task UpdateKeepsExistingPriceDefinitionAsync()
    {
        var (context, gameConfiguration) = await CreateConfigurationAsync("Season6").ConfigureAwait(false);
        using var _ = context;
        var jewelOfBless = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 13 });
        var customPrice = context.CreateNew<ItemPriceDefinition>();
        customPrice.Name = "Custom";
        customPrice.BasePriceFormula = "1";
        gameConfiguration.ItemPriceDefinitions.Add(customPrice);
        jewelOfBless.PriceDefinition = customPrice;

        await new AddItemPriceDefinitionsPlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(jewelOfBless.PriceDefinition, Is.SameAs(customPrice));
    }

    private static Dictionary<string, string?> GetPriceDefinitionNames(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.ToDictionary(item => item.ToString(), item => item.PriceDefinition?.Name.ValueInNeutralLanguage);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateConfigurationAsync(string version)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        DataInitializationBase dataInitialization = version switch
        {
            "075" => new Version075.DataInitialization(contextProvider, NullLoggerFactory.Instance),
            "095d" => new Version095d.DataInitialization(contextProvider, NullLoggerFactory.Instance),
            _ => new VersionSeasonSix.DataInitialization(contextProvider, NullLoggerFactory.Instance),
        };
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        return (context, gameConfiguration);
    }
}
