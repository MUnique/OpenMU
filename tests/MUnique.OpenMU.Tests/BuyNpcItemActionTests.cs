// <copyright file="BuyNpcItemActionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Tests for <see cref="BuyNpcItemAction"/>.
/// </summary>
[TestFixture]
public class BuyNpcItemActionTests
{
    private const byte StoreSlot = 5;
    private const byte InventorySlot = 20;
    private const int StorePrice = 500;

    /// <summary>
    /// Verifies that buying a stackable item onto an existing stack reports the purchase
    /// as bought instead of failed.
    /// </summary>
    [Test]
    public async ValueTask BuyingStackableItemOntoExistingStackReportsBoughtAndNotFailedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var definition = CreateStackableDefinition();
        var target = CreateItem(definition, 2);
        await player.Inventory!.AddItemAsync(InventorySlot, target).ConfigureAwait(false);
        var storeItem = CreateItem(definition, 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(target.Durability, Is.EqualTo(5));
        Assert.That(player.Money, Is.EqualTo(1_000_000 - StorePrice));
        Assert.That(player.Inventory.Items.Count(i => i.Definition == definition), Is.EqualTo(1));
        Mock.Get(player.ViewPlugIns.GetPlugIn<INpcItemBoughtPlugIn>()!).Verify(p => p.NpcItemBoughtAsync(target), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()!).Verify(p => p.ItemDurabilityChangedAsync(target, false), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()!).Verify(p => p.BuyNpcItemFailedAsync(), Times.Never);
    }

    /// <summary>
    /// Verifies that buying a stackable item without enough money keeps the stack
    /// and reports the purchase as failed.
    /// </summary>
    [Test]
    public async ValueTask BuyingStackableItemWithoutEnoughMoneyReportsFailedAndKeepsStackAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 0;
        var definition = CreateStackableDefinition();
        var target = CreateItem(definition, 2);
        await player.Inventory!.AddItemAsync(InventorySlot, target).ConfigureAwait(false);
        var storeItem = CreateItem(definition, 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(target.Durability, Is.EqualTo(2));
        Assert.That(player.Money, Is.EqualTo(0));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()!).Verify(p => p.BuyNpcItemFailedAsync(), Times.Once);
        Mock.Get(player.ViewPlugIns.GetPlugIn<INpcItemBoughtPlugIn>()!).Verify(p => p.NpcItemBoughtAsync(It.IsAny<Item>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the purchase plug-ins are called before and after a purchase which is stacked onto an existing item.
    /// </summary>
    [Test]
    public async ValueTask StackedPurchaseCallsBeforeAndAfterPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var plugIn = RegisterPurchasePlugIn(player);
        var definition = CreateStackableDefinition();
        var target = CreateItem(definition, 2);
        await player.Inventory!.AddItemAsync(InventorySlot, target).ConfigureAwait(false);
        var storeItem = CreateItem(definition, 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        plugIn.Verify(p => p.BeforePurchaseAsync(player, player.OpenedNpc, It.Is<NpcItemPurchaseArgs>(a => a.StoreItem == storeItem)), Times.Once);
        plugIn.Verify(p => p.AfterPurchaseAsync(player, player.OpenedNpc, It.IsAny<NpcItemPurchaseArgs>(), target), Times.Once);
    }

    /// <summary>
    /// Verifies that the purchase plug-ins are called before and after a purchase which creates a new item.
    /// </summary>
    [Test]
    public async ValueTask NormalPurchaseCallsBeforeAndAfterPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var plugIn = RegisterPurchasePlugIn(player);
        var storeItem = CreateItem(CreateStackableDefinition(), 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        var bought = player.Inventory!.Items.Single();
        Assert.That(player.Money, Is.EqualTo(1_000_000 - StorePrice));
        plugIn.Verify(p => p.BeforePurchaseAsync(player, player.OpenedNpc, It.IsAny<NpcItemPurchaseArgs>()), Times.Once);
        plugIn.Verify(p => p.AfterPurchaseAsync(player, player.OpenedNpc, It.IsAny<NpcItemPurchaseArgs>(), bought), Times.Once);
    }

    /// <summary>
    /// Verifies that the price and the item chosen by the plug-in are used.
    /// </summary>
    [Test]
    public async ValueTask PlugInCanChangeItemAndPriceAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var plugIn = RegisterPurchasePlugIn(player);
        var replacement = CreateItem(CreateStackableDefinition(), 7);
        plugIn.Setup(p => p.BeforePurchaseAsync(It.IsAny<Player>(), It.IsAny<NonPlayerCharacter>(), It.IsAny<NpcItemPurchaseArgs>()))
            .Callback<Player, NonPlayerCharacter, NpcItemPurchaseArgs>((_, _, args) =>
            {
                args.ReplacementItem = replacement;
                args.Price = 100;
            })
            .Returns(ValueTask.CompletedTask);
        var storeItem = CreateItem(CreateStackableDefinition(), 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1_000_000 - 100));
        Assert.That(player.Inventory!.Items.Single().Durability, Is.EqualTo(7));
    }

