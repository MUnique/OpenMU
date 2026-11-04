// <copyright file="PlayerShopCurrenciesDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlayerShopCurrencies;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the initial configuration of the player shop currencies.
/// </summary>
[TestFixture]
internal class PlayerShopCurrenciesDataTest
{
    /// <summary>
    /// Tests that a new database contains the deactivated plugin, configured with the jewels as price items,
    /// which are referenced instead of being copied.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsJewelsAsPriceItemsAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        var plugInConfiguration = gameConfiguration.PlugInConfigurations.Single(c => c.TypeId == typeof(PlayerShopCurrenciesFeaturePlugIn).GUID);
        var json = plugInConfiguration.CustomConfiguration;
        TestContext.Out.WriteLine(json);

        Assert.That(plugInConfiguration.IsActive, Is.False);
        Assert.That(json, Does.Contain("\"PriceItems\""));
        Assert.That(json, Does.Not.Contain("\"Name\""), "The items should be referenced instead of being serialized completely.");
        Assert.That(json!.Split("\"$ref\"").Length - 1, Is.EqualTo(5), "Jewels of Bless, Soul, Chaos, Creation and Life");
    }
}
