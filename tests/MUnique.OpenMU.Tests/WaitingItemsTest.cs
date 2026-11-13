// <copyright file="WaitingItemsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.WaitingItems;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using BasicItem = MUnique.OpenMU.Persistence.BasicModel.Item;

/// <summary>
/// Tests the waiting items: items which find no place when their storage is loaded wait for space in the inventory.
/// </summary>
[TestFixture]
public class WaitingItemsTest
{
    private const byte FirstInventorySlot = 12;

    private const int InventoryCells = 64;

    /// <summary>
    /// Tests that a storage passes an item which finds no free place to the given action and removes it from the item storage.
    /// </summary>
    [Test]
    public void StoragePutsAsideItemWithoutPlace()
    {
        var smallItems = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var grownItem = CreateItem(2, 2, 0);
        var itemStorage = CreateItemStorage([.. smallItems, grownItem]);
        var putAside = new List<Item>();

        var storage = new Storage(16, 0, 0, itemStorage, TakeInto(putAside));

        Assert.That(putAside, Is.EqualTo(new[] { grownItem }));
        Assert.That(itemStorage.Items, Does.Not.Contain(grownItem));
        Assert.That(storage.Items, Is.EquivalentTo(smallItems));
    }

    /// <summary>
    /// Tests that an item which finds no free place stays in the item storage when the action throws, so it isn't lost.
    /// </summary>
    [Test]
    public void ItemStaysWhenPuttingAsideFails()
    {
        var grownItem = CreateItem(2, 2, 0);
        var itemStorage = CreateItemStorage([.. Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)), grownItem]);

        Assert.That(() => new Storage(16, 0, 0, itemStorage, _ => throw new InvalidOperationException()), Throws.InvalidOperationException);
        Assert.That(itemStorage.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that the personal store passes an item which finds no free place to the given action
    /// and removes it from the inventory of the character.
    /// </summary>
    [Test]
    public void ShopStoragePutsAsideItemWithoutPlace()
    {
        var character = PlayerTestHelper.CreateCharacter(GameContextTestHelper.CreateGameContext());
        var firstSlot = InventoryConstants.FirstStoreItemSlotIndex;
        var smallItems = Enumerable.Range(firstSlot, InventoryConstants.StoreSize).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var grownItem = CreateItem(2, 2, firstSlot);
        smallItems.ForEach(character.Inventory!.Items.Add);
        character.Inventory.Items.Add(grownItem);
        var putAside = new List<Item>();

        var store = new ShopStorage(character, TakeInto(putAside));

        Assert.That(putAside, Is.EqualTo(new[] { grownItem }));
        Assert.That(character.Inventory.Items, Does.Not.Contain(grownItem));
        Assert.That(store.Items, Is.EquivalentTo(smallItems));
    }

    /// <summary>
    /// Tests that a storage without such an action still throws when an item finds no free place.
    /// </summary>
    [Test]
    public void StorageWithoutActionThrows()
    {
        var items = Enumerable.Range(0, 16).Select(slot => CreateItem(1, 1, (byte)slot)).Append(CreateItem(2, 2, 0)).ToArray();

        Assert.That(() => new Storage(16, 0, 0, CreateItemStorage(items)), Throws.TypeOf<ArgumentException>());
    }

    /// <summary>
    /// Tests that a character, whose inventory has an item which grew and finds no place, enters the game,
    /// and that the item waits for space in the inventory.
    /// </summary>
    [Test]
    public async ValueTask GrownItemWaitsWhenCharacterEntersAsync()
    {
        var (player, grownItem, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);

        var waitingItem = player.SelectedCharacter!.WaitingItems.Single();
        Assert.That(waitingItem.Item, Is.SameAs(grownItem));
        Assert.That(waitingItem.Reason, Is.EqualTo(WaitingItemReason.NoSpaceWhenLoaded));
        Assert.That(player.SelectedCharacter.Inventory!.Items, Does.Not.Contain(grownItem));
        Assert.That(player.Inventory!.Items.Count(), Is.EqualTo(InventoryCells));
    }

    /// <summary>
    /// Tests that an inventory item which finds no place stays in the inventory of the character when putting it
    /// aside fails, so it isn't lost when the character is saved.
    /// </summary>
    [Test]
    public async ValueTask InventoryItemStaysWhenPuttingAsideFailsAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var character = PlayerTestHelper.CreateCharacter(gameContext);
        Mock.Get(character).Setup(c => c.WaitingItems).Returns((ICollection<WaitingItem>)null!);
        var grownItem = CreateItem(2, 2, FirstInventorySlot);
        Enumerable.Range(FirstInventorySlot, InventoryCells).Select(slot => CreateItem(1, 1, (byte)slot)).ToList().ForEach(character.Inventory!.Items.Add);
        character.Inventory.Items.Add(grownItem);

        Assert.That(async () => await PlayerTestHelper.EnterWorldAsync(player, character).ConfigureAwait(false), Throws.Exception);
        Assert.That(character.Inventory.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that an item which finds no place in the main inventory is put into an inventory extension
    /// instead of waiting, when the extension has space.
    /// </summary>
    [Test]
    public async ValueTask GrownItemIsPutIntoExtensionAsync()
    {
        var (player, grownItem, _) = await CreatePlayerWithGrownItemAsync(1).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.WaitingItems, Is.Empty);
        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
        Assert.That(grownItem.ItemSlot, Is.EqualTo(InventoryConstants.FirstExtensionItemSlotIndex));
        Assert.That(player.SelectedCharacter.Inventory!.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that an item in the vault, which finds no place when the vault is opened, waits for space in the inventory.
    /// </summary>
    [Test]
    public async ValueTask GrownVaultItemWaitsWhenVaultIsOpenedAsync()
    {
        var (player, _, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false), Is.True);
        var grownVaultItem = CreateItem(2, 2, 0);
        var vault = CreateItemStorage([.. Enumerable.Range(0, InventoryConstants.WarehouseSize).Select(slot => CreateItem(1, 1, (byte)slot)), grownVaultItem]);
        Mock.Get(player.Account!).Setup(a => a.Vault).Returns(vault);
        var npc = new NonPlayerCharacter(null!, new Persistence.BasicModel.MonsterDefinition { NpcWindow = NpcWindow.VaultStorage }, null!);

        await new TalkNpcAction().TalkToNpcAsync(player, npc).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.WaitingItems.Select(w => w.Item), Does.Contain(grownVaultItem));
        Assert.That(vault.Items, Does.Not.Contain(grownVaultItem));
        Assert.That(player.Vault!.Items.Count(), Is.EqualTo(InventoryConstants.WarehouseSize));
    }

    /// <summary>
    /// Tests that a waiting item stays in the waiting items while it still doesn't fit.
    /// </summary>
    [Test]
    public async ValueTask WaitingItemStaysWithoutSpaceAsync()
    {
        var (player, grownItem, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);

        var delivered = await player.DeliverWaitingItemsAsync().ConfigureAwait(false);

        Assert.That(delivered, Is.Zero);
        Assert.That(player.SelectedCharacter!.WaitingItems.Single().Item, Is.SameAs(grownItem));
        Assert.That(player.Inventory!.Items, Does.Not.Contain(grownItem));
    }

    /// <summary>
    /// Tests that a waiting item is put into the inventory as soon as it fits there, and that the player is told.
    /// </summary>
    [Test]
    public async ValueTask WaitingItemIsDeliveredWhenItFitsAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);

        var delivered = await player.DeliverWaitingItemsAsync().ConfigureAwait(false);

        Assert.That(delivered, Is.EqualTo(1));
        Assert.That(player.SelectedCharacter!.WaitingItems, Is.Empty);
        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
        Assert.That(player.SelectedCharacter.Inventory!.Items, Does.Contain(grownItem));
        Assert.That(grownItem.ItemSlot, Is.EqualTo(FirstInventorySlot));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()!).Verify(p => p.ItemAppearAsync(grownItem), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(p => p.ShowMessageAsync("Scroll of Blood +3 (2x2) was put into your inventory.", MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// Tests that a smaller waiting item is delivered even when an older, bigger one doesn't fit yet.
    /// </summary>
    [Test]
    public async ValueTask SmallerItemIsDeliveredBeforeBiggerOneAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        var smallWaitingItem = CreateItem(1, 1, 0);
        character.AddWaitingItem(player.PersistenceContext, smallWaitingItem, WaitingItemReason.NoSpaceWhenLoaded);
        character.WaitingItems.Last().AddedAt = character.WaitingItems.First().AddedAt.AddMinutes(1);
        await RemoveSmallItemsAsync(player, smallItems, 75).ConfigureAwait(false);

        var delivered = await player.DeliverWaitingItemsAsync().ConfigureAwait(false);

        Assert.That(delivered, Is.EqualTo(1));
        Assert.That(player.Inventory!.Items, Does.Contain(smallWaitingItem));
        Assert.That(character.WaitingItems.Single().Item, Is.SameAs(grownItem));
    }

    /// <summary>
    /// Tests that a waiting item without an item definition stays, and doesn't hold back the other waiting items.
    /// </summary>
    [Test]
    public async ValueTask ItemWithoutDefinitionStaysAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        var waitingItem = character.WaitingItems.Single();
        var definition = grownItem.Definition;
        grownItem.Definition = null;
        var smallWaitingItem = CreateItem(1, 1, 0);
        character.AddWaitingItem(player.PersistenceContext, smallWaitingItem, WaitingItemReason.NoSpaceWhenLoaded);
        character.WaitingItems.Last().AddedAt = waitingItem.AddedAt.AddMinutes(1);
        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);

        var delivered = await player.DeliverWaitingItemsAsync().ConfigureAwait(false);

        Assert.That(delivered, Is.EqualTo(1));
        Assert.That(player.Inventory!.Items, Does.Contain(smallWaitingItem));
        Assert.That(character.WaitingItems.Single(), Is.SameAs(waitingItem));
        Assert.That(waitingItem.Item, Is.SameAs(grownItem));
        Assert.That(player.HasDeliverableWaitingItems(), Is.False);

        grownItem.Definition = definition;
        Assert.That(player.HasDeliverableWaitingItems(), Is.True);
    }

    /// <summary>
    /// Tests that the delivery plug-in tells the player about the waiting items when the character entered the game,
    /// and puts them into the inventory when they fit.
    /// </summary>
    [Test]
    public async ValueTask PlugInDeliversWhenCharacterEnteredAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var plugIn = new WaitingItemsDeliveryPlugIn();
        var messages = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);

        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        messages.Verify(p => p.ShowMessageAsync("1 item(s) wait for space in your inventory. They are put into it as soon as they fit:", MessageType.BlueNormal), Times.Once);
        messages.Verify(p => p.ShowMessageAsync("- Scroll of Blood +3 (2x2)", MessageType.BlueNormal), Times.Once);
        Assert.That(player.SelectedCharacter!.WaitingItems, Has.Count.EqualTo(1));

        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);
        await plugIn.PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
        Assert.That(player.SelectedCharacter.WaitingItems, Is.Empty);
    }

    /// <summary>
    /// Tests that the periodic delivery puts a waiting item into the inventory when it fits,
    /// but only while no dialog is open.
    /// </summary>
    [Test]
    public async ValueTask PeriodicDeliveryOnlyWithoutDialogAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var gameContext = (GameContext)player.GameContext;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);
        var plugIn = new WaitingItemsDeliveryPlugIn();

        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.NpcDialogOpened).ConfigureAwait(false), Is.True);
        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        Assert.That(player.Inventory!.Items, Does.Not.Contain(grownItem));

        Assert.That(await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false), Is.True);
        plugIn.ForceStart();
        await plugIn.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that the periodic delivery waits while a purchase in the personal store holds its lock.
    /// </summary>
    [Test]
    public async ValueTask PeriodicDeliveryWaitsForStoreLockAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var gameContext = (GameContext)player.GameContext;
        await gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);

        Task delivery;
        using (await player.ShopStorage!.StoreLock.LockAsync().ConfigureAwait(false))
        {
            delivery = new WaitingItemsDeliveryPlugIn().ExecuteTaskAsync(gameContext).AsTask();
            await Task.Delay(100).ConfigureAwait(false);
            Assert.That(delivery.IsCompleted, Is.False);
            Assert.That(player.Inventory!.Items, Does.Not.Contain(grownItem));
        }

        await delivery.ConfigureAwait(false);
        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that the delivery after opening the vault waits while a purchase in the personal store holds its lock.
    /// </summary>
    [Test]
    public async ValueTask VaultDeliveryWaitsForStoreLockAsync()
    {
        var (player, grownItem, smallItems) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        await RemoveSmallItemsAsync(player, smallItems, 12, 13, 20, 21).ConfigureAwait(false);

        Task delivery;
        using (await player.ShopStorage!.StoreLock.LockAsync().ConfigureAwait(false))
        {
            delivery = new WaitingItemsDeliveryPlugIn().DeliverAndShowAsync(player).AsTask();
            await Task.Delay(100).ConfigureAwait(false);
            Assert.That(delivery.IsCompleted, Is.False);
        }

        await delivery.ConfigureAwait(false);
        Assert.That(player.Inventory!.Items, Does.Contain(grownItem));
    }

    /// <summary>
    /// Tests that an error in the delivery when the character entered the game is logged instead of thrown,
    /// so that it doesn't stop entering the game.
    /// </summary>
    [Test]
    public async ValueTask ErrorWhenCharacterEnteredDoesntThrowAsync()
    {
        var (player, _, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!)
            .Setup(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()))
            .ThrowsAsync(new InvalidOperationException());

        await new WaitingItemsDeliveryPlugIn().PlayerStateChangedAsync(player, PlayerState.CharacterSelection, PlayerState.EnteredWorld).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter!.WaitingItems, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that a character can't be deleted while items wait for it, because an item which found no place
    /// in the vault belongs to the account and would be deleted with the character.
    /// </summary>
    [Test]
    public async ValueTask CharacterWithWaitingItemsIsNotDeletedAsync()
    {
        var (player, _, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        character.Name = "Waiting";
        var accountMock = Mock.Get(player.Account!);
        accountMock.Setup(a => a.Characters).Returns(new List<Character> { character });
        player.Account!.SecurityCode = "1234";
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);

        await new DeleteCharacterAction().DeleteCharacterAsync(player, character.Name, "1234").ConfigureAwait(false);

        Assert.That(player.Account!.Characters, Does.Contain(character));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowCharacterDeleteResponsePlugIn>()!).Verify(p => p.ShowCharacterDeleteResponseAsync(CharacterDeleteResult.Unsuccessful), Times.Once);
    }

    /// <summary>
    /// Tests that a character whose only waiting item has no item definition can't be deleted either,
    /// because the item is kept until the definition is back, and that the player is asked to contact an admin.
    /// </summary>
    [Test]
    public async ValueTask CharacterWithUndeliverableWaitingItemIsNotDeletedAsync()
    {
        var (player, grownItem, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        grownItem.Definition = null;
        var character = player.SelectedCharacter!;
        character.Name = "Waiting";
        Mock.Get(player.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        player.Account!.SecurityCode = "1234";
        await player.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);

        await new DeleteCharacterAction().DeleteCharacterAsync(player, character.Name, "1234").ConfigureAwait(false);

        Assert.That(player.Account.Characters, Does.Contain(character));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!).Verify(p => p.ShowMessageAsync(It.Is<string>(m => m.Contains("admin")), MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// Tests that a long list of waiting items is told in short messages: one per named item,
    /// and the number of the items which aren't named.
    /// </summary>
    [Test]
    public async ValueTask ManyWaitingItemsAreToldInShortMessagesAsync()
    {
        var (player, _, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        for (var i = 0; i < 4; i++)
        {
            player.SelectedCharacter!.AddWaitingItem(player.PersistenceContext, CreateItem(2, 2, 0), WaitingItemReason.NoSpaceWhenLoaded);
        }

        var messages = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);

        await player.ShowWaitingItemsAsync(3, pointToCommand: true).ConfigureAwait(false);

        messages.Verify(p => p.ShowMessageAsync("5 item(s) wait for space in your inventory. They are put into it as soon as they fit:", MessageType.BlueNormal), Times.Once);
        messages.Verify(p => p.ShowMessageAsync(It.Is<string>(m => m.StartsWith("- ")), MessageType.BlueNormal), Times.Exactly(3));
        messages.Verify(p => p.ShowMessageAsync("... and 2 more. /waiting lists more of them.", MessageType.BlueNormal), Times.Once);
    }

    /// <summary>
    /// Tests that the delivery points the player to /waiting for the items which aren't named
    /// only while that chat command is active.
    /// </summary>
    /// <param name="commandActive">If set to <c>true</c>, the chat command is active.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask DeliveryPointsToActiveCommandAsync(bool commandActive)
    {
        var plugInConfigurations = commandActive
            ? null
            : new[] { new PlugInConfiguration { TypeId = typeof(WaitingItemsChatCommandPlugIn).GUID, IsActive = false } };
        var (player, _, _) = await CreatePlayerWithGrownItemAsync(plugInConfigurations: plugInConfigurations).ConfigureAwait(false);
        Assert.That(player.GameContext.PlugInManager.GetStrategy<IChatCommandPlugIn>("/waiting") is not null, Is.EqualTo(commandActive));
        for (var i = 0; i < 4; i++)
        {
            player.SelectedCharacter!.AddWaitingItem(player.PersistenceContext, CreateItem(2, 2, 0), WaitingItemReason.NoSpaceWhenLoaded);
        }

        var messages = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);

        await new WaitingItemsDeliveryPlugIn().DeliverAndShowAsync(player).ConfigureAwait(false);

        messages.Verify(p => p.ShowMessageAsync("... and 2 more. /waiting lists more of them.", MessageType.BlueNormal), Times.Exactly(commandActive ? 1 : 0));
        messages.Verify(p => p.ShowMessageAsync("... and 2 more.", MessageType.BlueNormal), Times.Exactly(commandActive ? 0 : 1));
    }

    /// <summary>
    /// Tests that the player isn't pointed to the chat command when it's not wanted,
    /// e.g. because it's deactivated or it's the chat command itself.
    /// </summary>
    [Test]
    public async ValueTask NotPointedToCommandAsync()
    {
        var (player, _, _) = await CreatePlayerWithGrownItemAsync().ConfigureAwait(false);
        for (var i = 0; i < 4; i++)
        {
            player.SelectedCharacter!.AddWaitingItem(player.PersistenceContext, CreateItem(2, 2, 0), WaitingItemReason.NoSpaceWhenLoaded);
        }

        var messages = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);

        await player.ShowWaitingItemsAsync(3).ConfigureAwait(false);

        messages.Verify(p => p.ShowMessageAsync("... and 2 more.", MessageType.BlueNormal), Times.Once);
        messages.Verify(p => p.ShowMessageAsync(It.Is<string>(m => m.Contains("/waiting")), MessageType.BlueNormal), Times.Never);
    }

    private static async ValueTask<(Player Player, Item GrownItem, List<Item> SmallItems)> CreatePlayerWithGrownItemAsync(int inventoryExtensions = 0, IEnumerable<PlugInConfiguration>? plugInConfigurations = null)
    {
        var gameContext = GameContextTestHelper.CreateGameContext(plugInConfigurations);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var character = PlayerTestHelper.CreateCharacter(gameContext);
        Mock.Get(character).Setup(c => c.WaitingItems).Returns(new List<WaitingItem>());
        character.InventoryExtensions = inventoryExtensions;

        // The small items come first, so the grown item is the one which finds no place.
        var smallItems = Enumerable.Range(FirstInventorySlot, InventoryCells).Select(slot => CreateItem(1, 1, (byte)slot)).ToList();
        var grownItem = CreateItem(2, 2, FirstInventorySlot);
        grownItem.Level = 3;
        grownItem.Definition!.Name = "Scroll of Blood";
        smallItems.ForEach(character.Inventory!.Items.Add);
        character.Inventory.Items.Add(grownItem);

        await PlayerTestHelper.EnterWorldAsync(player, character).ConfigureAwait(false);
        return (player, grownItem, smallItems);
    }

    private static async ValueTask RemoveSmallItemsAsync(Player player, List<Item> smallItems, params int[] slots)
    {
        foreach (var slot in slots)
        {
            await player.Inventory!.RemoveItemAsync(smallItems.First(i => i.ItemSlot == slot)).ConfigureAwait(false);
        }
    }

    private static Item CreateItem(byte width, byte height, byte slot)
    {
        // The items are compared by their identifier.
        return new BasicItem
        {
            Id = Guid.NewGuid(),
            Definition = new ItemDefinition { Width = width, Height = height, Durability = 1 },
            ItemSlot = slot,
            Durability = 1,
        };
    }

    private static Func<Item, bool> TakeInto(List<Item> items)
    {
        return item =>
        {
            items.Add(item);
            return true;
        };
    }

    private static ItemStorage CreateItemStorage(params Item[] items)
    {
        var storage = new Mock<ItemStorage>();
        storage.Setup(s => s.Items).Returns(items.ToList());
        return storage.Object;
    }
}