    /// <summary>
    /// Verifies that a purchase cancelled by the plug-in costs nothing and gives nothing.
    /// </summary>
    [Test]
    public async ValueTask CancelledPurchaseFailsWithoutAfterPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var plugIn = RegisterPurchasePlugIn(player);
        plugIn.Setup(p => p.BeforePurchaseAsync(It.IsAny<Player>(), It.IsAny<NonPlayerCharacter>(), It.IsAny<NpcItemPurchaseArgs>()))
            .Callback<Player, NonPlayerCharacter, NpcItemPurchaseArgs>((_, _, args) => args.Cancel = true)
            .Returns(ValueTask.CompletedTask);
        var storeItem = CreateItem(CreateStackableDefinition(), 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1_000_000));
        Assert.That(player.Inventory!.Items, Is.Empty);
        VerifyNotBought(plugIn);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()!).Verify(p => p.BuyNpcItemFailedAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that the plug-in is not told about purchases which failed because of missing money.
    /// </summary>
    [Test]
    public async ValueTask PurchaseWithoutMoneyDoesNotCallAfterPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 0;
        var plugIn = RegisterPurchasePlugIn(player);
        var storeItem = CreateItem(CreateStackableDefinition(), 3);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(player.Inventory!.Items, Is.Empty);
        VerifyNotBought(plugIn);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()!).Verify(p => p.BuyNpcItemFailedAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that the plug-in is not told about purchases which failed because the inventory is full.
    /// </summary>
    [Test]
    public async ValueTask PurchaseWithFullInventoryDoesNotCallAfterPlugInAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var plugIn = RegisterPurchasePlugIn(player);
        var bigDefinition = CreateStackableDefinition();
        bigDefinition.Width = 8;
        bigDefinition.Height = 8;
        var storeItem = CreateItem(bigDefinition, 1);
        storeItem.ItemSlot = StoreSlot;
        player.OpenedNpc = CreateMerchant(storeItem);
        await player.Inventory!.AddItemAsync(InventorySlot, CreateItem(CreateStackableDefinition(), 1)).ConfigureAwait(false);

        await new BuyNpcItemAction().BuyItemAsync(player, StoreSlot).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1_000_000));
        Assert.That(player.Inventory.Items.Count(), Is.EqualTo(1));
        VerifyNotBought(plugIn);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()!).Verify(p => p.BuyNpcItemFailedAsync(), Times.Once);
    }

    private static void VerifyNotBought(Mock<INpcItemPurchasePlugIn> plugIn)
    {
        plugIn.Verify(p => p.AfterPurchaseAsync(It.IsAny<Player>(), It.IsAny<NonPlayerCharacter>(), It.IsAny<NpcItemPurchaseArgs>(), It.IsAny<Item>()), Times.Never);
    }

    private static Mock<INpcItemPurchasePlugIn> RegisterPurchasePlugIn(Player player)
    {
        var plugIn = new Mock<INpcItemPurchasePlugIn>();
        plugIn.Setup(p => p.BeforePurchaseAsync(It.IsAny<Player>(), It.IsAny<NonPlayerCharacter>(), It.IsAny<NpcItemPurchaseArgs>())).Returns(ValueTask.CompletedTask);
        plugIn.Setup(p => p.AfterPurchaseAsync(It.IsAny<Player>(), It.IsAny<NonPlayerCharacter>(), It.IsAny<NpcItemPurchaseArgs>(), It.IsAny<Item>())).Returns(ValueTask.CompletedTask);
        player.GameContext.PlugInManager.RegisterPlugInAtPlugInPoint(plugIn.Object);
        return plugIn;
    }

    private static NonPlayerCharacter CreateMerchant(Item storeItem)
    {
        var store = new Mock<ItemStorage>();
        store.SetupAllProperties();
        store.Setup(s => s.Items).Returns(new List<Item> { storeItem });
        return new NonPlayerCharacter(null!, new MonsterDefinition { MerchantStore = store.Object }, null!);
    }

    private static ItemDefinition CreateStackableDefinition()
    {
        return new ItemDefinition
        {
            Group = 15,
            Value = StorePrice,

            // Like the scrolls and orbs of the initialized data, which cost their value.
            PriceDefinition = new MUnique.OpenMU.Persistence.BasicModel.ItemPriceDefinition { BasePriceFormula = "value" },
            Width = 1,
            Height = 1,
            Durability = 10,
        };
    }

    private static Item CreateItem(ItemDefinition definition, double durability)
    {
        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        item.Object.Durability = durability;
        return item.Object;
    }
}
