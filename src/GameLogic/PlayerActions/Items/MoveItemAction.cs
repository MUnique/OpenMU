// <copyright file="MoveItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Interfaces;
using static MUnique.OpenMU.DataModel.InventoryConstants;

/// <summary>
/// Action to move an item between <see cref="Storages"/> or the same storage.
/// </summary>
public class MoveItemAction
{
    private enum Movement
    {
        None,

        Normal,

        PartiallyStack,

        CompleteStack,
    }

    /// <summary>
    /// Moves the item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="fromSlot">From slot.</param>
    /// <param name="fromStorage">From storage.</param>
    /// <param name="toSlot">To slot.</param>
    /// <param name="toStorage">To storage.</param>
    public async ValueTask MoveItemAsync(Player player, byte fromSlot, Storages fromStorage, byte toSlot, Storages toStorage)
    {
        var fromStorageInfo = this.GetStorageInfo(player, fromStorage);
        var fromItemStorage = fromStorageInfo?.Storage;
        var item = fromItemStorage?.GetItem(fromSlot);

        if (item is null)
        {
            // Item not found
            await player.InvokeViewPlugInAsync<IItemMoveFailedPlugIn>(p => p.ItemMoveFailedAsync(null)).ConfigureAwait(false);
            return;
        }

        var toStorageInfo = this.GetStorageInfo(player, toStorage);
        var toItemStorage = toStorageInfo?.Storage;

        var movement = await this.CanMoveAsync(player, item, toSlot, fromSlot, toStorageInfo, fromStorageInfo).ConfigureAwait(false);
        if (movement != Movement.None)
        {
            var cancelEventArgs = new CancelEventArgs();
            player.GameContext.PlugInManager.GetPlugInPoint<IItemMovingPlugIn>()?.ItemMoving(player, item, fromStorage, toSlot, cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                movement = Movement.None;
            }
        }

        switch (movement)
        {
            case Movement.Normal:
                await this.MoveNormalAsync(player, fromSlot, toSlot, toStorage, fromItemStorage!, item, toItemStorage!).ConfigureAwait(false);
                break;
            case Movement.PartiallyStack when toItemStorage?.GetItem(toSlot) is { } targetItem:
                await this.PartiallyStackAsync(player, item, targetItem).ConfigureAwait(false);
                break;
            case Movement.CompleteStack when toItemStorage?.GetItem(toSlot) is { } targetItem:
                await this.FullStackAsync(player, item, targetItem).ConfigureAwait(false);
                break;
            default:
                await player.InvokeViewPlugInAsync<IItemMoveFailedPlugIn>(p => p.ItemMoveFailedAsync(item)).ConfigureAwait(false);
                break;
        }

        if (movement is not Movement.None
            && player.GameContext.PlugInManager.GetPlugInPoint<PlugIns.IItemMovedPlugIn>() is { } itemMovedPlugIn)
        {
            await itemMovedPlugIn.ItemMovedAsync(player, item).ConfigureAwait(false);
        }

        if (movement is not (Movement.None or Movement.Normal)
            && toItemStorage?.GetItem(toSlot) is { } target
            && player.GameContext.PlugInManager.GetPlugInPoint<PlugIns.IItemStackedPlugIn>() is { } itemStackedPlugIn)
        {
            await itemStackedPlugIn.ItemStackedAsync(player, item, target).ConfigureAwait(false);
        }
    }

