// <copyright file="MoveItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using System.ComponentModel;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Interfaces;
using static OpenMU.GameLogic.InventoryConstants;

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
    public void MoveItem(Player player, byte fromSlot, Storages fromStorage, byte toSlot, Storages toStorage)
    {
        var fromStorageInfo = this.GetStorageInfo(player, fromStorage);
        var fromItemStorage = fromStorageInfo?.Storage;
        var item = fromItemStorage?.GetItem(fromSlot);

        if (item is null)
        {
            // Item not found
            player.ViewPlugIns.GetPlugIn<IItemMoveFailedPlugIn>()?.ItemMoveFailed(null);
            return;
        }

        var toStorageInfo = this.GetStorageInfo(player, toStorage);
        var toItemStorage = toStorageInfo?.Storage;

        var movement = this.CanMove(player, item, toSlot, fromSlot, toStorageInfo, fromStorageInfo);
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
                this.MoveNormal(player, fromSlot, toSlot, toStorage, fromItemStorage!, item, toItemStorage!);
                break;
            case Movement.PartiallyStack when toItemStorage?.GetItem(toSlot) is { } targetItem:
                this.PartiallyStack(player, item, targetItem);
                break;
            case Movement.CompleteStack when toItemStorage?.GetItem(toSlot) is { } targetItem:
                this.FullStack(player, item, targetItem);
                break;
            default:
                player.ViewPlugIns.GetPlugIn<IItemMoveFailedPlugIn>()?.ItemMoveFailed(item);
                break;
        }

        if (movement != Movement.None)
        {
            player.GameContext.PlugInManager.GetPlugInPoint<PlugIns.IItemMovedPlugIn>()?.ItemMoved(player, item);
        }
    }

    private void FullStack(Player player, Item sourceItem, Item targetItem)
    {
        targetItem.Durability += sourceItem.Durability;
        player.ViewPlugIns.GetPlugIn<IItemMoveFailedPlugIn>()?.ItemMoveFailed(sourceItem);
        player.ViewPlugIns.GetPlugIn<Views.Inventory.IItemRemovedPlugIn>()?.RemoveItem(sourceItem.ItemSlot);
        player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(targetItem, false);
    }

    private void PartiallyStack(Player player, Item sourceItem, Item targetItem)
    {
        var partialAmount = (byte)Math.Min(targetItem.Definition!.Durability - targetItem.Durability, sourceItem.Durability);
        targetItem.Durability += partialAmount;
        sourceItem.Durability -= partialAmount;
        player.ViewPlugIns.GetPlugIn<IItemMoveFailedPlugIn>()?.ItemMoveFailed(sourceItem);
        player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(sourceItem, false);
        player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(targetItem, false);
    }

    private void MoveNormal(Player player, byte fromSlot, byte toSlot, Storages toStorage, IStorage fromItemStorage, Item item, IStorage toItemStorage)
    {
        fromItemStorage.RemoveItem(item);
        if (!toItemStorage.AddItem(toSlot, item))
        {
            fromItemStorage.AddItem(item);

            player.ViewPlugIns.GetPlugIn<IItemMoveFailedPlugIn>()?.ItemMoveFailed(item);
            return;
        }

        player.ViewPlugIns.GetPlugIn<Views.Inventory.IItemMovedPlugIn>()?.ItemMoved(item, toSlot, toStorage);
        var isTradeOngoing = player.PlayerState.CurrentState == PlayerState.TradeOpened
                             || player.PlayerState.CurrentState == PlayerState.TradeButtonPressed;
        var isItemMovedToOrFromTrade = toItemStorage == player.TemporaryStorage || fromItemStorage == player.TemporaryStorage;
        if (isTradeOngoing && isItemMovedToOrFromTrade && player.TradingPartner is Player tradingPartner)
        {
            // When Trading, send update to Trading-Partner
            if (fromItemStorage == player.TemporaryStorage)
            {
                tradingPartner.ViewPlugIns.GetPlugIn<ITradeItemDisappearPlugIn>()?.TradeItemDisappear(fromSlot, item);
            }

            if (toItemStorage == player.TemporaryStorage)
            {
                tradingPartner.ViewPlugIns.GetPlugIn<ITradeItemAppearPlugIn>()?.TradeItemAppear(toSlot, item);
            }
        }
    }

    private StorageInfo? GetStorageInfo(Player player, Storages storageType)
    {
        StorageInfo? result;
        switch (storageType)
        {
            case Storages.Inventory when player.Inventory is not null:
                result = new StorageInfo(
                    player.Inventory,
                    InventoryRows,
                    EquippableSlotsCount,
                    (byte)(EquippableSlotsCount + GetInventorySize(player)));
                break;
            case Storages.PersonalStore when player.ShopStorage is not null:
                result = new StorageInfo(
                    player.ShopStorage,
                    StoreRows,
                    FirstStoreItemSlotIndex,
                    (byte)(FirstStoreItemSlotIndex + StoreSize));
                break;
            case Storages.Vault when player.Vault is not null:
                result = new StorageInfo(player.Vault, WarehouseRows, 0, WarehouseSize);
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

    private Movement CanMove(Player player, Item item, byte toSlot, byte fromSlot, StorageInfo? toStorage, StorageInfo? fromStorage)
    {
        if (toStorage is null || fromStorage is null)
        {
            return Movement.None;
        }

        if (fromStorage.Storage == player.Vault
            && toStorage.Storage == player.Inventory
            && player.IsVaultLocked)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("The vault is locked.", MessageType.BlueNormal);
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

            if (itemDefinition.ItemSlot.ItemSlots.Contains(toSlot) &&
                player.CompliesRequirements(item))
            {
                if (itemDefinition.ItemSlot.ItemSlots.Contains(RightHandSlot)
                    && itemDefinition.ItemSlot.ItemSlots.Contains(LeftHandSlot)
                    && toSlot == RightHandSlot
                    && storage.GetItem(LeftHandSlot)?.Definition!.Width >= 2)
                {
                    return Movement.None;
                }

                return Movement.Normal;
            }

            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You can't wear this Item.", MessageType.BlueNormal);
            return Movement.None;
        }

        if (item.Definition!.IsBoundToCharacter && toStorage != fromStorage)
        {
            player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("This item is bound to the inventory of this character.", MessageType.BlueNormal);
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