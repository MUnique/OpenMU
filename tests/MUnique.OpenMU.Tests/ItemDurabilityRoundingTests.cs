// <copyright file="ItemDurabilityRoundingTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Tests that the fractional durability of an item is seen as whole points, rounded up,
/// so that a fraction of wear neither shows as a lost point nor can be repaired and paid for.
/// </summary>
[TestFixture]
public class ItemDurabilityRoundingTests
{
    private const byte InventorySlot = 20;

    private const byte MaximumDurability = 10;

    private const int Money = 1_000_000;

    /// <summary>
    /// Verifies that the durability is rounded up to whole points.
    /// </summary>
    /// <param name="durability">The stored durability.</param>
    /// <param name="expected">The expected whole durability.</param>
    [TestCase(10.0, 10)]
    [TestCase(9.9998, 10)]
    [TestCase(9.0, 9)]
    [TestCase(8.5, 9)]
    [TestCase(0.0, 0)]
    [TestCase(255.5, 255)]
    public void DurabilityIsRoundedUp(double durability, int expected)
    {
        var item = CreateWearableItem();
        item.Durability = durability;

        Assert.That(item.Durability(), Is.EqualTo(expected));
    }

    /// <summary>
    /// Verifies that a fraction of wear doesn't change the whole durability, and that a full point of wear does.
    /// </summary>
    [Test]
    public void OnlyFullPointOfWearChangesWholeDurability()
    {
        var item = CreateWearableItem();
        item.Durability = MaximumDurability;

        Assert.That(item.DecreaseDurability(0.0002), Is.False);
        Assert.That(item.Durability(), Is.EqualTo(MaximumDurability));

        Assert.That(item.DecreaseDurability(1), Is.True);
        Assert.That(item.Durability(), Is.EqualTo(MaximumDurability - 1));
    }

    /// <summary>
    /// Verifies that an item keeps its last point until it's worn out completely.
    /// </summary>
    [Test]
    public void LastPointIsKeptUntilFullyWornOut()
    {
        var item = CreateWearableItem();
        item.Durability = 1;

        Assert.That(item.DecreaseDurability(0.5), Is.False);
        Assert.That(item.Durability(), Is.EqualTo(1));

        Assert.That(item.DecreaseDurability(0.5), Is.True);
        Assert.That(item.Durability, Is.EqualTo(0));
    }

    /// <summary>
    /// Verifies that an item which is only worn by a fraction of a point isn't repaired and doesn't cost money.
    /// </summary>
    [Test]
    public async ValueTask ItemWornByFractionIsNotRepairedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = Money;
        var item = CreateWearableItem();
        item.Durability = MaximumDurability - 0.0002;
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(Money));
        Assert.That(item.Durability, Is.EqualTo(MaximumDurability - 0.0002));
    }

    /// <summary>
    /// Verifies that the repair of all items doesn't cost money when the equipped items are only worn by a fraction of a point.
    /// </summary>
    [Test]
    public async ValueTask RepairAllDoesNotChargeForFractionOfWearAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = Money;
        var item = CreateWearableItem();
        item.Durability = MaximumDurability - 0.0002;
        await player.Inventory!.AddItemAsync(InventoryConstants.WingsSlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairAllItemsAsync(player).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(Money));
    }

    /// <summary>
    /// Verifies that an item which lost a whole point is repaired to its maximum durability and costs money.
    /// </summary>
    [Test]
    public async ValueTask ItemWornByWholePointIsRepairedAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.Money = Money;
        var item = CreateWearableItem();
        item.Durability = MaximumDurability - 1.5;
        await player.Inventory!.AddItemAsync(InventorySlot, item).ConfigureAwait(false);

        await new ItemRepairAction().RepairItemAsync(player, InventorySlot).ConfigureAwait(false);

        Assert.That(item.Durability, Is.EqualTo(MaximumDurability));
        Assert.That(player.Money, Is.LessThan(Money));
    }

    private static Item CreateWearableItem()
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.BasePowerUpAttributes).Returns(new List<ItemBasePowerUpDefinition>());
        definitionMock.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        var definition = definitionMock.Object;
        definition.Width = 1;
        definition.Height = 1;
        definition.Durability = MaximumDurability;
        definition.ItemSlot = new ItemSlotType();
        definition.IsRepairable = true;

        var item = new Mock<Item>();
        item.SetupAllProperties();
        item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        item.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        item.Object.Definition = definition;
        return item.Object;
    }
}
