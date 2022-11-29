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
    public async ValueTask DropItemAsync(Player player, byte slot, Point target)
    {
        var item = player.Inventory?.GetItem(slot);

        if (item is not null && (player.CurrentMap?.Terrain.WalkMap[target.X, target.Y] ?? false))
        {
            if (item.Definition!.DropItems.Count > 0
                && item.Definition!.DropItems.Where(di => di.SourceItemLevel == item.Level) is { } itemDropGroups)
            {
                await this.DropRandomItemAsync(item, player, itemDropGroups).ConfigureAwait(false);
            }
            else
            {
                await this.DropItemAsync(player, item, target).ConfigureAwait(false);
            }
        }
        else
        {
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(slot, false)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Drops a random item of the given <see cref="DropItemGroup"/> at the players coordinates.
    /// </summary>
    /// <param name="sourceItem">The source item.</param>
    /// <param name="player">The player.</param>
    /// <param name="dropItemGroups">The <see cref="DropItemGroup"/> from which the random item is generated.</param>
    private async ValueTask DropRandomItemAsync(Item sourceItem, Player player, IEnumerable<ItemDropItemGroup> dropItemGroups)
    {
        if (dropItemGroups.Any(g => g.RequiredCharacterLevel > player.Level))
        {
            await player.InvokeViewPlugInAsync<IItemDropResultPlugIn>(p => p.ItemDropResultAsync(sourceItem.ItemSlot, false)).ConfigureAwait(false);
            return;
        }

        var item = player.GameContext.DropGenerator.GenerateItemDrop(dropItemGroups, out var dropEffect, out var droppedMoneyAmount);
        if (droppedMoneyAmount is { })
        {
            var droppedMoney = new DroppedMoney(droppedMoneyAmount.Value, player.Position, player.CurrentMap!);
            await player.CurrentMap!.AddAsync(droppedMoney).ConfigureAwait(false);
        }

        if (item is { })
        {
            var droppedItem = new DroppedItem(item, player.Position, player.CurrentMap!, player);
            await player.CurrentMap!.AddAsync(droppedItem).ConfigureAwait(false);
        }

        if (dropEffect is { } && dropEffect != ItemDropEffect.Undefined)
        {
            await this.ShowDropEffectAsync(player, dropEffect.Value).ConfigureAwait(false);
        }

        await this.RemoveItemFromInventoryAsync(player, sourceItem).ConfigureAwait(false);
        await player.PersistenceContext.DeleteAsync(sourceItem).ConfigureAwait(false);
    }

    private async ValueTask ShowDropEffectAsync(Player player, ItemDropEffect dropEffect)
    {
        if (dropEffect == ItemDropEffect.Swirl)
        {
            await player.InvokeViewPlugInAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(player, IShowEffectPlugIn.EffectType.Swirl)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowItemDropEffectPlugIn>(p => p.ShowEffectAsync(dropEffect, player.Position)).ConfigureAwait(false);
        }
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