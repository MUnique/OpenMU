// <copyright file="TradePersistenceAttachTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// Ensures trade item reattachment does not recursively track unrelated inventory items.
/// </summary>
[TestFixture]
public class TradePersistenceAttachTests
{
    /// <summary>
    /// An incoming item must be attached with its owned options, without navigating
    /// through a parent inventory that also references already-tracked item identities.
    /// </summary>
    [Test]
    public void AttachTransferredItemDoesNotReattachParentInventoryGraph()
    {
        var conflicting = CreateGraph();
        using (var context = new TradeAttachContext())
        {
            context.Attach(conflicting.Existing);
            using var attachment = new TestTrackingContext(context);

            Assert.Throws<InvalidOperationException>(() => attachment.Attach(conflicting.Incoming));
        }

        var safe = CreateGraph();
        using var safeContext = new TradeAttachContext();
        safeContext.Attach(safe.Existing);
        using var safeAttachment = new TestTrackingContext(safeContext);
        Assert.DoesNotThrow(() => safeAttachment.AttachWithoutParent(safe.Incoming));
        Assert.That(safeContext.ChangeTracker.Entries<TrackedItem>().Count(), Is.EqualTo(2));
        Assert.That(safeContext.ChangeTracker.Entries<TrackedStorage>(), Is.Empty);
        Assert.That(safeContext.ChangeTracker.Entries<TrackedOption>().Count(), Is.EqualTo(1));
    }

    private static (TrackedItem Existing, TrackedItem Incoming) CreateGraph()
    {
        var duplicateId = Guid.NewGuid();
        var existing = new TrackedItem { Id = duplicateId };
        var staleCopy = new TrackedItem { Id = duplicateId };
        var incoming = new TrackedItem { Id = Guid.NewGuid() };
        var storage = new TrackedStorage { Id = Guid.NewGuid() };
        storage.Items.Add(staleCopy);
        storage.Items.Add(incoming);
        staleCopy.Storage = storage;
        incoming.Storage = storage;
        var option = new TrackedOption { Id = Guid.NewGuid(), Item = incoming };
        incoming.Options.Add(option);
        return (existing, incoming);
    }

    private sealed class TestTrackingContext : EntityFrameworkContextBase
    {
        public TestTrackingContext(DbContext context)
            : base(context, null!, false, null, NullLogger.Instance)
        {
        }
    }

    private sealed class TradeAttachContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // No database calls are made. The deliberately unreachable port prevents
            // the test from connecting to a real PostgreSQL server.
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=1;Database=trade_attach_no_connection;Username=none;Password=none");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrackedItem>().HasKey(i => i.Id);
            modelBuilder.Entity<TrackedStorage>().HasKey(i => i.Id);
            modelBuilder.Entity<TrackedOption>().HasKey(i => i.Id);
            modelBuilder.Entity<TrackedItem>().HasOne(i => i.Storage).WithMany(s => s.Items);
            modelBuilder.Entity<TrackedOption>().HasOne(o => o.Item).WithMany(i => i.Options);
        }
    }

    private sealed class TrackedStorage
    {
        public Guid Id { get; set; }

        public ICollection<TrackedItem> Items { get; } = new List<TrackedItem>();
    }

    private sealed class TrackedItem
    {
        public Guid Id { get; set; }

        public TrackedStorage? Storage { get; set; }

        [MemberOfAggregate]
        public ICollection<TrackedOption> Options { get; } = new List<TrackedOption>();
    }

    private sealed class TrackedOption
    {
        public Guid Id { get; set; }

        public TrackedItem? Item { get; set; }
    }
}
