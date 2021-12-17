// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Character;
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
            if (item.Definition!.DropItems.Count > 0
                && item.Definition!.DropItems.Where(di => di.SourceItemLevel == item.Level) is { } itemDropGroups)
            {
                this.DropRandomItem(item, player, itemDropGroups);
            }
            else
            {
                this.DropItem(player, item, target);
            }
        }
        else
        {
            player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(slot, false);
        }
    }

    /// <summary>
    /// Drops a random item of the given <see cref="DropItemGroup"/> at the players coordinates.
    /// </summary>
    /// <param name="sourceItem">The source item.</param>
    /// <param name="player">The player.</param>
    /// <param name="dropItemGroups">The <see cref="DropItemGroup"/> from which the random item is generated.</param>
    private void DropRandomItem(Item sourceItem, Player player, IEnumerable<ItemDropItemGroup> dropItemGroups)
    {
        var item = player.GameContext.DropGenerator.GenerateItemDrop(dropItemGroups, out var dropEffect, out var droppedMoneyAmount);
        if (droppedMoneyAmount is { })
        {
            var droppedMoney = new DroppedMoney(droppedMoneyAmount.Value, player.Position, player.CurrentMap!);
            player.CurrentMap!.Add(droppedMoney);
        }

        if (item is { })
        {
            var droppedItem = new DroppedItem(item, player.Position, player.CurrentMap!, player);
            player.CurrentMap!.Add(droppedItem);
        }

        if (dropEffect is { } && dropEffect != ItemDropEffect.Undefined)
        {
            this.ShowDropEffect(player, dropEffect.Value);
        }

        this.RemoveItemFromInventory(player, sourceItem);
        player.PersistenceContext.Delete(sourceItem);
    }

    private void ShowDropEffect(Player player, ItemDropEffect dropEffect)
    {
        if (dropEffect == ItemDropEffect.Swirl)
        {
            player.ViewPlugIns.GetPlugIn<IShowEffectPlugIn>()?.ShowEffect(player, IShowEffectPlugIn.EffectType.Swirl);
        }
        else
        {
            player.ViewPlugIns.GetPlugIn<IShowItemDropEffectPlugIn>()?.ShowEffect(dropEffect, player.Position);
        }
    }

    private void DropItem(Player player, Item item, Point target)
    {
        var owners = item.Definition!.IsBoundToCharacter
            ? player.GetAsEnumerable()
            : player.Party?.PartyList.AsEnumerable() ?? player.GetAsEnumerable();
        var droppedItem = new DroppedItem(item, target, player.CurrentMap!, player, owners);
        player.CurrentMap!.Add(droppedItem);
        this.RemoveItemFromInventory(player, item);
    }

    private void RemoveItemFromInventory(Player player, Item item)
    {
        player.Inventory!.RemoveItem(item);
        player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(item.ItemSlot, true);
    }
}