// <copyright file="ItemRuleFlagsTests.cs" company="MUnique">
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
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests that the item rule flags of <see cref="ItemDefinition"/> are enforced by the
/// drop, sell and repair actions. The storage moves are tested in <see cref="MoveItemActionTests"/>.
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
    /// Verifies that an item which isn't repairable isn't repaired, while a repairable one is.
    /// The item can't be worn, so its durability is the number of pieces, and repairing would reduce it.
    /// </summary>
    /// <param name="isRepairable">If set to <c>true</c>, the item is repairable.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask ItemIsOnlyRepairedWhenRepairableAsync(bool isRepairable)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = 1_000_000;
        var item = CreateItem(definition => definition.IsRepairable = isRepairable);
        item.Durability = 5;
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(item.Durability, isRepairable ? Is.Not.EqualTo(5) : Is.EqualTo(5));
    }

    private static Item CreateItem(Action<ItemDefinition> setRule)
    {
        var definition = new ItemDefinition { Width = 1, Height = 1, Durability = 10 };
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
