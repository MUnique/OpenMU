// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.GameLogic.PlugIns;
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
        await this.RemoveItemFromInventoryAsync(player, item).ConfigureAwait(false);

        // We have to save here already. Otherwise, if the item got modified since last
        // save point by the dropper, changes would not be saved by the picking up player!
        await player.PersistenceContext.SaveChangesAsync().ConfigureAwait(false);

        // Some room for improvement: When the item is not persisted, we don't need to save.
        // However, to check this in the right order, we need to extend IContext to
        // give us this information.
        var wasItemPersisted = player.PersistenceContext.Detach(item);
        var droppedItem = new DroppedItem(item, target, player.CurrentMap!, player, owners, wasItemPersisted);
        await player.CurrentMap!.AddAsync(droppedItem).ConfigureAwait(false);
    }

    private async ValueTask RemoveItemFromInventoryAsync(Player player, Item item)
    {
        await player.Inventory!.RemoveItemAsync(item).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(item.ItemSlot, true)).ConfigureAwait(false);
    }
}