// <copyright file="EntityDeletionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Tests deletion of item aggregates before their next progress save.
/// </summary>
[TestFixture]
public class EntityDeletionTests
{
    /// <summary>
    /// Deleting an item must not issue DELETE statements for options which were never saved.
    /// </summary>
    /// <param name="savedItem">Whether the item already exists in the database.</param>
    /// <param name="savedOption">Whether the option already exists in the database.</param>
    [TestCase(true, false)]
    [TestCase(true, true)]
    [TestCase(false, false)]
    public async Task DeleteItemWithOptionAsync(bool savedItem, bool savedOption)
    {
        using var database = new EntityDataContext();
        using var context = new PlayerContext(database, null!, NullLogger<PlayerContext>.Instance);
        var item = new Item { Id = Guid.NewGuid() };
        var option = new ItemOptionLink { Id = Guid.NewGuid() };
        item.ItemOptions.Add(option);
        database.Add(item);
        if (savedItem)
        {
            database.Entry(item).State = EntityState.Unchanged;
        }

        if (savedOption)
        {
            database.Entry(option).State = EntityState.Unchanged;
        }

        await context.DeleteAsync(item).ConfigureAwait(false);
        database.ChangeTracker.DetectChanges();

        Assert.That(database.Entry(item).State, Is.EqualTo(savedItem ? EntityState.Deleted : EntityState.Detached));
        Assert.That(database.Entry(option).State, Is.EqualTo(savedOption ? EntityState.Deleted : EntityState.Detached));
    }

    /// <summary>
    /// Set membership links have composite keys and must follow the same deletion rules as options.
    /// </summary>
    /// <param name="savedLink">Whether the membership already exists in the database.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DeleteItemWithSetMembershipAsync(bool savedLink)
    {
        using var database = new EntityDataContext();
        using var context = new PlayerContext(database, null!, NullLogger<PlayerContext>.Instance);
        var item = new Item { Id = Guid.NewGuid() };
        var membership = new ItemItemOfItemSet { Item = item, ItemId = item.Id, ItemOfItemSetId = Guid.NewGuid() };
        item.JoinedItemSetGroups.Add(membership);
        database.Add(item);
        database.Entry(item).State = EntityState.Unchanged;
        if (savedLink)
        {
            database.Entry(membership).State = EntityState.Unchanged;
        }

        await context.DeleteAsync(item).ConfigureAwait(false);
        database.ChangeTracker.DetectChanges();

        Assert.That(database.Entry(item).State, Is.EqualTo(EntityState.Deleted));
        Assert.That(database.Entry(membership).State, Is.EqualTo(savedLink ? EntityState.Deleted : EntityState.Detached));
    }
}
