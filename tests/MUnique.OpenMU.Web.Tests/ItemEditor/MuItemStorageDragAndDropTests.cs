// <copyright file="MuItemStorageDragAndDropTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemEditor;

using Bunit;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.Web.ItemEditor;

/// <summary>
/// Tests for moving items by drag and drop in the <see cref="MuItemStorage"/>.
/// </summary>
[TestFixture]
public class MuItemStorageDragAndDropTests
{
    /// <summary>
    /// Dropping an item on a free cell moves it there and selects it.
    /// </summary>
    [Test]
    public void DropOnFreeCell_MovesItem()
    {
        using var context = new BunitContext();
        var (storage, item, _) = CreateStorage();
        Item? selectedItem = null;
        var component = Render(context, storage, selected => selectedItem = selected);

        Assert.That(component.FindAll(".mu-drop-cell"), Is.Empty);
        FindSelector(component, item).DragStart();

        var cells = component.FindAll(".mu-drop-cell");
        Assert.That(cells, Has.Count.EqualTo(InventoryConstants.WarehouseRows * InventoryConstants.RowSize));

        cells[(2 * InventoryConstants.RowSize) + 3].Drop();

        Assert.That(item.ItemSlot, Is.EqualTo((2 * InventoryConstants.RowSize) + 3));
        Assert.That(selectedItem, Is.SameAs(item));
        Assert.That(component.FindAll(".mu-drop-cell"), Is.Empty);
    }

    /// <summary>
    /// Dropping an item where it would overlap another item keeps it at its place.
    /// </summary>
    [Test]
    public void DropOnOccupiedCell_KeepsItem()
    {
        using var context = new BunitContext();
        var (storage, item, other) = CreateStorage();
        var component = Render(context, storage, _ => { });

        FindSelector(component, item).DragStart();
        component.FindAll(".mu-drop-cell")[other.ItemSlot].DragEnter();
        Assert.That(component.FindAll(".mu-drop-preview.invalid"), Has.Count.EqualTo(1));

        component.FindAll(".mu-drop-cell")[other.ItemSlot].Drop();

        Assert.That(item.ItemSlot, Is.Zero);
    }

    /// <summary>
    /// Ending the drag outside of a cell keeps the item and hides the drop cells.
    /// </summary>
    [Test]
    public void DragEnd_HidesDropCells()
    {
        using var context = new BunitContext();
        var (storage, item, _) = CreateStorage();
        var component = Render(context, storage, _ => { });

        FindSelector(component, item).DragStart();
        FindSelector(component, item).DragEnd();

        Assert.That(component.FindAll(".mu-drop-cell"), Is.Empty);
        Assert.That(item.ItemSlot, Is.Zero);
    }

    /// <summary>
    /// Leaving the box with the pointer hides the drop preview, but moving to the next cell keeps it.
    /// </summary>
    [Test]
    public void DragLeave_HidesDropPreview()
    {
        using var context = new BunitContext();
        var (storage, item, _) = CreateStorage();
        var component = Render(context, storage, _ => { });

        FindSelector(component, item).DragStart();
        component.FindAll(".mu-drop-cell")[1].DragEnter();
        component.FindAll(".mu-drop-cell")[2].DragEnter();
        component.FindAll(".mu-drop-cell")[1].DragLeave();
        Assert.That(component.FindAll(".mu-drop-preview"), Has.Count.EqualTo(1));

        component.FindAll(".mu-drop-cell")[2].DragLeave();
        Assert.That(component.FindAll(".mu-drop-preview"), Is.Empty);
    }

    /// <summary>
    /// When the shared drag ends in another box, the drop preview of this box doesn't come back on the next drag.
    /// </summary>
    [Test]
    public void SharedDragEndedElsewhere_ClearsDropPreview()
    {
        using var context = new BunitContext();
        var (storage, item, _) = CreateStorage();
        var component = context.Render<MuItemStorage>(parameters => parameters
            .Add(c => c.Value, storage)
            .Add(c => c.ValueExpression, () => storage)
            .Add(c => c.StorageType, StorageType.Vault)
            .Add(c => c.DraggedItem, item)
            .Add(c => c.DraggedItemChanged, _ => { }));

        component.FindAll(".mu-drop-cell")[1].DragEnter();
        Assert.That(component.FindAll(".mu-drop-preview"), Has.Count.EqualTo(1));

        component.Render(parameters => parameters.Add(c => c.DraggedItem, null));
        component.Render(parameters => parameters.Add(c => c.DraggedItem, item));

        Assert.That(component.FindAll(".mu-drop-preview"), Is.Empty);
    }

    private static AngleSharp.Dom.IElement FindSelector(IRenderedComponent<MuItemStorage> component, Item item)
    {
        return component.FindAll(".mu-item-selector").First(e => e.GetAttribute("title") == item.ToString());
    }

    private static IRenderedComponent<MuItemStorage> Render(BunitContext context, ItemStorage storage, Action<Item?> onSelectedItemChanged)
    {
        return context.Render<MuItemStorage>(parameters => parameters
            .Add(c => c.Value, storage)
            .Add(c => c.ValueExpression, () => storage)
            .Add(c => c.StorageType, StorageType.Vault)
            .Add(c => c.SelectedItemChanged, onSelectedItemChanged));
    }

    private static (ItemStorage Storage, Item Item, Item Other) CreateStorage()
    {
        var persistenceContext = new InMemoryPersistenceContextProvider().CreateNewContext();
        var storage = persistenceContext.CreateNew<ItemStorage>();
        var item = CreateItem(1, 0);
        var other = CreateItem(2, 5);
        storage.Items.Add(item);
        storage.Items.Add(other);
        return (storage, item, other);

        Item CreateItem(short number, byte slot)
        {
            var definition = persistenceContext.CreateNew<ItemDefinition>();
            definition.Name = $"Item {number}";
            definition.Number = number;
            definition.Width = 1;
            definition.Height = 1;
            var result = persistenceContext.CreateNew<Item>();
            result.Definition = definition;
            result.ItemSlot = slot;
            return result;
        }
    }
}
