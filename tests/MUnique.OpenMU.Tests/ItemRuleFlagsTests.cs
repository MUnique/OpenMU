// <copyright file="ItemRuleFlagsTests.cs" company="MUnique">
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
/// Tests that the item rule flags of <see cref="ItemDefinition"/> are enforced by the item actions.
/// The checks when an item is moved into a storage are tested in <see cref="MoveItemActionTests"/>.
/// </summary>
[TestFixture]
public class ItemRuleFlagsTests
{
    private const byte InventorySlot = 20;

    /// <summary>
    /// Verifies that an item which isn't droppable stays in the inventory, while a droppable one is dropped.
    /// </summary>
    /// <param name="isDroppable">If set to <c>true</c>, the item is droppable.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask ItemIsOnlyDroppedWhenDroppableAsync(bool isDroppable)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var item = CreateItem(definition => definition.IsDroppable = isDroppable);
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new DropItemAction().DropItemAsync(player, InventorySlot, new Point(10, 10)).ConfigureAwait(false);

        Assert.That(player.Inventory.GetItem(InventorySlot), isDroppable ? Is.Null : Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that an item which isn't sellable to an NPC isn't sold.
    /// </summary>
    [Test]
    public async ValueTask ItemWhichIsNotSellableToNpcIsNotSoldAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.OpenedNpc = new NonPlayerCharacter(null!, new MonsterDefinition { MerchantStore = new Mock<ItemStorage>().Object }, null!);
        var item = CreateItem(definition => definition.IsSellableToNpc = false);
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        var sold = await new SellItemToNpcAction().SellItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(sold, Is.False);
        Assert.That(player.Inventory.GetItem(InventorySlot), Is.SameAs(item));
    }

    /// <summary>
    /// Verifies that a wearable item is repaired to its maximum durability when it is repairable, and not at all when it isn't.
    /// </summary>
    /// <param name="isRepairable">If set to <c>true</c>, the item is repairable.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask WearableItemIsOnlyRepairedWhenRepairableAsync(bool isRepairable)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var item = CreateItem(definition =>
        {
            definition.ItemSlot = new ItemSlotType();
            definition.IsRepairable = isRepairable;
        });
        item.Durability = 5;
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(isRepairable ? item.Definition!.Durability : 5));
    }

    /// <summary>
    /// Verifies that an item which can't be worn isn't repaired: its durability is the number of pieces,
    /// and a repair would reduce the stack to one piece.
    /// </summary>
    [Test]
    public async ValueTask StackIsNotRepairedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var stack = CreateItem(_ => { });
        stack.Durability = 5;
        await player.Inventory!.AddItemAsync(InventorySlot, stack).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(stack.Durability, Is.EqualTo(5));
    }

    /// <summary>
    /// Verifies that items which another window (e.g. the chaos machine) left in the temporary storage go
    /// back to the inventory when a trade opens, so they are neither offered in the trade nor lost when
    /// it is cancelled.
    /// </summary>
    [Test]
    public async ValueTask LeftoverItemsReturnToInventoryWhenTradeOpensAsync()
    {
        var trader1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var leftover = CreateItem(definition => definition.IsTradable = false);
        await trader1.TemporaryStorage!.AddItemAsync(0, leftover).ConfigureAwait(false);

        await new TradeRequestAction().RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await new TradeAcceptAction().HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);

        Assert.That(trader1.Inventory!.Items, Does.Contain(leftover));
        Assert.That(trader1.TemporaryStorage.Items, Is.Empty);
    }

    /// <summary>
    /// Verifies that a trade is cancelled when it contains an item which isn't tradable, however it got there.
    /// </summary>
    [Test]
    public async ValueTask TradeWithItemWhichIsNotTradableIsCancelledAsync()
    {
        var trader1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);

        await new TradeRequestAction().RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await new TradeAcceptAction().HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);

        // Not moved in by MoveItemAction, which would refuse it.
        var notTradable = CreateItem(definition => definition.IsTradable = false);
        await trader1.TemporaryStorage!.AddItemAsync(0, notTradable).ConfigureAwait(false);

        var buttonAction = new TradeButtonAction();
        await buttonAction.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await buttonAction.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);

        Assert.That(trader2.Inventory!.Items, Does.Not.Contain(notTradable));
        Assert.That(trader1.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(trader2.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

    /// <summary>
    /// Verifies that an item which isn't sellable in a personal store can't be bought from one, even when it
    /// is in the store already (e.g. since before the item rules existed).
    /// </summary>
    [Test]
    public async ValueTask ItemWhichIsNotPersonalStoreSellableCanNotBeBoughtAsync()
    {
        var seller = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var buyer = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        buyer.Money = 1_000_000;
        var item = CreateItem(definition => definition.IsPersonalStoreSellable = false);
        item.StorePrice = 100;
        await seller.ShopStorage!.AddItemAsync(InventoryConstants.FirstStoreItemSlotIndex, item).ConfigureAwait(false);
        seller.ShopStorage.StoreOpen = true;

        await new BuyRequestAction().BuyItemAsync(buyer, seller, InventoryConstants.FirstStoreItemSlotIndex).ConfigureAwait(false);

        Assert.That(seller.ShopStorage.GetItem(InventoryConstants.FirstStoreItemSlotIndex), Is.SameAs(item));
        Assert.That(buyer.Inventory!.Items, Does.Not.Contain(item));
        Assert.That(buyer.Money, Is.EqualTo(1_000_000));
    }

    private static Item CreateItem(Action<ItemDefinition> setRule)
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
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
        return item.Object;
    }
}
