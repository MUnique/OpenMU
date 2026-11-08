// <copyright file="InventoryPlacementServiceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemEditor;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Web.ItemEditor;

/// <summary>
/// Tests for inventory placement and drag previews.
/// </summary>
[TestFixture]
public class InventoryPlacementServiceTests
{
    private readonly InventoryPlacementService _placementService = new(StorageType.Inventory, 0);

    /// <summary>
    /// A valid preview accounts for the full item rectangle and excludes the dragged item itself.
    /// </summary>
    [Test]
    public void CanMoveItemAllowsItsCurrentRectangle()
    {
        var item = CreateItem(12, 2, 2);

        var canMove = this._placementService.CanMoveItem(item, 12, [item], 12, 8);

        Assert.That(canMove, Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(12));
    }

    /// <summary>
    /// A preview is rejected when any cell of the item rectangle is occupied.
    /// </summary>
    [Test]
    public void CanMoveItemRejectsOccupiedCells()
    {
        var item = CreateItem(12, 2, 2);
        var blocker = CreateItem(23, 1, 1);

        var canMove = this._placementService.CanMoveItem(item, 22, [item, blocker], 12, 8);

        Assert.That(canMove, Is.False);
        Assert.That(item.ItemSlot, Is.EqualTo(12));
    }

    /// <summary>
    /// A preview is rejected when the item rectangle crosses a horizontal or vertical boundary.
    /// </summary>
    [TestCase((byte)19)]
    [TestCase((byte)68)]
    public void CanMoveItemRejectsOutOfBoundsRectangles(byte destinationSlot)
    {
        var item = CreateItem(12, 2, 2);

        var canMove = this._placementService.CanMoveItem(item, destinationSlot, [item], 12, 8);

        Assert.That(canMove, Is.False);
    }

    /// <summary>
    /// The preview accepts a rectangle which fits at the bottom edge of the inventory.
    /// </summary>
    [Test]
    public void CanMoveItemAllowsRectangleAtBottomBoundary()
    {
        var item = CreateItem(12, 2, 2);

        var canMove = this._placementService.CanMoveItem(item, 60, [item], 12, 8);

        Assert.That(canMove, Is.True);
    }

    /// <summary>
    /// Placement in an inventory extension uses coordinates relative to that extension.
    /// </summary>
    [Test]
    public void CanMoveItemUsesExtensionRelativeCoordinates()
    {
        var item = CreateItem(76, 2, 2);
        var blocker = CreateItem(85, 1, 1);

        var canMove = this._placementService.CanMoveItem(item, 84, [item, blocker], 76, 4);

        Assert.That(canMove, Is.False);
    }

    /// <summary>
    /// An item can be moved from the main inventory into an empty extension rectangle.
    /// </summary>
    [Test]
    public void CanMoveItemAllowsMovingIntoAnExtension()
    {
        var item = CreateItem(12, 2, 2);

        var canMove = this._placementService.CanMoveItem(item, 84, [item], 76, 4);

        Assert.That(canMove, Is.True);
    }

    /// <summary>
    /// A preview is rejected when its top-left slot is outside the displayed section.
    /// </summary>
    [TestCase((byte)11)]
    [TestCase((byte)76)]
    public void CanMoveItemRejectsSlotsOutsideSection(byte destinationSlot)
    {
        var item = CreateItem(12, 1, 1);

        var canMove = this._placementService.CanMoveItem(item, destinationSlot, [item], 12, 8);

        Assert.That(canMove, Is.False);
    }

    /// <summary>
    /// A negative column must not wrap to the previous row when calculating a drop position.
    /// </summary>
    [Test]
    public void GetDropPositionDoesNotWrapColumnAcrossRows()
    {
        var destination = InventoryPlacementService.GetDropPosition(20, 12, 1, 0);

        Assert.That(destination.Column, Is.EqualTo(-1));
        Assert.That(destination.Row, Is.EqualTo(1));
    }

    /// <summary>
    /// A placement with a negative column is rejected instead of wrapping into the previous row.
    /// </summary>
    [Test]
    public void CanMoveItemRejectsNegativeColumn()
    {
        var item = CreateItem(12, 2, 2);

        var canMove = this._placementService.CanMoveItem(item, -1, 1, [item], 12, 8);

        Assert.That(canMove, Is.False);
    }

    /// <summary>
    /// A successful move updates the item's slot after the preview check.
    /// </summary>
    [Test]
    public void TryMoveItemUpdatesSlotWhenPlacementIsValid()
    {
        var item = CreateItem(12, 2, 2);

        var moved = this._placementService.TryMoveItem(item, 22, [item], 12, 8);

        Assert.That(moved, Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(22));
    }

    /// <summary>
    /// An invalid move leaves the item's original slot unchanged.
    /// </summary>
    [Test]
    public void TryMoveItemPreservesSlotWhenPlacementIsInvalid()
    {
        var item = CreateItem(12, 2, 2);
        var blocker = CreateItem(23, 1, 1);

        var moved = this._placementService.TryMoveItem(item, 22, [item, blocker], 12, 8);

        Assert.That(moved, Is.False);
        Assert.That(item.ItemSlot, Is.EqualTo(12));
    }

    /// <summary>
    /// A move using explicit row and column coordinates stores the same location used by the preview.
    /// </summary>
    [Test]
    public void TryMoveItemUsesExplicitDropCoordinates()
    {
        var item = CreateItem(12, 2, 2);

        var moved = this._placementService.TryMoveItem(item, 2, 3, [item], 12, 8);

        Assert.That(moved, Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(38));
    }

    /// <summary>
    /// Equipment preview accepts only slots declared by the item's definition.
    /// </summary>
    [Test]
    public void CanEquipItemRequiresCompatibleSlot()
    {
        var slotType = new Mock<ItemSlotType>();
        slotType.SetupGet(type => type.ItemSlots).Returns(new List<int> { InventoryConstants.HelmSlot });
        var item = new Item
        {
            Definition = new ItemDefinition { ItemSlot = slotType.Object },
            ItemSlot = 12,
        };

        Assert.That(this._placementService.CanEquipItem(item, InventoryConstants.HelmSlot, [item]), Is.True);
        Assert.That(this._placementService.CanEquipItem(item, InventoryConstants.ArmorSlot, [item]), Is.False);
    }

    /// <summary>
    /// Equipment preview rejects slots occupied by another item.
    /// </summary>
    [Test]
    public void CanEquipItemRejectsOccupiedSlot()
    {
        var slotType = new Mock<ItemSlotType>();
        slotType.SetupGet(type => type.ItemSlots).Returns(new List<int> { InventoryConstants.HelmSlot });
        var item = new Item
        {
            Definition = new ItemDefinition { ItemSlot = slotType.Object },
            ItemSlot = 12,
        };
        var equippedItem = CreateItem(InventoryConstants.HelmSlot, 2, 2);

        Assert.That(this._placementService.CanEquipItem(item, InventoryConstants.HelmSlot, [item, equippedItem]), Is.False);
    }

    private static Item CreateItem(byte slot, byte width, byte height)
    {
        return new Item
        {
            ItemSlot = slot,
            Definition = new ItemDefinition
            {
                Width = width,
                Height = height,
            },
        };
    }
}
