// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to pick up an item from the floor.
/// </summary>
public class PickupItemAction
{
    /// <summary>
    /// Pickups the item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="dropId">The drop identifier.</param>
    public async ValueTask PickupItemAsync(Player player, ushort dropId)
    {
        var droppedLocateable = player.CurrentMap?.GetDrop(dropId);

        switch (droppedLocateable)
        {
            case DroppedMoney droppedMoney:
                if (!await TryPickupMoneyAsync(player, droppedMoney).ConfigureAwait(false))
                {
                    await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                }

                break;
            case DroppedItem droppedItem:
            {
                var (success, stackTarget) = await TryPickupItemAsync(player, droppedItem).ConfigureAwait(false);
                if (success)
                {
                    if (stackTarget != null)
                    {
                        await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.ItemStacked)).ConfigureAwait(false);
                        await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(stackTarget, false)).ConfigureAwait(false);
                    }
                    else
                    {
                        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(droppedItem.Item)).ConfigureAwait(false);
                    }
                }
                else
                {
                    await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                }

                break;
            }

            default:
                await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                break;
        }
    }

    private static bool CanPickup(Player player, ILocateable droppedLocateable)
    {
        if (!player.IsAlive)
        {
            return false;
        }

        var dist = (int)player.GetDistanceTo(droppedLocateable);
        if (dist > 3)
        {
            return false;
        }

        return true;
    }

    private static bool IsLimitReached(Player player, ItemDefinition? itemDefinition)
    {
        if (itemDefinition is null)
        {
            return true;
        }

        if (itemDefinition.StorageLimitPerCharacter == 0)
        {
            return false;
        }

        return player.Inventory?.Items.Count(item => item.Definition == itemDefinition) >= itemDefinition.StorageLimitPerCharacter;
    }

    private static async ValueTask<(bool Success, Item? StackTarget)> TryPickupItemAsync(Player player, DroppedItem droppedItem)
    {
        if (!CanPickup(player, droppedItem))
        {
            return (false, null);
        }

        if (IsLimitReached(player, droppedItem.Item.Definition))
        {
            var itemName = droppedItem.Item.Level > 0
                ? $"{droppedItem.Item.Definition?.Name} +{droppedItem.Item.Level}"
                : droppedItem.Item.Definition?.Name;

            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"Limit reached for '{itemName}'.", MessageType.BlueNormal)).ConfigureAwait(false);
            return (false, null);
        }

        var slot = player.Inventory?.CheckInvSpace(droppedItem.Item);
        if (slot < InventoryConstants.EquippableSlotsCount)
        {
            return (false, null);
        }

        var result = await droppedItem.TryPickUpByAsync(player).ConfigureAwait(false);
        if (result.Success)
        {
            await player.OnPickedUpItemAsync(droppedItem).ConfigureAwait(false);
        }

        return result;
    }

    private static async ValueTask<bool> TryPickupMoneyAsync(Player player, DroppedMoney droppedMoney)
    {
        return CanPickup(player, droppedMoney) && await droppedMoney.TryPickUpByAsync(player).ConfigureAwait(false);
    }
}