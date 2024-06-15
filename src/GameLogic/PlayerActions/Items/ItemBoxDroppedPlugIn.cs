// <copyright file="ItemBoxDroppedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;
using static MUnique.OpenMU.GameLogic.PlugIns.IItemDropPlugIn;

/// <summary>
/// This plugin handles the drop of an item box, e.g. box of kundun.
/// </summary>
[PlugIn(nameof(ItemBoxDroppedPlugIn), "This plugin handles the drop of an item box, e.g. box of kundun.")]
[Guid("3D15D55D-EEFE-4B5F-89B1-6934AB3F0BEE")]
public sealed class ItemBoxDroppedPlugIn : IItemDropPlugIn
{
    /// <inheritdoc />
    public async ValueTask HandleItemDropAsync(Player player, Item sourceItem, Point target, ItemDropArguments cancelArgs)
    {
        if (sourceItem.Definition!.DropItems.Count <= 0
            || sourceItem.Definition!.DropItems.Where(di => di.SourceItemLevel == sourceItem.Level) is not { } itemDropGroups)
        {
            return;
        }

        cancelArgs.WasHandled = true;
        if (itemDropGroups.Any(g => g.RequiredCharacterLevel > player.Level))
        {
            cancelArgs.Success = false;
            return;
        }

        cancelArgs.Success = true;
        var (item, droppedMoneyAmount, dropEffect) = player.GameContext.DropGenerator.GenerateItemDrop(itemDropGroups);
        if (droppedMoneyAmount is not null)
        {
            var droppedMoney = new DroppedMoney(droppedMoneyAmount.Value, player.Position, player.CurrentMap!);
            await player.CurrentMap!.AddAsync(droppedMoney).ConfigureAwait(false);
        }

        if (item is not null)
        {
            var droppedItem = new DroppedItem(item, player.Position, player.CurrentMap!, player);
            await player.CurrentMap!.AddAsync(droppedItem).ConfigureAwait(false);
        }

        if (dropEffect is not ItemDropEffect.Undefined)
        {
            await this.ShowDropEffectAsync(player, dropEffect).ConfigureAwait(false);
        }
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
}