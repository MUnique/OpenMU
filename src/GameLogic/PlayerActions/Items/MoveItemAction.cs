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
                    return true;
                }

                /* TODO:
                else if (itemDefinition.ItemSlot == 0 && toSlot == 1 &&
                    player.SelectedCharacter.CharacterClass.CanWear2ndWeapon && player.ComplyRequirements(itemDefinition)
                    && itemDefinition.X == 1) //2nd weapon
                {
                    if (storage.GetItem(0) != null)
                    {
                        var firstwep = item.Definition;
                        return (firstwep.X < 2);
                    }
                    else
                        return true;
                }*/
                else
                {
                    player.PlayerView.ShowMessage("You can't wear this Item.", MessageType.BlueNormal);
                    return false;
                }
            }

            // Build an array, and set true, where an item is blocking a place.
            bool samestorage = toStorage.Storage == fromStorage.Storage;

            // var storageInfo = GetStorageInfo(player, toStorageType);
            byte starti = toStorage.StartIndex;
            bool[,] inv = new bool[toStorage.Rows, RowSize];

            for (; toStorage.StartIndex < toStorage.EndIndex; toStorage.StartIndex++)
            {
                if (toStorage.StartIndex == fromSlot && samestorage)
                {
                    continue; // to make sure that the same item is not blocking itself
                }

                var blockingItem = toStorage.Storage.GetItem(toStorage.StartIndex);
                if (blockingItem == null)
                {
                    continue; // no item is blocking the slot
                }

                var bas = blockingItem.Definition;
                int ci = (toStorage.StartIndex - starti) % RowSize;
                int ri = (toStorage.StartIndex - starti) / RowSize;

                // Set all taken slots of this item to true
                for (int r = ri; r < ri + bas.Height; r++)
                {
                    for (int c = ci; c < ci + bas.Width; c++)
                    {
                        inv[r, c] = true;
                    }
                }
            }

            int ro = (toSlot - starti) / RowSize;
            int co = (toSlot - starti) % RowSize;
            var bi = item.Definition;

            // TODO return (storage.FitsInside(inv, (byte)ro, (byte)co, bi.X, bi.Y));
            return true;
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
