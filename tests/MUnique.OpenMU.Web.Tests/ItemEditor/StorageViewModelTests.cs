// <copyright file="StorageViewModelTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemEditor;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.ItemEditor;

/// <summary>
/// Tests for placing items by drag and drop with <see cref="StorageViewModel.CanPlace"/> and <see cref="StorageViewModel.TryPlace"/>.
/// </summary>
[TestFixture]
public class StorageViewModelTests
{
    private IContext _context = null!;

    /// <summary>
    /// Sets up a new persistence context.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._context = new InMemoryPersistenceContextProvider().CreateNewContext();
    }

    /// <summary>
    /// Disposes the persistence context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this._context.Dispose();
    }

    /// <summary>
    /// An item is moved to the slot of the dropped cell, if it fits there.
    /// </summary>
    [Test]
    public void TryPlace_FreeCell_MovesItem()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: 0, width: 2, height: 3);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.TryPlace(item, column: 5, row: 4), Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo((4 * InventoryConstants.RowSize) + 5));
    }

    /// <summary>
    /// The slot of the inventory box starts after the equipped items.
    /// </summary>
    [Test]
    public void TryPlace_Inventory_AddsStartIndex()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: InventoryConstants.EquippableSlotsCount, width: 1, height: 1);
        var viewModel = new StorageViewModel(storage, StorageType.Inventory, InventoryConstants.InventoryRows, InventoryConstants.EquippableSlotsCount, InventoryConstants.GetInventorySize(0) - 1);

        Assert.That(viewModel.TryPlace(item, column: 1, row: 2), Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(InventoryConstants.EquippableSlotsCount + (2 * InventoryConstants.RowSize) + 1));
    }

    /// <summary>
    /// An item which would stick out of the box isn't moved.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="row">The row.</param>
    [TestCase(7, 0)]
    [TestCase(0, 13)]
    [TestCase(-1, 0)]
    public void TryPlace_OutOfBox_DoesNothing(int column, int row)
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: 0, width: 2, height: 3);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.TryPlace(item, column, row), Is.False);
        Assert.That(item.ItemSlot, Is.Zero);
    }

    /// <summary>
    /// An item which would overlap with another item isn't moved.
    /// </summary>
    [Test]
    public void TryPlace_OverlappingOtherItem_DoesNothing()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: 0, width: 2, height: 2);
        _ = this.AddItem(storage, slot: (byte)((3 * InventoryConstants.RowSize) + 4), width: 1, height: 1);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.CanPlace(item, column: 3, row: 2), Is.False);
        Assert.That(viewModel.CanPlace(item, column: 5, row: 2), Is.True);
        Assert.That(viewModel.TryPlace(item, column: 3, row: 2), Is.False);
        Assert.That(item.ItemSlot, Is.Zero);
    }

    /// <summary>
    /// The item may overlap with its own previous position.
    /// </summary>
    [Test]
    public void TryPlace_OverlappingItself_MovesItem()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: 0, width: 2, height: 2);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.TryPlace(item, column: 1, row: 1), Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(InventoryConstants.RowSize + 1));
    }

    /// <summary>
    /// An item can be moved from another box of the same storage, e.g. from the vault extension into the vault.
    /// </summary>
    [Test]
    public void TryPlace_ItemOfOtherBox_MovesItem()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(storage, slot: (byte)(InventoryConstants.WarehouseSize + 3), width: 1, height: 1);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.TryPlace(item, column: 2, row: 0), Is.True);
        Assert.That(item.ItemSlot, Is.EqualTo(2));
    }

    /// <summary>
    /// An item of another storage isn't moved.
    /// </summary>
    [Test]
    public void TryPlace_ItemOfOtherStorage_DoesNothing()
    {
        var storage = this._context.CreateNew<ItemStorage>();
        var otherStorage = this._context.CreateNew<ItemStorage>();
        var item = this.AddItem(otherStorage, slot: 0, width: 1, height: 1);
        var viewModel = new StorageViewModel(storage, StorageType.Vault, InventoryConstants.WarehouseRows, 0, InventoryConstants.WarehouseSize - 1);

        Assert.That(viewModel.TryPlace(item, column: 2, row: 0), Is.False);
        Assert.That(item.ItemSlot, Is.Zero);
    }

    private Item AddItem(ItemStorage storage, byte slot, byte width, byte height)
    {
        var definition = this._context.CreateNew<ItemDefinition>();
        definition.Width = width;
        definition.Height = height;
        var item = this._context.CreateNew<Item>();
        item.Definition = definition;
        item.ItemSlot = slot;
        storage.Items.Add(item);
        return item;
    }
}
