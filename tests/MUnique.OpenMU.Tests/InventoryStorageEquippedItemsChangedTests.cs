// <copyright file="InventoryStorageEquippedItemsChangedTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.PlugIns;
using static MUnique.OpenMU.DataModel.InventoryConstants;

/// <summary>
/// Verifies that inventory equipment change operations await every asynchronous subscriber.
/// </summary>
[TestFixture]
public class InventoryStorageEquippedItemsChangedTests
{
    /// <summary>
    /// Ensures add, remove, and explicit equipment-change notifications await earlier async handlers.
    /// </summary>
    /// <param name="operationName">The operation to perform.</param>
    [TestCase("add")]
    [TestCase("remove")]
    [TestCase("raise")]
    public async Task EquipmentChangeOperationsAwaitEverySubscriberAsync(string operationName)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var inventory = player.Inventory!;
        var storage = inventory.AsInventoryStorage!;
        var itemDefinition = player.PersistenceContext.CreateNew<ItemDefinition>();
        itemDefinition.Width = 1;
        itemDefinition.Height = 1;
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = itemDefinition;
        item.Durability = 1;

        if (operationName == "remove")
        {
            Assert.That(await inventory.AddItemAsync(Ring1Slot, item).ConfigureAwait(false), Is.True);
        }

        var delayedHandlerStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseDelayedHandler = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var followingHandlerInvoked = false;
        AsyncEventHandler<ItemEventArgs> delayedHandler = async _ =>
        {
            delayedHandlerStarted.TrySetResult();
            await releaseDelayedHandler.Task.ConfigureAwait(false);
        };
        AsyncEventHandler<ItemEventArgs> followingHandler = _ =>
        {
            followingHandlerInvoked = true;
            return ValueTask.CompletedTask;
        };
        storage.EquippedItemsChanged += delayedHandler;
        storage.EquippedItemsChanged += followingHandler;

        try
        {
            Task operation = operationName switch
            {
                "add" => inventory.AddItemAsync(Ring1Slot, item).AsTask(),
                "remove" => inventory.RemoveItemAsync(item).AsTask(),
                "raise" => storage.RaiseEquippedItemsChangedAsync(item, true).AsTask(),
                _ => throw new ArgumentOutOfRangeException(nameof(operationName)),
            };

            await delayedHandlerStarted.Task.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(operation.IsCompleted, Is.False, "The operation must remain incomplete until the delayed subscriber finishes.");
                Assert.That(followingHandlerInvoked, Is.False, "Subscribers must run sequentially so later handlers observe earlier updates.");
            });

            releaseDelayedHandler.TrySetResult();
            await operation.ConfigureAwait(false);
            Assert.That(followingHandlerInvoked, Is.True);
        }
        finally
        {
            releaseDelayedHandler.TrySetResult();
            storage.EquippedItemsChanged -= delayedHandler;
            storage.EquippedItemsChanged -= followingHandler;
        }
    }
}
