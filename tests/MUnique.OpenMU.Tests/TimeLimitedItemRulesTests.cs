// <copyright file="TimeLimitedItemRulesTests.cs" company="MUnique">
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
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the rules for time-limited items: a valid one never leaves the inventory of its owner, and an expired
/// one can only be dropped or sold to an NPC, as far as its item definition allows it.
/// </summary>
[TestFixture]
public class TimeLimitedItemRulesTests
{
    private const byte InventorySlot = 20;

    /// <summary>
    /// Verifies that a time-limited item can't be moved into another storage than the inventory, also when its
    /// item definition allows it: its remaining time mustn't pass to another player, and it's no crafting ingredient.
    /// </summary>
    /// <param name="storage">The target storage.</param>
    [TestCase(Storages.Vault)]
    [TestCase(Storages.Trade)]
    [TestCase(Storages.PersonalStore)]
    [TestCase(Storages.ChaosMachine)]
    public async ValueTask TimeLimitedItemStaysInInventoryAsync(Storages storage)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = CreateItem(DateTime.UtcNow.AddDays(1), _ => { });
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new MoveItemAction().MoveItemAsync(player, InventorySlot, Storages.Inventory, 0, storage).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(InventorySlot), Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that a time-limited item can be moved within the inventory.
    /// </summary>
    [Test]
    public async ValueTask TimeLimitedItemCanBeMovedWithinInventoryAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = CreateItem(DateTime.UtcNow.AddDays(1), _ => { });
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new MoveItemAction().MoveItemAsync(player, InventorySlot, Storages.Inventory, InventorySlot + 1, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(InventorySlot + 1), Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that an expired item can't be equipped.
    /// </summary>
    [Test]
    public async ValueTask ExpiredItemCanNotBeEquippedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = CreateItem(DateTime.UtcNow.AddSeconds(-1), definition =>
        {
            var slot = new Mock<ItemSlotType>();
            slot.Setup(s => s.ItemSlots).Returns(new List<int> { InventoryConstants.PetSlot });
            definition.ItemSlot = slot.Object;
        });
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new MoveItemAction().MoveItemAsync(player, InventorySlot, Storages.Inventory, InventoryConstants.PetSlot, Storages.Inventory).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(InventoryConstants.PetSlot), Is.Null);
        Assert.That(player.Inventory.GetItem(InventorySlot), Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that a valid time-limited item isn't dropped, and an expired one only if it's droppable.
    /// </summary>
    /// <param name="isExpired">If set to <c>true</c>, the item is expired.</param>
    /// <param name="isDroppable">If set to <c>true</c>, the item is droppable.</param>
    /// <param name="expectedDropped">If set to <c>true</c>, the item is expected to be dropped.</param>
    [TestCase(false, true, false)]
    [TestCase(true, true, true)]
    [TestCase(true, false, false)]
    public async ValueTask TimeLimitedItemIsOnlyDroppedWhenExpiredAsync(bool isExpired, bool isDroppable, bool expectedDropped)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = CreateItem(isExpired ? DateTime.UtcNow.AddSeconds(-1) : DateTime.UtcNow.AddDays(1), definition => definition.IsDroppable = isDroppable);
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new DropItemAction().DropItemAsync(player, InventorySlot, new Point(10, 10)).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(InventorySlot), expectedDropped ? Is.Null : Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that the rules let a time-limited item be sold to an NPC only once it expired, and then also when its
    /// definition allows it only for expired items. The refusal by the rules shows a message.
    /// </summary>
    /// <param name="isExpired">If set to <c>true</c>, the item is expired.</param>
    /// <param name="isSellable">If set to <c>true</c>, the item is sellable to an NPC.</param>
    /// <param name="isSellableWhenExpired">If set to <c>true</c>, the item is sellable to an NPC once it expired.</param>
    /// <param name="expectedAllowed">If set to <c>true</c>, the rules are expected to allow the sale.</param>
    [TestCase(false, true, true, false)]
    [TestCase(true, false, true, true)]
    [TestCase(true, true, false, true)]
    [TestCase(true, false, false, false)]
    public async ValueTask TimeLimitedItemIsOnlySoldWhenExpiredAsync(bool isExpired, bool isSellable, bool isSellableWhenExpired, bool expectedAllowed)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { MerchantStore = new Mock<ItemStorage>().Object }, null!);
        var item = CreateItem(isExpired ? DateTime.UtcNow.AddSeconds(-1) : DateTime.UtcNow.AddDays(1), definition =>
        {
            definition.IsSellableToNpc = isSellable;
            definition.IsSellableWhenExpired = isSellableWhenExpired;
        });
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new SellItemToNpcAction().SellItemAsync(player, InventorySlot).ConfigureAwait(false);

        Mock.Get(player.ViewPlugIns.GetPlugIn<GameLogic.Views.IShowMessagePlugIn>()!)
            .Verify(v => v.ShowMessageAsync(It.IsAny<string>(), It.IsAny<Interfaces.MessageType>()), expectedAllowed ? Times.Never() : Times.Once());
    }

    /// <summary>
    /// Verifies that a time-limited item isn't repaired, even if its definition allows it.
    /// </summary>
    [Test]
    public async ValueTask TimeLimitedItemIsNotRepairedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var item = CreateItem(DateTime.UtcNow.AddDays(1), definition => definition.ItemSlot = new ItemSlotType());
        item.Durability = 5;
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(5));
    }

    /// <summary>
    /// Verifies that a trade is cancelled when it contains a time-limited item, however it got there.
    /// </summary>
    [Test]
    public async ValueTask TradeWithTimeLimitedItemIsCancelledAsync()
    {
        var trader1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await new TradeRequestAction().RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await new TradeAcceptAction().HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);

        // Not moved in by MoveItemAction, which would refuse it.
        var item = CreateItem(DateTime.UtcNow.AddDays(1), _ => { });
        await trader1.TemporaryStorage!.AddItemAsync(0, item).ConfigureAwait(false);

        var buttonAction = new TradeButtonAction();
        await buttonAction.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await buttonAction.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);

        Assert.That(trader2.Inventory!.Items, Does.Not.Contain(item));
    }

    /// <summary>
    /// Verifies that a time-limited item can't be bought from a personal store, even when it's in the store.
    /// </summary>
    [Test]
    public async ValueTask TimeLimitedItemCanNotBeBoughtFromPersonalStoreAsync()
    {
        var seller = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var buyer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        buyer.Money = 1_000_000;
        var item = CreateItem(DateTime.UtcNow.AddDays(1), _ => { });
        item.StorePrice = 100;
        await seller.ShopStorage!.AddItemAsync(InventoryConstants.FirstStoreItemSlotIndex, item).ConfigureAwait(false);
        seller.ShopStorage.StoreOpen = true;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, InventoryConstants.FirstStoreItemSlotIndex).ConfigureAwait(false);

        Assert.That(seller.ShopStorage.GetItem(InventoryConstants.FirstStoreItemSlotIndex), Is.SameAs(item));
        Assert.That(buyer.Money, Is.EqualTo(1_000_000));
    }

    private static Item CreateItem(DateTime expiresAt, Action<ItemDefinition> setRule)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definitionMock.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        var definition = definitionMock.Object;
        definition.Width = 1;
        definition.Height = 1;
        definition.Durability = 10;
        setRule(definition);

        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        item.Object.Durability = 1;
        item.Object.ExpiresAt = expiresAt;
        return item.Object;
    }
}
