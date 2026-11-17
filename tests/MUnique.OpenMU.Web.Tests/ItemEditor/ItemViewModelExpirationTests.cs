// <copyright file="ItemViewModelExpirationTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.ItemEditor;

using System.ComponentModel.DataAnnotations;
using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Web.Shared.Components.ItemEdit;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests the expiration of an item in the <see cref="ItemViewModel"/> of the item editor.
/// </summary>
[TestFixture]
public class ItemViewModelExpirationTests
{
    /// <summary>
    /// Tests that switching the time limit on lets the item expire in one day, and switching it off makes it permanent.
    /// </summary>
    [Test]
    public void TimeLimitCanBeSwitched()
    {
        var item = CreateItem(isDroppable: true);
        var viewModel = new ItemViewModel(item, new Mock<IContext>().Object);

        viewModel.IsTimeLimited = true;
        Assert.That(item.ExpiresAt, Is.EqualTo(DateTime.UtcNow.AddDays(1)).Within(TimeSpan.FromMinutes(1)));

        var expiration = new DateTime(2027, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
        viewModel.ExpiresAt = expiration;
        Assert.That(item.ExpiresAt, Is.EqualTo(expiration));
        Assert.That(item.ExpiresAt!.Value.Kind, Is.EqualTo(DateTimeKind.Utc));

        viewModel.IsTimeLimited = false;
        Assert.That(item.ExpiresAt, Is.Null);
    }

    /// <summary>
    /// Tests that an item which couldn't be disposed of once expired can't be made time-limited, while one which
    /// can be dropped can.
    /// </summary>
    /// <param name="isDroppable">If set to <c>true</c>, the item is droppable.</param>
    [TestCase(true)]
    [TestCase(false)]
    public void ItemWithoutWayOutCannotBeTimeLimited(bool isDroppable)
    {
        var item = CreateItem(isDroppable);
        var viewModel = new ItemViewModel(item, new Mock<IContext>().Object) { IsTimeLimited = true };

        var results = viewModel.Validate(new ValidationContext(viewModel)).ToList();

        Assert.That(results, isDroppable ? Is.Empty : Has.Count.EqualTo(1));
    }

    private static Item CreateItem(bool isDroppable)
    {
        var slot = new BasicModel.ItemSlotType();
        slot.ItemSlots.Add(8);
        var definition = new BasicModel.ItemDefinition
        {
            ItemSlot = slot,
            Durability = 255,
            IsDroppable = isDroppable,
            IsSellableToNpc = false,
            IsSellableWhenExpired = false,
        };
        return new BasicModel.Item { Definition = definition, Durability = 255 };
    }
}
