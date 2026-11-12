// <copyright file="StorageTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Tests the <see cref="Storage"/>.
/// </summary>
[TestFixture]
public class StorageTest
{
    /// <summary>
    /// Tests the adding of a small item to the upper left corner of the storage.
    /// </summary>
    [Test]
    public async ValueTask AddItem1X1TopLeftAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        var item = this.GetItem(1, 1);
        var added = await storage.AddItemAsync(12, item).ConfigureAwait(false);
        Assert.That(added, Is.True);
        Assert.That(storage.Items.Contains(item), Is.True);
        Assert.That(storage.FreeSlots.Contains((byte)12), Is.False);
    }

    /// <summary>
    /// Tests adding a 2x2 item to the right bottom corner of the storage.
    /// </summary>
    [Test]
    public async ValueTask AddItem2X2AtRightBottomAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        var item = this.GetItem(2, 2);
        byte slot = 12 + (6 * 8) + 6;
        var added = await storage.AddItemAsync(slot, item).ConfigureAwait(false);
        Assert.That(added, Is.True);
        Assert.That(storage.Items.Contains(item), Is.True);
        Assert.That(storage.FreeSlots.Contains(slot), Is.False);
    }

    /// <summary>
    /// Tests if the adding of the item fails because the same slot is already in use by another item.
    /// </summary>
    [Test]
    public async ValueTask AddItemFailSpaceInUseAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        await storage.AddItemAsync(12, this.GetItem(1, 1)).ConfigureAwait(false);
        var item = this.GetItem(1, 1);
        var added = await storage.AddItemAsync(12, item).ConfigureAwait(false);
        Assert.That(added, Is.False);
        Assert.That(storage.Items.Contains(item), Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)12), Is.False);
    }

    /// <summary>
    /// Tests if the adding of the item one line below another 2x2 item fails.
    /// </summary>
    [Test]
    public async ValueTask AddItemFailSpaceInUseVerticalAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        var addedItem = this.GetItem(2, 2);
        await storage.AddItemAsync(12, addedItem).ConfigureAwait(false);
        var added = await storage.AddItemAsync(13, this.GetItem(1, 1)).ConfigureAwait(false);
        Assert.That(added, Is.False);
        added = await storage.AddItemAsync(12 + 8, this.GetItem(1, 1)).ConfigureAwait(false);
        Assert.That(added, Is.False);
        Assert.That(storage.Items.Count(), Is.EqualTo(1));
        Assert.That(storage.Items.Contains(addedItem), Is.True);
        Assert.That(storage.FreeSlots.Contains((byte)12), Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)13), Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)(12 + 8)), Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)(13 + 8)), Is.False);
    }

    /// <summary>
    /// Tests if adding an 2x1 item at the right border fails,
    /// because the left part of the item would hang over.
    /// </summary>
    [Test]
    public async ValueTask AddItem2X1FailRightBorderAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        var added = await storage.AddItemAsync(12 + 7, this.GetItem(2, 1)).ConfigureAwait(false);
        Assert.That(added, Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)(12 + 7)), Is.True);
    }

    /// <summary>
    /// Tests if adding an 1x2 item at the bottom border fails,
    /// because the bottom part of the item would hang over.
    /// </summary>
    [Test]
    public async ValueTask AddItem1X2FailBottomAsync()
    {
        var itemStorage = this.CreateItemStorage();
        var storage = new Storage(12 + 64, 12, 0, itemStorage) as IStorage;
        var added = await storage.AddItemAsync(12 + (7 * 8), this.GetItem(1, 2)).ConfigureAwait(false);
        Assert.That(added, Is.False);
        Assert.That(storage.FreeSlots.Contains((byte)(12 + (7 * 8))), Is.True);
    }

    /// <summary>
    /// Tests that loading a storage moves an item which doesn't fit at its slot anymore, e.g. because its item
    /// definition became bigger (the Scroll of Blood, 1x2 to 2x2), or the item next to it, to a free place, so both
    /// are in the storage without overlapping.
    /// </summary>
    /// <param name="slotInGrid">The slot of the grown item, relative to the first slot of the grid.</param>
    [TestCase(7)] // The rightmost column: the item would hang over the right border.
    [TestCase(1)] // Next to an item to its right.
    public void LoadingMovesGrownItemToAFreePlace(byte slotInGrid)
    {
        var grownItem = CreateLoadedItem(2, 2, (byte)(12 + slotInGrid));
        var blockingItem = CreateLoadedItem(1, 1, 12 + 2);
        var itemStorage = this.CreateItemStorage(grownItem, blockingItem);

        var storage = new Storage(12 + 64, 12, 0, itemStorage);

        Assert.Multiple(() =>
        {
            Assert.That(storage.Items, Does.Contain(grownItem));
            Assert.That(storage.Items, Does.Contain(blockingItem));
            Assert.That(storage.FreeSlots.Count(), Is.EqualTo(64 - 4 - 1), "The two items overlap.");
        });
    }

    /// <summary>
    /// Tests that loading a storage fails when an item which doesn't fit at its slot finds no free place at all.
    /// A character with such an inventory can't enter the game until an admin moves the item.
    /// </summary>
    [Test]
    public void LoadingFailsWhenGrownItemFindsNoFreePlace()
    {
        var items = Enumerable.Range(0, 63).Select(index => CreateLoadedItem(1, 1, (byte)(12 + index))).ToList();
        items.Add(CreateLoadedItem(2, 2, 12 + 63));
        var itemStorage = this.CreateItemStorage(items.ToArray());

        Assert.That(() => new Storage(12 + 64, 12, 0, itemStorage), Throws.TypeOf<ArgumentException>().With.Message.Contains("do not fit"));
    }

    /// <summary>
    /// Creates an item as it is loaded from the database, with its collections, at the specified slot.
    /// </summary>
    private static Item CreateLoadedItem(byte width, byte height, byte slot)
    {
        return new MUnique.OpenMU.Persistence.BasicModel.Item
        {
            Definition = new ItemDefinition { Name = $"Item {width}x{height}", Durability = 1, Width = width, Height = height },
            ItemSlot = slot,
        };
    }

    private ItemDefinition GetItemDefintion(byte width, byte heigth)
    {
        var itemDefinition = new ItemDefinition
        {
            Durability = 100,
            Width = width,
            Height = heigth,
        };
        return itemDefinition;
    }

    private Item GetItem(byte width, byte heigth)
    {
        var item = new Item { Definition = this.GetItemDefintion(width, heigth) };
        item.Durability = item.Definition.Durability;
        return item;
    }

    private ItemStorage CreateItemStorage(params Item[] items)
    {
        var storage = new Mock<ItemStorage>();
        storage.Setup(s => s.Items).Returns(items.ToList());
        return storage.Object;
    }
}