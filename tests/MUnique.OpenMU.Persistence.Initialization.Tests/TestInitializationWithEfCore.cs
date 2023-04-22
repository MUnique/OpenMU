// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// The main program class.
/// </summary>
[TestFixture]
internal class TestInitializationWithEfCore
{
    /// <summary>
    /// Tests the data initialization using the entity framework core.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically.")]
    public async Task SetupDatabaseAndTestLoadingDataAsync()
    {
        var manager = new PersistenceContextProvider(new NullLoggerFactory(), null);
        using var update = await manager.ReCreateDatabaseAsync().ConfigureAwait(false);
        await this.TestDataInitializationAsync(new PersistenceContextProvider(new NullLoggerFactory(), null)).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task TestDataInitializationInMemoryAsync()
    {
        await this.TestDataInitializationAsync(new InMemoryPersistenceContextProvider()).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task TestSeason6DataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task Test075DataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version075.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task Test095dDataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version095d.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    private async Task TestDataInitializationAsync(IPersistenceContextProvider contextProvider)
    {
        var initialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await initialization.CreateInitialDataAsync(3, true).ConfigureAwait(false);

        // Loading game configuration
        using var context = contextProvider.CreateNewConfigurationContext();
        var gameConfiguraton = (await context.GetAsync<DataModel.Configuration.GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
        Assert.That(gameConfiguraton, Is.Not.Null);

        // Testing loading of an account
        using var accountContext = contextProvider.CreateNewPlayerContext(gameConfiguraton!);
        var account1 = await accountContext.GetAccountByLoginNameAsync("test1", "test1").ConfigureAwait(false);
        Assert.That(account1, Is.Not.Null);
        Assert.That(account1!.LoginName, Is.EqualTo("test1"));
    }

    private async Task TestIfItemsFitIntoInventoriesAsync(IPersistenceContextProvider contextProvider)
    {
        using var configContext = contextProvider.CreateNewConfigurationContext();
        var config = (await configContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        using var context = contextProvider.CreateNewPlayerContext(config);
        var characters = (await context.GetAccountsOrderedByLoginNameAsync(0, 100).ConfigureAwait(false)).SelectMany(a => a.Characters).ToList();
        Assert.That(characters, Is.Not.Empty);
        byte inventorySize = (byte)(InventoryConstants.EquippableSlotsCount + 64);
        foreach (var character in characters)
        {
            try
            {
                var storage = character.Inventory!;
                var inventory = new Storage(inventorySize, InventoryConstants.EquippableSlotsCount, 0, storage);
                Assert.That(inventory.Items.Count(), Is.EqualTo(storage.Items.Count));
            }
            catch (Exception ex)
            {
                Assert.Warn($"{ex.Message} Character: {character.Name}");
            }
        }
    }
}