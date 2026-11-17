// <copyright file="ItemExpirationExtensionsTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Tests for the extensions of time-limited items in <see cref="DataModel.ItemExtensions"/>.
/// </summary>
[TestFixture]
public class ItemExpirationExtensionsTest
{
    private static readonly DateTime Now = new(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Tests that an item without an expiration is neither time-limited nor expired.
    /// </summary>
    [Test]
    public void PermanentItem()
    {
        var item = new Item();

        Assert.That(item.IsTimeLimited(), Is.False);
        Assert.That(item.IsExpired(Now), Is.False);
    }

    /// <summary>
    /// Tests that a time-limited item is expired from its expiration timestamp on.
    /// </summary>
    [Test]
    public void TimeLimitedItemExpiresAtItsTimestamp()
    {
        var item = new Item { ExpiresAt = Now };

        Assert.That(item.IsTimeLimited(), Is.True);
        Assert.That(item.IsExpired(Now.AddSeconds(-1)), Is.False);
        Assert.That(item.IsExpired(Now), Is.True);
        Assert.That(item.IsExpired(Now.AddDays(1)), Is.True);
    }

    /// <summary>
    /// Tests that an item has an effect while it has durability and isn't expired.
    /// </summary>
    [Test]
    public void HasEffect()
    {
        Assert.That(new Item { Durability = 1 }.HasEffect(Now), Is.True);
        Assert.That(new Item { Durability = 0 }.HasEffect(Now), Is.False);
        Assert.That(new Item { Durability = 1, ExpiresAt = Now.AddSeconds(1) }.HasEffect(Now), Is.True);
        Assert.That(new Item { Durability = 1, ExpiresAt = Now }.HasEffect(Now), Is.False);
    }

    /// <summary>
    /// Tests that an item definition allows to get rid of an expired item when it allows one of the ways.
    /// </summary>
    /// <param name="droppable">If set to <c>true</c>, the item is droppable.</param>
    /// <param name="sellable">If set to <c>true</c>, the item is sellable to an NPC.</param>
    /// <param name="sellableWhenExpired">If set to <c>true</c>, the item is sellable to an NPC when expired.</param>
    /// <param name="expected">The expected result.</param>
    [TestCase(false, false, false, false)]
    [TestCase(true, false, false, true)]
    [TestCase(false, true, false, true)]
    [TestCase(false, false, true, true)]
    public void CanBeDisposedOfWhenExpired(bool droppable, bool sellable, bool sellableWhenExpired, bool expected)
    {
        var definition = new ItemDefinition
        {
            IsDroppable = droppable,
            IsSellableToNpc = sellable,
            IsSellableToNpcWhenExpired = sellableWhenExpired,
        };

        Assert.That(definition.CanBeDisposedOfWhenExpired(), Is.EqualTo(expected));
    }

    /// <summary>
    /// Tests that the expiration is copied together with the other values of an item.
    /// </summary>
    [Test]
    public void AssignValuesCopiesTheExpiration()
    {
        var source = new Item { ExpiresAt = Now };
        var target = new Item();

        target.AssignValues(source);

        Assert.That(target.ExpiresAt, Is.EqualTo(Now));
    }
}