    private async ValueTask FullStackAsync(Player player, Item sourceItem, Item targetItem)
    {
        targetItem.Durability += sourceItem.Durability;
        await player.InvokeViewPlugInAsync<IItemMoveFailedPlugIn>(p => p.ItemMoveFailedAsync(sourceItem)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<Views.Inventory.IItemRemovedPlugIn>(p => p.RemoveItemAsync(sourceItem.ItemSlot)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);
    }

    private async ValueTask PartiallyStackAsync(Player player, Item sourceItem, Item targetItem)
    {
        var partialAmount = (byte)Math.Min(targetItem.Definition!.Durability - targetItem.Durability, sourceItem.Durability);
        targetItem.Durability += partialAmount;
        sourceItem.Durability -= partialAmount;
        await player.InvokeViewPlugInAsync<IItemMoveFailedPlugIn>(p => p.ItemMoveFailedAsync(sourceItem)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(sourceItem, false)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(targetItem, false)).ConfigureAwait(false);
    }

    private async ValueTask MoveNormalAsync(Player player, byte fromSlot, byte toSlot, Storages toStorage, IStorage fromItemStorage, Item item, IStorage toItemStorage)
    {
        await fromItemStorage.RemoveItemAsync(item).ConfigureAwait(false);
        if (!await toItemStorage.AddItemAsync(toSlot, item).ConfigureAwait(false))
        {
            await fromItemStorage.AddItemAsync(item).ConfigureAwait(false);

            await player.InvokeViewPlugInAsync<IItemMoveFailedPlugIn>(p => p.ItemMoveFailedAsync(item)).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<Views.Inventory.IItemMovedPlugIn>(p => p.ItemMovedAsync(item, toSlot, toStorage)).ConfigureAwait(false);
        var isTradeOngoing = player.PlayerState.CurrentState == PlayerState.TradeOpened
                             || player.PlayerState.CurrentState == PlayerState.TradeButtonPressed;
        var isItemMovedToOrFromTrade = toItemStorage == player.TemporaryStorage || fromItemStorage == player.TemporaryStorage;
        if (isTradeOngoing && isItemMovedToOrFromTrade && player.TradingPartner is Player tradingPartner)
        {
            // When Trading, send update to Trading-Partner
            if (fromItemStorage == player.TemporaryStorage)
            {
                await tradingPartner.InvokeViewPlugInAsync<ITradeItemDisappearPlugIn>(p => p.TradeItemDisappearAsync(fromSlot, item)).ConfigureAwait(false);
            }

            if (toItemStorage == player.TemporaryStorage)
            {
                await tradingPartner.InvokeViewPlugInAsync<ITradeItemAppearPlugIn>(p => p.TradeItemAppearAsync(toSlot, item)).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Gets the storage information.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="storageType">Type of the storage.</param>
    /// <returns>The information about the requested storage for the specified player.</returns>
    /// <remarks>
    /// For extended inventories and vaults, this is simplified!
    /// The client would prevent storing an item across boundaries, the
    /// server wouldn't.
    /// </remarks>
    private StorageInfo? GetStorageInfo(Player player, Storages storageType)
    {
        StorageInfo? result;
        switch (storageType)
        {
            case Storages.Inventory when player.Inventory is not null:
                result = new StorageInfo(
                    player.Inventory,
                    (byte)(InventoryRows + (player.SelectedCharacter!.InventoryExtensions * RowsOfOneExtension)),
                    EquippableSlotsCount,
                    (byte)(EquippableSlotsCount + player.GetInventorySize()));
                break;
            case Storages.PersonalStore when player.ShopStorage is not null:
                result = new StorageInfo(
                    player.ShopStorage,
                    StoreRows,
                    FirstStoreItemSlotIndex,
                    (byte)(FirstStoreItemSlotIndex + StoreSize));
                break;
            case Storages.Vault when player.Vault is not null:
                var warehouseSize = player.Account!.IsVaultExtended ? WarehouseSize * 2 : WarehouseSize;
                var warehouseRows = (byte)(WarehouseRows * 2);
                result = new StorageInfo(player.Vault, warehouseRows, 0, (byte)warehouseSize);
                break;
            case Storages.Trade:
            case Storages.ChaosMachine:
            case Storages.PetTrainer:
            case Storages.Refinery:
            case Storages.Smelting:
            case Storages.ItemRestore:
            case Storages.ChaosCardMaster:
            case Storages.CherryBlossomSpirit:
            case Storages.SeedCrafting:
            case Storages.SeedSphereCrafting:
            case Storages.SeedMountCrafting:
            case Storages.SeedUnmountCrafting:
                if (player.TemporaryStorage is not null)
                {
                    result = new StorageInfo(player.TemporaryStorage, TemporaryStorageRows, 0, TemporaryStorageSize);
                }
                else
                {
                    result = null;
                }

                break;
            default:
                result = null;
                player.Logger.LogError($"Moving to {storageType} is not implemented.");
                break;
        }

        return result;
    }

    private async ValueTask<Movement> CanMoveAsync(Player player, Item item, byte toSlot, byte fromSlot, StorageInfo? toStorage, StorageInfo? fromStorage)
    {
        if (toStorage is null || fromStorage is null)
        {
            return Movement.None;
        }

        if (fromStorage.Storage == player.Vault
            && toStorage.Storage == player.Inventory
            && player.IsVaultLocked)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("The vault is locked.", MessageType.BlueNormal)).ConfigureAwait(false);
            return Movement.None;
        }

        var storage = toStorage.Storage;
        if (toStorage.Storage == player.Inventory && toSlot <= LastEquippableItemSlotIndex)
        {
            var itemDefinition = item.Definition;
            if (storage.GetItem(toSlot) != null || itemDefinition?.ItemSlot is null)
            {
                return Movement.None;
            }

            if (player.CurrentMiniGame is { } miniGame
                && !miniGame.IsItemAllowedToEquip(item))
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You can't equip this item during the event.", MessageType.BlueNormal)).ConfigureAwait(false);
                return Movement.None;
            }

            if (itemDefinition.ItemSlot.ItemSlots.Contains(toSlot) &&
                player.CompliesRequirements(item))
            {
                static bool IsOneHandedOrShield(ItemDefinition definition) =>
                    (definition.ItemSlot!.ItemSlots.Contains(RightHandSlot) && definition.ItemSlot.ItemSlots.Contains(LeftHandSlot)) || definition.Group == 6;

                var rightHandItemDefinition = storage.GetItem(RightHandSlot)?.Definition!;

                if ((toSlot == LeftHandSlot
                    && itemDefinition.Width >= 2
                    && rightHandItemDefinition != null
                    && !rightHandItemDefinition.IsAmmunition)
                    || (toSlot == RightHandSlot
                    && IsOneHandedOrShield(itemDefinition)
                    && storage.GetItem(LeftHandSlot)?.Definition!.Width >= 2))
                {
                    // Attempting to equip a two-handed item to the left hand slot when a shield is in the right hand slot,
                    // or trying to equip a one-handed weapon or shield to the right hand slot when a two-handed item is in the left hand slot.
                    return Movement.None;
                }

                return Movement.Normal;
            }

            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You can't wear this item.", MessageType.BlueNormal)).ConfigureAwait(false);
            return Movement.None;
        }

        if (item.Definition!.IsBoundToCharacter && toStorage != fromStorage)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("This item is bound to the inventory of this character.", MessageType.BlueNormal)).ConfigureAwait(false);
            return Movement.None;
        }

        return this.ItemFitsAtNewLocation(player, item, toSlot, fromSlot, toStorage, fromStorage);
    }

