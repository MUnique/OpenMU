// <copyright file="BuyNpcItemActionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
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
