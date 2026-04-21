// <copyright file="MoveItemActionTests.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests for <see cref="MoveItemAction"/>.
/// </summary>
[TestFixture]
public class MoveItemActionTests
{
    [Test]
    public async ValueTask CompleteStackConsumesSourceItemAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var definition = CreateDefinition(1, 1, 10);
        var source = CreateItem(definition, 3);
        var target = CreateItem(definition, 2);

        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);
        await player.Inventory.AddItemAsync(21, target).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 21, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.Null);
        Assert.That(player.Inventory.GetItem(21), Is.SameAs(target));
        Assert.That(target.Durability, Is.EqualTo(5));
        Assert.That(player.Inventory.ItemStorage.Items.Count(i => ReferenceEquals(i, source)), Is.EqualTo(0));
        Assert.That(player.Inventory.ItemStorage.Items.Count(i => ReferenceEquals(i, target)), Is.EqualTo(1));
    }

    [Test]
    public async ValueTask CompleteStackAndRelogKeepsSinglePersistedItemAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var selectedCharacter = player.SelectedCharacter!;
        var definition = CreateDefinition(1, 1, 10);
        var source = CreateItem(definition, 3);
        var target = CreateItem(definition, 2);

        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);
        await player.Inventory.AddItemAsync(21, target).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 21, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.Null);
        Assert.That(player.Inventory.GetItem(21), Is.Not.Null);
        Assert.That(player.Inventory.GetItem(21)!.Durability, Is.EqualTo(5));

        await player.RemoveFromGameAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(selectedCharacter).ConfigureAwait(false);

        var persistedTarget = player.Inventory!.GetItem(21);
        Assert.That(persistedTarget, Is.Not.Null);
        Assert.That(persistedTarget!.Durability, Is.EqualTo(5));
        Assert.That(player.Inventory.GetItem(20), Is.Null);
        Assert.That(player.Inventory.Items.Count(i => i.Definition == definition), Is.EqualTo(1));
    }

    [Test]
    public async ValueTask FailedMoveToOccupiedSlotKeepsSourceAtOriginalSlotAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var source = CreateItem(CreateDefinition(), 1);
        var blocker = CreateItem(CreateDefinition(2, 2), 1);

        await player.Inventory!.AddItemAsync(30, source).ConfigureAwait(false);
        await player.Inventory.AddItemAsync(20, blocker).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 30, Storages.Inventory, 21, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(30), Is.SameAs(source));
        Assert.That(player.Inventory.GetItem(20), Is.SameAs(blocker));
        Assert.That(player.Inventory.GetItem(21), Is.Null);
    }

    [Test]
    public async ValueTask FailedVaultToInventoryMoveKeepsItemInVaultAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var vaultStorage = CreateVaultStorage();
        player.Vault = vaultStorage;
        player.IsVaultLocked = true;

        var source = CreateItem(CreateDefinition(), 1);
        await vaultStorage.AddItemAsync(0, source).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 0, Storages.Vault, 20, Storages.Inventory).ConfigureAwait(false);

        Assert.That(vaultStorage.GetItem(0), Is.SameAs(source));
        Assert.That(player.Inventory!.GetItem(20), Is.Null);
    }

    [Test]
    public async ValueTask MoveToSlotOutsideGridBoundsIsRejectedWithoutMutationAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var source = CreateItem(CreateDefinition(2, 1), 1);

        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 27, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.SameAs(source));
        Assert.That(player.Inventory.GetItem(27), Is.Null);
        Assert.That(source.ItemSlot, Is.EqualTo(20));
    }

    [Test]
    public async ValueTask MoveRequestInInvalidPlayerStateIsRejectedWithoutMutationAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var source = CreateItem(CreateDefinition(), 1);
        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false), Is.True);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 22, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.SameAs(source));
        Assert.That(player.Inventory.GetItem(22), Is.Null);
    }

    [Test]
    public async ValueTask MoveToTradeStorageOutsideTradeIsRejectedWithoutMutationAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var source = CreateItem(CreateDefinition(), 1);
        await player.Inventory!.AddItemAsync(20, source).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.SameAs(source));
        Assert.That(player.TemporaryStorage!.Items, Does.Not.Contain(source));
    }

    [Test]
    public async ValueTask MoveRequestInTradeButtonPressedStateIsRejectedWithoutMutationAsync()
    {
        var trader1 = await CreateTestPlayerAsync().ConfigureAwait(false);
        var trader2 = await CreateTestPlayerAsync().ConfigureAwait(false);
        var tradeRequestAction = new TradeRequestAction();
        var tradeResponseAction = new TradeAcceptAction();
        var tradeButtonAction = new TradeButtonAction();

        var itemInTrade = CreateItem(CreateDefinition(), 1);
        var blockedMoveItem = CreateItem(CreateDefinition(), 1);
        await trader1.Inventory!.AddItemAsync(20, itemInTrade).ConfigureAwait(false);
        await trader1.Inventory.AddItemAsync(21, blockedMoveItem).ConfigureAwait(false);

        await tradeRequestAction.RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await tradeResponseAction.HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(trader1, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
        await tradeButtonAction.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);

        Assert.That(trader1.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));

        await action.MoveItemAsync(trader1, 21, Storages.Inventory, 1, Storages.Trade).ConfigureAwait(false);

        Assert.That(trader1.Inventory.GetItem(21), Is.SameAs(blockedMoveItem));
        Assert.That(trader1.TemporaryStorage!.GetItem(1), Is.Null);
    }

    [Test]
    public async ValueTask LogoutAndRelogAfterInventoryToVaultMoveKeepsSinglePersistedCopyAsync()
    {
        var player = await CreateTestPlayerAsync().ConfigureAwait(false);
        var selectedCharacter = player.SelectedCharacter!;
        var vaultStorage = CreateVaultStorage();
        player.Vault = vaultStorage;
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { NpcWindow = NpcWindow.VaultStorage }, null!);
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false), Is.True);

        var movedItem = CreateItem(CreateDefinition(), 1);
        await player.Inventory!.AddItemAsync(20, movedItem).ConfigureAwait(false);

        var action = new MoveItemAction();
        await action.MoveItemAsync(player, 20, Storages.Inventory, 0, Storages.Vault).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(20), Is.Null);
        Assert.That(vaultStorage.GetItem(0), Is.SameAs(movedItem));

        await player.RemoveFromGameAsync().ConfigureAwait(false);
        await player.SetSelectedCharacterAsync(selectedCharacter).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items.Count(i => ReferenceEquals(i, movedItem)), Is.EqualTo(0));
        Assert.That(vaultStorage.Items.Count(i => ReferenceEquals(i, movedItem)), Is.EqualTo(1));
    }

    private static Storage CreateVaultStorage()
    {
        var itemStorage = new Mock<ItemStorage>();
        itemStorage.Setup(i => i.Items).Returns(new List<Item>());
        return new Storage(InventoryConstants.WarehouseSize, itemStorage.Object);
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

    private static ItemDefinition CreateDefinition(byte width = 1, byte height = 1, byte durability = 1)
    {
        return new ItemDefinition
        {
            Width = width,
            Height = height,
            Durability = durability,
        };
    }

    private static Item CreateItem(ItemDefinition definition, double durability, byte level = 0)
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        item.Object.Durability = durability;
        item.Object.Level = level;
        return item.Object;
    }
}
