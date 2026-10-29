// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to pick up an item from the floor.
/// </summary>
public class PickupItemAction
{
    /// <summary>
    /// The maximum distance in tiles between the player and a drop which can be picked up.
    /// </summary>
    internal const int MaximumPickupDistance = 3;

    private static readonly TimeSpan RefusalLogInterval = TimeSpan.FromSeconds(10);

    private static readonly ConditionalWeakTable<Player, StrongBox<long>> LastRefusalLogs = new();

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
                    LogRefusal(player, droppedMoney, CanPickup(player, droppedMoney) ? "nobody could take the money (maximum money or no eligible party member)" : GetDistanceOrDeathReason(player));
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
                player.Logger.Log(GetLogLevel(player), "Pickup refused: {Player} at {Map} ({Position}) requested drop {DropId}, which doesn't exist (anymore).", player, player.CurrentMap?.Definition.Name, player.Position, dropId);
                await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                break;
        }
    }

    private static async ValueTask<(bool Success, Item? StackTarget)> RejectAsync(Player player, DroppedItem droppedItem, string messageKey)
    {
        LogRefusal(player, droppedItem, messageKey);
        await player.ShowLocalizedBlueMessageAsync(messageKey).ConfigureAwait(false);
        return (false, null);
    }

    private static bool CanPickup(Player player, ILocateable droppedLocateable)
    {
        if (!player.IsAlive)
        {
            return false;
        }

        var dist = (int)player.GetDistanceTo(droppedLocateable);
        if (dist > MaximumPickupDistance)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Logs why a pick up request was refused. The client only gets a generic failure, so this is the
    /// only place where the actual reason can be seen - e.g. when the position of the player on the
    /// server differs from the one the client shows.
    /// </summary>
    private static void LogRefusal(Player player, ILocateable drop, string reason)
    {
        player.Logger.Log(
            GetLogLevel(player),
            "Pickup refused: {Player} at {Map} ({Position}) could not pick up {Drop}, distance {Distance}: {Reason}",
            player,
            player.CurrentMap?.Definition.Name,
            player.Position,
            drop,
            (int)player.GetDistanceTo(drop),
            reason);
    }

    /// <summary>
    /// Server-side bots and offline levelers retry far away drops several times per second, which
    /// would flood the log; their refusals are only of interest when debugging them.
    /// A connected client can also send an unlimited amount of pick up requests, e.g. with ids of
    /// drops which don't exist, so a player gets at most one refusal per <see cref="RefusalLogInterval"/>
    /// logged as information; the others are logged for debugging.
    /// </summary>
    internal static LogLevel GetLogLevel(Player player)
    {
        if (player is OfflinePlayer)
        {
            return LogLevel.Debug;
        }

        var now = Environment.TickCount64;
        var lastLogged = LastRefusalLogs.GetValue(player, _ => new StrongBox<long>(now - (long)RefusalLogInterval.TotalMilliseconds));
        lock (lastLogged)
        {
            if (now - lastLogged.Value < RefusalLogInterval.TotalMilliseconds)
            {
                return LogLevel.Debug;
            }

            lastLogged.Value = now;
            return LogLevel.Information;
        }
    }

    private static string GetDistanceOrDeathReason(Player player)
    {
        return player.IsAlive ? $"too far away (maximum {MaximumPickupDistance})" : "player is dead";
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

    private static bool PlayerHasActiveQuestForItem(Player player, Item item)
    {
        var questStates = player.SelectedCharacter?.QuestStates;
        if (questStates is null)
        {
            return false;
        }

        return questStates
            .Select(q => q.ActiveQuest)
            .OfType<QuestDefinition>()
            .SelectMany(q => q.RequiredItems)
            .Any(r => r.Item == item.Definition && (r.DropItemGroup?.ItemLevel is null || r.DropItemGroup.ItemLevel == item.Level));
    }

    private static async ValueTask<(bool Success, Item? StackTarget)> TryPickupItemAsync(Player player, DroppedItem droppedItem)
    {
        if (!CanPickup(player, droppedItem))
        {
            LogRefusal(player, droppedItem, GetDistanceOrDeathReason(player));
            return (false, null);
        }

        if (IsLimitReached(player, droppedItem.Item.Definition))
        {
            var itemName = string.Empty;
            using (CultureHelper.SetTemporaryCulture(player.Culture))
            {
                itemName = droppedItem.Item.Level > 0
                    ? $"{droppedItem.Item.Definition?.Name.ToString()} +{droppedItem.Item.Level.ToString()}"
                    : droppedItem.Item.Definition?.Name.ToString();
            }

            LogRefusal(player, droppedItem, nameof(PlayerMessage.PickupLimitReached));
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.PickupLimitReached), itemName).ConfigureAwait(false);
            return (false, null);
        }

        var slot = player.Inventory?.CheckInvSpace(droppedItem.Item);
        if (slot < InventoryConstants.EquippableSlotsCount)
        {
            LogRefusal(player, droppedItem, "no free inventory space");
            return (false, null);
        }

        if (droppedItem.Item.Definition?.IsQuestItem == true
            && !PlayerHasActiveQuestForItem(player, droppedItem.Item))
        {
            return await RejectAsync(player, droppedItem, nameof(PlayerMessage.ItemDoesNotBelongToYou)).ConfigureAwait(false);
        }

        if (droppedItem.Item.Definition?.IsBoundToCharacter == true
            && !droppedItem.IsPlayerAnOwner(player))
        {
            return await RejectAsync(player, droppedItem, nameof(PlayerMessage.ItemDoesNotBelongToYou)).ConfigureAwait(false);
        }

        if (!droppedItem.IsPlayerAnOwner(player) && droppedItem.IsOwnerPickupPriorityActive)
        {
            return await RejectAsync(player, droppedItem, nameof(PlayerMessage.ItemDoesNotBelongToYou)).ConfigureAwait(false);
        }

        var result = await droppedItem.TryPickUpByAsync(player).ConfigureAwait(false);
        if (result.Success)
        {
            await player.OnPickedUpItemAsync(droppedItem).ConfigureAwait(false);
        }
        else
        {
            LogRefusal(player, droppedItem, result.StackTarget is null ? "already picked up, not an owner yet or no free inventory space" : "could not be stacked");
        }

        return result;
    }

    private static async ValueTask<bool> TryPickupMoneyAsync(Player player, DroppedMoney droppedMoney)
    {
        return CanPickup(player, droppedMoney) && await droppedMoney.TryPickUpByAsync(player).ConfigureAwait(false);
    }
}