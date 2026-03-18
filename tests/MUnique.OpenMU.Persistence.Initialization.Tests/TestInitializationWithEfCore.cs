// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// The main program class.
/// </summary>
[TestFixture]
internal class TestInitializationWithEfCore
{
    private const byte IcarusMapNumber = 10;
    private static readonly Guid FeatherDropGroupId = new(0x200, IcarusMapNumber, 1, 0, 0, 0, 0, 0, 0, 0, 0);
    private static readonly Guid CrestDropGroupId = new(0x200, IcarusMapNumber, 2, 0, 0, 0, 0, 0, 0, 0, 0);

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
        await this.AssertIcarusFeatherAndCrestDropGroupsAsync(contextProvider).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that applying the update for Crest of Monarch in Season 6 is idempotent.
    /// </summary>
    [Test]
    public async Task TestSeason6CrestOfMonarchUpdatePlugInAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var map = gameConfiguration.Maps.First(m => m.Number == IcarusMapNumber && m.Discriminator == 0);

        if (gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == CrestDropGroupId) is { } existingCrestGroup)
        {
            map.DropItemGroups.Remove(existingCrestGroup);
            gameConfiguration.DropItemGroups.Remove(existingCrestGroup);
        }

        var update = new AddCrestOfMonarchDropGroupUpdateSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var groups = gameConfiguration.DropItemGroups.Where(group => group.GetId() == CrestDropGroupId).ToList();
        Assert.That(groups, Has.Count.EqualTo(1));
        Assert.That(map.DropItemGroups.Count(group => group.GetId() == CrestDropGroupId), Is.EqualTo(1));
        Assert.That(groups[0].Chance, Is.EqualTo(0.001));
        Assert.That(groups[0].MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(groups[0].ItemLevel, Is.EqualTo((byte)1));
        Assert.That(groups[0].PossibleItems, Has.Count.EqualTo(1));
        Assert.That(groups[0].PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(groups[0].PossibleItems.Single().Number, Is.EqualTo((short)14));
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

    private async Task AssertIcarusFeatherAndCrestDropGroupsAsync(IPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewConfigurationContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var map = gameConfiguration.Maps.First(m => m.Number == IcarusMapNumber && m.Discriminator == 0);

        var featherGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == FeatherDropGroupId);
        var crestGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == CrestDropGroupId);

        Assert.That(map.DropItemGroups.Count(group => group.GetId() == FeatherDropGroupId), Is.EqualTo(1));
        Assert.That(map.DropItemGroups.Count(group => group.GetId() == CrestDropGroupId), Is.EqualTo(1));

        Assert.That(featherGroup.Chance, Is.EqualTo(0.001));
        Assert.That(featherGroup.MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(featherGroup.ItemLevel, Is.Null);
        Assert.That(featherGroup.PossibleItems, Has.Count.EqualTo(1));
        Assert.That(featherGroup.PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(featherGroup.PossibleItems.Single().Number, Is.EqualTo((short)14));

        Assert.That(crestGroup.Chance, Is.EqualTo(0.001));
        Assert.That(crestGroup.MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(crestGroup.ItemLevel, Is.EqualTo((byte)1));
        Assert.That(crestGroup.PossibleItems, Has.Count.EqualTo(1));
        Assert.That(crestGroup.PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(crestGroup.PossibleItems.Single().Number, Is.EqualTo((short)14));
    }
}
