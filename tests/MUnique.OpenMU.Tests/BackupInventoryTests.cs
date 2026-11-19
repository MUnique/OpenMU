// <copyright file="BackupInventoryTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for the handling of the <see cref="Player.BackupInventory"/> when a player leaves the game.
/// </summary>
[TestFixture]
public class BackupInventoryTests
{
    /// <summary>
    /// Verifies that a backup of a crafting dialog which was closed without crafting isn't restored
    /// when the player leaves the game: sold items stay away and new items are kept.
    /// </summary>
    [Test]
    public async ValueTask OutdatedBackupIsNotRestoredWhenLeavingAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var selectedCharacter = player.SelectedCharacter!;
        var soldItem = CreateItem(CreateDefinition());
        var keptItem = CreateItem(CreateDefinition());
        await player.Inventory!.AddItemAsync(20, soldItem).ConfigureAwait(false);
        await player.Inventory.AddItemAsync(21, keptItem).ConfigureAwait(false);

        // This is what happens when the player talks to a npc which offers craftings.
        player.BackupInventory = new BackupItemStorage(player.Inventory.ItemStorage);

        // Afterwards, the player sells one item and gets another one.
        await player.Inventory.RemoveItemAsync(soldItem).ConfigureAwait(false);
        var newItem = CreateItem(CreateDefinition());
        await player.Inventory.AddItemAsync(22, newItem).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(selectedCharacter).ConfigureAwait(false);

        Assert.That(player.BackupInventory, Is.Null);
        Assert.That(player.Inventory!.Items, Does.Not.Contain(soldItem));
        Assert.That(player.Inventory.GetItem(21), Is.SameAs(keptItem));
        Assert.That(player.Inventory.GetItem(22), Is.SameAs(newItem));
    }

    /// <summary>
    /// Verifies that the backup is still restored when items are waiting in the temporary storage,
    /// which is the case when the player disconnects during a crafting.
    /// </summary>
    [Test]
    public async ValueTask BackupIsRestoredWhenItemsAreInTemporaryStorageAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var selectedCharacter = player.SelectedCharacter!;
        var craftingItem = CreateItem(CreateDefinition());
        await player.Inventory!.AddItemAsync(20, craftingItem).ConfigureAwait(false);
        player.BackupInventory = new BackupItemStorage(player.Inventory.ItemStorage);

        // The player moves the item into the crafting dialog.
        await player.Inventory.RemoveItemAsync(craftingItem).ConfigureAwait(false);
        await player.TemporaryStorage!.AddItemAsync(0, craftingItem).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(selectedCharacter).ConfigureAwait(false);

        Assert.That(player.BackupInventory, Is.Null);
        Assert.That(player.Inventory!.GetItem(20), Is.SameAs(craftingItem));
    }

    /// <summary>
    /// Verifies that the backup is still restored when a trade is in progress, because the
    /// items which were offered are only in the trade storage of the player.
    /// </summary>
    [Test]
    public async ValueTask BackupIsRestoredWhenATradeIsInProgressAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var selectedCharacter = player.SelectedCharacter!;
        var tradedItem = CreateItem(CreateDefinition());
        await player.Inventory!.AddItemAsync(20, tradedItem).ConfigureAwait(false);
        player.BackupInventory = new BackupItemStorage(player.Inventory.ItemStorage);
        player.TradingPartner = new Mock<ITrader>().Object;

        // The offered item is not in the inventory anymore.
        await player.Inventory.RemoveItemAsync(tradedItem).ConfigureAwait(false);

        await player.RemoveFromGameAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(selectedCharacter).ConfigureAwait(false);

        Assert.That(player.BackupInventory, Is.Null);
        Assert.That(player.Inventory!.GetItem(20), Is.SameAs(tradedItem));
    }

    private static async ValueTask<Player> CreateTestPlayerAsync()
    {
        var gameConfig = new Mock<GameConfiguration>();
        gameConfig.SetupAllProperties();
        gameConfig.Setup(c => c.Maps).Returns(new List<GameMapDefinition>());
        gameConfig.Setup(c => c.Items).Returns(new List<ItemDefinition>());
        gameConfig.Setup(c => c.Skills).Returns(new List<Skill>());
        gameConfig.Setup(c => c.PlugInConfigurations).Returns(new List<PlugInConfiguration>());
        gameConfig.Setup(c => c.CharacterClasses).Returns(new List<CharacterClass>());
        gameConfig.Setup(c => c.Attributes).Returns(new List<AttributeDefinition>());
        gameConfig.Setup(c => c.GlobalAttributeCombinations).Returns(new List<AttributeRelationship>());
        gameConfig.Setup(c => c.GlobalBaseAttributeValues).Returns(new List<ConstValueAttribute>
        {
            new(1, Stats.MoneyAmountRate),
        });
        var map = new Mock<GameMapDefinition>();
        map.SetupAllProperties();
        map.Setup(m => m.DropItemGroups).Returns(new List<DropItemGroup>());
        map.Setup(m => m.MonsterSpawns).Returns(new List<MonsterSpawnArea>());
        map.Object.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfig.Object.RecoveryInterval = int.MaxValue;
        gameConfig.Object.Maps.Add(map.Object);

        var mapInitializer = new MapInitializer(gameConfig.Object, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(
            gameConfig.Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(null, new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        return await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
    }

    private static ItemDefinition CreateDefinition()
    {
        return new ItemDefinition
        {
            Width = 1,
            Height = 1,
            Durability = 1,
        };
    }

    private static Item CreateItem(ItemDefinition definition)
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        item.Object.Durability = 1;
        return item.Object;
    }
}
