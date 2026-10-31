// <copyright file="StorageReorganizationPersistenceTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections.Specialized;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Verifies that reorganizing loaded items preserves their persistent storage ownership.
/// </summary>
[TestFixture]
public class StorageReorganizationPersistenceTests
{
    /// <summary>
    /// Verifies equipment, inventory, vault and extension slot collisions with the real persistent model.
    /// </summary>
    /// <param name="occupiedSlot">The slot assigned to both loaded items.</param>
    /// <param name="reorganizedSlot">The first free slot for the second item.</param>
    /// <param name="numberOfSlots">The number of slots in the storage view.</param>
    /// <param name="boxOffset">The start of the rectangular storage area.</param>
    /// <param name="slotOffset">The first absolute slot of the storage view.</param>
    [TestCase(9, 12, 76, 12, 0)]
    [TestCase(12, 13, 76, 12, 0)]
    [TestCase(0, 1, 120, 0, 0)]
    [TestCase(76, 77, 32, 0, 76)]
    [TestCase(204, 205, 32, 0, 204)]
    public void ReorganizingLoadedItemsPreservesOwnerAndForeignKey(byte occupiedSlot, byte reorganizedSlot, int numberOfSlots, int boxOffset, int slotOffset)
    {
        var persistentStorage = new EntityFramework.Model.ItemStorage { Id = Guid.NewGuid() };
        var definition = new EntityFramework.Model.ItemDefinition
        {
            Id = Guid.NewGuid(),
            Width = 1,
            Height = 1,
        };
        var first = new EntityFramework.Model.Item { Id = Guid.NewGuid(), Definition = definition, ItemSlot = occupiedSlot };
        var reorganized = new EntityFramework.Model.Item { Id = Guid.NewGuid(), Definition = definition, ItemSlot = occupiedSlot };
        persistentStorage.Items.Add(first);
        persistentStorage.Items.Add(reorganized);
        Assert.That(reorganized.RawItemStorage, Is.SameAs(persistentStorage));
        Assert.That(reorganized.ItemStorageId, Is.EqualTo(persistentStorage.Id));
        var collectionChanges = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)persistentStorage.Items).CollectionChanged += (_, args) => collectionChanges.Add(args.Action);

        var storage = new Storage(
            numberOfSlots,
            boxOffset,
            slotOffset,
            new ItemStorageAdapter(persistentStorage, (byte)slotOffset, (byte)numberOfSlots));

        Assert.Multiple(() =>
        {
            Assert.That(storage.GetItem(occupiedSlot), Is.SameAs(first));
            Assert.That(storage.GetItem(reorganizedSlot), Is.SameAs(reorganized));
            Assert.That(reorganized.ItemSlot, Is.EqualTo(reorganizedSlot));
            Assert.That(persistentStorage.Items, Is.EquivalentTo(new[] { first, reorganized }));
            Assert.That(first.RawItemStorage, Is.SameAs(persistentStorage));
            Assert.That(first.ItemStorageId, Is.EqualTo(persistentStorage.Id));
            Assert.That(reorganized.RawItemStorage, Is.SameAs(persistentStorage));
            Assert.That(reorganized.ItemStorageId, Is.EqualTo(persistentStorage.Id));
            Assert.That(collectionChanges, Is.Empty, "Moving an already owned item must not change storage membership.");
        });
    }

    /// <summary>
    /// Verifies that failed reorganization leaves every item owned by the original storage.
    /// </summary>
    [Test]
    public void FullStorageRejectsCollisionWithoutChangingOwnership()
    {
        var persistentStorage = new EntityFramework.Model.ItemStorage { Id = Guid.NewGuid() };
        var definition = new EntityFramework.Model.ItemDefinition { Id = Guid.NewGuid(), Width = 1, Height = 1 };
        var items = Enumerable.Range(0, 9)
            .Select(index => new EntityFramework.Model.Item
            {
                Id = Guid.NewGuid(),
                Definition = definition,
                ItemSlot = (byte)(index % 8),
            })
            .ToArray();
        foreach (var item in items)
        {
            persistentStorage.Items.Add(item);
        }

        Assert.Throws<ArgumentException>(() => new Storage(8, persistentStorage));

        Assert.That(persistentStorage.Items, Is.EquivalentTo(items));
        foreach (var item in items)
        {
            Assert.That(item.RawItemStorage, Is.SameAs(persistentStorage));
            Assert.That(item.ItemStorageId, Is.EqualTo(persistentStorage.Id));
        }
    }
}
