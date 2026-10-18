// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Pathfinding;
using static MUnique.OpenMU.GameLogic.PlugIns.IItemDropPlugIn;

/// <summary>
/// Action to drop an item from the inventory to the floor.
/// </summary>
public class DropItemAction
{
    /// <summary>
    /// Drops the item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="target">The target coordinates.</param>
    public async ValueTask DropItemAsync(Player player, byte slot, Point target)
    {
        var item = player.Inventory?.GetItem(slot);

        if (item is null
            || !(player.CurrentMap?.Terrain.WalkMap[target.X, target.Y] ?? false))
        {
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(slot, false)).ConfigureAwait(false);
            return;
        }

        if (player.IsTemplatePlayer)
        {
            player.Logger.LogWarning("Can't drop items of a template account.");
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(slot, false)).ConfigureAwait(false);
            return;
        }

        if (item.Definition is { IsDroppable: false })
        {
            player.Logger.LogWarning(
                "Player {0} tried to drop {1}, which its item definition doesn't allow. The client item data may differ from the server.",
                player,
                item);
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.ItemCannotBeDropped)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(slot, false)).ConfigureAwait(false);
            return;
        }

        if (player.GameContext.PlugInManager.GetPlugInPoint<IItemDropPlugIn>() is { } plugInPoint)
        {
            var dropArguments = new ItemDropArguments();
            await plugInPoint.HandleItemDropAsync(player, item, target, dropArguments).ConfigureAwait(false);
            if (dropArguments.Cancel)
            {
                // If we're here, that means that a plugin has handled the drop request.
                // Now, we have to decide if we should remove the item from the inventory or not.
                if (dropArguments.Success)
                {
                    await this.RemoveItemFromInventoryAsync(player, item).ConfigureAwait(false);
                    await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                }
                else
                {
                    await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(slot, false)).ConfigureAwait(false);
                }

                return;
            }
        }

        await this.DropItemAsync(player, item, target).ConfigureAwait(false);
    }

    private async ValueTask DropItemAsync(Player player, Item item, Point target)
    {
        var owners = item.Definition!.IsBoundToCharacter
            ? player.GetAsEnumerable()
            : player.Party?.PartyList.AsEnumerable() ?? player.GetAsEnumerable();

        // we have to remove it from the inventory already here, so it gets saved without a storage.
        var originalSlot = item.ItemSlot;
        await this.RemoveItemFromInventoryAsync(player, item).ConfigureAwait(false);

        // We have to save here already. Otherwise, if the item got modified since last
        // save point by the dropper, changes would not be saved by the picking up player!
        try
        {
            await player.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Without this, the item was gone from the inventory and never reached the floor.
            player.Logger.LogError(ex, "Couldn't save before dropping {Item} of {Player}; the drop is cancelled.", item, player);
            if (!await player.Inventory!.AddItemAsync(originalSlot, item).ConfigureAwait(false))
            {
                await player.Inventory.AddItemAsync(item).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<IUpdateInventoryListPlugIn>(p => p.UpdateInventoryListAsync()).ConfigureAwait(false);
            return;
        }

        // Some room for improvement: When the item is not persisted, we don't need to save.
        // However, to check this in the right order, we need to extend IContext to
        // give us this information.
        var wasItemPersisted = player.PersistenceContext.Detach(item);
        var droppedItem = new DroppedItem(item, target, player.CurrentMap!, player, owners, wasItemPersisted);
        await player.CurrentMap!.AddAsync(droppedItem).ConfigureAwait(false);
        player.Logger.LogInformation("Item dropped: {Player} dropped '{DroppedItem}' from slot {Slot} (persisted: {Persisted}).", player, droppedItem, originalSlot, wasItemPersisted);
    }

    private async ValueTask RemoveItemFromInventoryAsync(Player player, Item item)
    {
        await player.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(item.ItemSlot, true)).ConfigureAwait(false);
    }
}