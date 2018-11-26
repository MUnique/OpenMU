// <copyright file="MoveItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using static OpenMU.GameLogic.InventoryConstants;

    /// <summary>
    /// Action to move an item between <see cref="Storages"/> or the same storage.
    /// </summary>
    public class MoveItemAction
    {
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
            var fromItemStorage = fromStorageInfo.Storage;
            Item item = fromItemStorage.GetItem(fromSlot);

            if (item == null)
            {
                // Item not found
                player.PlayerView.InventoryView.ItemMoveFailed(null);
                return;
            }

            var toStorageInfo = this.GetStorageInfo(player, toStorage);
            var toItemStorage = toStorageInfo.Storage;

            if (!this.CanMove(player, item, toSlot, fromSlot, toStorageInfo, fromStorageInfo))
            {
                player.PlayerView.InventoryView.ItemMoveFailed(item);
                return;
            }

            fromItemStorage.RemoveItem(item);
            if (!toItemStorage.AddItem(toSlot, item))
            {
                fromItemStorage.AddItem(item);

                player.PlayerView.InventoryView.ItemMoveFailed(item);
                return;
            }

            player.PlayerView.InventoryView.ItemMoved(item, toSlot, toStorage);
            var isTradeOngoing = player.PlayerState.CurrentState == PlayerState.TradeOpened
                                 || player.PlayerState.CurrentState == PlayerState.TradeButtonPressed;
            var isItemMovedToOrFromTrade = toItemStorage == player.TemporaryStorage || fromItemStorage == player.TemporaryStorage;
            if (isTradeOngoing && isItemMovedToOrFromTrade && player.TradingPartner is Player tradingPartner)
            {
                // When Trading, send update to Trading-Partner
                if (fromItemStorage == player.TemporaryStorage)
                {
                    tradingPartner.PlayerView.TradeView.TradeItemDisappear(fromSlot, item);
                }

                if (toItemStorage == player.TemporaryStorage)
                {
                    tradingPartner.PlayerView.TradeView.TradeItemAppear(toSlot, item);
                }
            }
        }

        private StorageInfo GetStorageInfo(Player player, Storages storageType)
        {
            StorageInfo result;
            switch (storageType)
            {
                case Storages.Inventory:
                    result = new StorageInfo
                    {
                        Rows = InventoryRows,
                        StartIndex = EquippableSlotsCount,
                        EndIndex = (byte)(EquippableSlotsCount + GetInventorySize(player)),
                        Storage = player.Inventory
                    };
                    break;
                case Storages.PersonalStore:
                    result = new StorageInfo
                    {
                        Rows = StoreRows,
                        StartIndex = FirstStoreItemSlotIndex,
                        EndIndex = (byte)(FirstStoreItemSlotIndex + StoreSize),
                        Storage = player.ShopStorage
                    };
                    break;
                case Storages.Vault:
                    result = new StorageInfo
                    {
                        Rows = WarehouseRows,
                        EndIndex = WarehouseSize,
                        Storage = player.Vault,
                        StartIndex = 0
                    };
                    break;
                case Storages.Trade:
                case Storages.ChaosMachine:
                    result = new StorageInfo
                    {
                        Storage = player.TemporaryStorage,
                        Rows = TemporaryStorageRows,
                        EndIndex = TemporaryStorageSize,
                        StartIndex = 0
                    };
                    break;
                default:
                    throw new NotImplementedException($"Moving to {storageType} is not implemented.");
            }

            return result;
        }

        private bool CanMove(Player player, Item item, byte toSlot, byte fromSlot, StorageInfo toStorage, StorageInfo fromStorage)
        {
            var storage = toStorage.Storage;
            if (toStorage.Storage == player.Inventory && toSlot <= LastEquippableItemSlotIndex)
            {
                if (storage.GetItem(toSlot) != null)
                {
                    return false;
                }

                var itemDefinition = item.Definition;

                if (itemDefinition.ItemSlot.ItemSlots.Contains(toSlot) &&
                    player.CompliesRequirements(itemDefinition))
                {
                    // UpdatePreviewCharSet();
                    if (itemDefinition.ItemSlot.ItemSlots.Contains(RightHandSlot)
                        && itemDefinition.ItemSlot.ItemSlots.Contains(LeftHandSlot)
                        && toSlot == RightHandSlot
                        && storage.GetItem(LeftHandSlot)?.Definition.Width >= 2)
                    {
                        return false;
                    }

                    return true;
                }

                player.PlayerView.ShowMessage("You can't wear this Item.", MessageType.BlueNormal);
                return false;
            }

            return this.ItemFitsAtNewLocation(item, toSlot, fromSlot, toStorage, fromStorage);
        }

        private bool ItemFitsAtNewLocation(Item item, byte toSlot, byte fromSlot, StorageInfo toStorage, StorageInfo fromStorage)
        {
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
                if (blockingItem == null)
                {
                    continue; // no item is blocking the slot
                }

                this.SetUsedSlots(toStorage, blockingItem, usedSlots);
            }

            return !this.AreTargetSlotsBlocked(item, toSlot, toStorage, usedSlots);
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
            for (int r = rowIndex; r < rowIndex + item.Definition.Height; r++)
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
            for (int r = rowIndex; r < rowIndex + blockingItem.Definition.Height; r++)
            {
                for (int c = columnIndex; c < columnIndex + blockingItem.Definition.Width; c++)
                {
                    usedSlots[r, c] = true;
                }
            }
        }

        private class StorageInfo
        {
            public IStorage Storage { get; set; }

            public byte Rows { get; set; }

            public byte StartIndex { get; set; }

            public byte EndIndex { get; set; }
        }
    }
}
