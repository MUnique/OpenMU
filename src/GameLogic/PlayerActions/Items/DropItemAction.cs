// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Pathfinding;

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
    public void DropItem(Player player, byte slot, Point target)
    {
        var item = player.Inventory?.GetItem(slot);

        if (item is not null && (player.CurrentMap?.Terrain.WalkMap[target.X, target.Y] ?? false))
        {
            this.DropItem(player, item, target);
        }
        else
        {
            player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(slot, false);
        }
    }

    private void DropItem(Player player, Item item, Point target)
    {
        var owners = item.Definition!.IsBoundToCharacter
            ? player.GetAsEnumerable()
            : player.Party?.PartyList.AsEnumerable() ?? player.GetAsEnumerable();
        var droppedItem = new DroppedItem(item, target, player.CurrentMap!, player, owners);
        player.CurrentMap!.Add(droppedItem);
        player.Inventory!.RemoveItem(item);
        player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(item.ItemSlot, true);
    }
}