    private Movement ItemFitsAtNewLocation(Player player, Item item, byte toSlot, byte fromSlot, StorageInfo toStorage, StorageInfo fromStorage)
    {
        var targetItem = toStorage.Storage.GetItem(toSlot);
        if (targetItem != null)
        {
            var insidePlayerInventory = toStorage.Storage == player.Inventory && fromStorage.Storage == player.Inventory;
            if (!insidePlayerInventory)
            {
                return Movement.None;
            }

            if (item.CanCompletelyStackOn(targetItem))
            {
                return Movement.CompleteStack;
            }

            if (item.CanPartiallyStackOn(targetItem))
            {
                return Movement.PartiallyStack;
            }

            return Movement.None;
        }

        // Build an array, and set true, where an item is blocking a place.
        bool sameStorage = toStorage.Storage == fromStorage.Storage;

        bool[,] usedSlots = new bool[toStorage.Rows, RowSize];

        for (var i = toStorage.StartIndex; i < toStorage.EndIndex; i++)
        {
            if (toStorage.StartIndex == fromSlot && sameStorage)
            {
                continue; // to make sure that the same item is not blocking itself
            }

            var blockingItem = toStorage.Storage.GetItem(toStorage.StartIndex);
            if (blockingItem is null)
            {
                continue; // no item is blocking the slot
            }

            this.SetUsedSlots(toStorage, blockingItem, usedSlots);
        }

        return this.AreTargetSlotsBlocked(item, toSlot, toStorage, usedSlots) ? Movement.None : Movement.Normal;
    }

    private bool AreTargetSlotsBlocked(Item item, byte toSlot, StorageInfo toStorage, bool[,] usedSlots)
    {
        for (var i = toStorage.StartIndex; i < toStorage.EndIndex; i++)
        {
            if (this.IsTargetSlotBlocked(item, toSlot, toStorage, usedSlots))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTargetSlotBlocked(Item item, byte toSlot, StorageInfo toStorage, bool[,] usedSlots)
    {
        int rowIndex = (toSlot - toStorage.StartIndex) / RowSize;
        int columnIndex = (toSlot - toStorage.StartIndex) % RowSize;

        if (rowIndex + item.Definition!.Height > usedSlots.GetLength(0))
        {
            return true;
        }

        if (columnIndex + item.Definition.Width > usedSlots.GetLength(1))
        {
            return true;
        }

        for (int r = rowIndex; r < rowIndex + item.Definition!.Height; r++)
        {
            for (int c = columnIndex; c < columnIndex + item.Definition.Width; c++)
            {
                if (usedSlots[r, c])
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetUsedSlots(StorageInfo toStorage, Item blockingItem, bool[,] usedSlots)
    {
        int columnIndex = (blockingItem.ItemSlot - toStorage.StartIndex) % RowSize;
        int rowIndex = (blockingItem.ItemSlot - toStorage.StartIndex) / RowSize;

        // Set all taken slots of this item to true
        for (int r = rowIndex; r < rowIndex + blockingItem.Definition!.Height; r++)
        {
            for (int c = columnIndex; c < columnIndex + blockingItem.Definition.Width; c++)
            {
                usedSlots[r, c] = true;
            }
        }
    }

    private record StorageInfo(IStorage Storage, byte Rows, byte StartIndex, byte EndIndex);
}