// <copyright file="CollectionAdapterTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using System.Collections.Specialized;

/// <summary>
/// Tests for the <see cref="CollectionAdapter{TClass,TEfCore}"/>.
/// </summary>
[TestFixture]
public class CollectionAdapterTests
{
    /// <summary>
    /// Tests that clearing the collection notifies about the removed items.
    /// Previously, it removed all items and then threw an <see cref="ArgumentException"/>,
    /// because a <see cref="NotifyCollectionChangedAction.Reset"/> can't have changed items.
    /// </summary>
    [Test]
    public void ClearNotifiesAboutTheRemovedItems()
    {
        var adapter = new CollectionAdapter<object, string>(new List<string> { "a", "b" });
        NotifyCollectionChangedEventArgs? args = null;
        adapter.CollectionChanged += (_, e) => args = e;

        adapter.Clear();

        Assert.That(adapter, Is.Empty);
        Assert.That(args?.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
        Assert.That(args?.OldItems, Is.EquivalentTo(new[] { "a", "b" }));
    }

    /// <summary>
    /// Tests that clearing the items of a storage, e.g. of a merchant store, removes the reference of the items to the storage.
    /// </summary>
    [Test]
    public void ClearingTheItemsOfAStorageRemovesTheirReferenceToIt()
    {
        var storage = new EntityFramework.Model.ItemStorage();
        var item = new EntityFramework.Model.Item();
        storage.Items.Add(item);
        Assert.That(item.RawItemStorage, Is.SameAs(storage));

        storage.Items.Clear();

        Assert.That(storage.Items, Is.Empty);
        Assert.That(item.RawItemStorage, Is.Null);
    }
}
