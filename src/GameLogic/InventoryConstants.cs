﻿// <copyright file="InventoryConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// The constants for the players inventory.
    /// </summary>
    public static class InventoryConstants
    {
        /// <summary>
        /// The first equippable item slot index.
        /// </summary>
        public static readonly byte FirstEquippableItemSlotIndex = 0;

        /// <summary>
        /// The last equippable item slot index.
        /// </summary>
        public static readonly byte LastEquippableItemSlotIndex = 11;

        /// <summary>
        /// The equippable slots count.
        /// </summary>
        public static readonly byte EquippableSlotsCount = (byte)(LastEquippableItemSlotIndex - FirstEquippableItemSlotIndex + 1);

        /// <summary>
        /// The left hand inventory slot index.
        /// </summary>
        public static readonly byte LeftHandSlot = 0;

        /// <summary>
        /// The right hand inventory slot index.
        /// </summary>
        public static readonly byte RightHandSlot = 1;

        /// <summary>
        /// The helm inventory slot index.
        /// </summary>
        public static readonly byte HelmSlot = 2;

        /// <summary>
        /// The armor inventory slot index.
        /// </summary>
        public static readonly byte ArmorSlot = 3;

        /// <summary>
        /// The pants inventory slot index.
        /// </summary>
        public static readonly byte PantsSlot = 4;

        /// <summary>
        /// The gloves inventory slot index.
        /// </summary>
        public static readonly byte GlovesSlot = 5;

        /// <summary>
        /// The boots inventory slot index.
        /// </summary>
        public static readonly byte BootsSlot = 6;

        /// <summary>
        /// The wings inventory slot index.
        /// </summary>
        public static readonly byte WingsSlot = 7;

        /// <summary>
        /// The pet inventory slot index.
        /// </summary>
        public static readonly byte PetSlot = 8;

        /// <summary>
        /// The size of a row.
        /// </summary>
        public static readonly int RowSize = 8;

        /// <summary>
        /// Number of rows in the inventory.
        /// </summary>
        public static readonly byte InventoryRows = 8;

        /// <summary>
        /// The maximum number of extensions.
        /// </summary>
        public static readonly byte MaximumNumberOfExtensions = 4;

        /// <summary>
        /// The number of rows of one extension.
        /// </summary>
        public static readonly byte RowsOfOneExtension = 4;

        /// <summary>
        /// Number of rows of the inventory extension.
        /// </summary>
        public static readonly byte AllInventoryExtensionRows = (byte)(MaximumNumberOfExtensions * RowsOfOneExtension);

        /// <summary>
        /// Index of the first personal store slot.
        /// 12 = number of wearable item slots
        /// 64 = number of inventory slots
        /// 128 = number of extended inventory slots (64 are hidden in game, S6E3).
        /// </summary>
        public static readonly byte FirstStoreItemSlotIndex = (byte)(
            EquippableSlotsCount + // 12
            (InventoryRows * RowSize) + // 64
            (AllInventoryExtensionRows * RowSize)); // 128

        /// <summary>
        /// The number of personal store rows.
        /// </summary>
        public static readonly byte StoreRows = 4;

        /// <summary>
        /// The size of the personal store.
        /// </summary>
        public static readonly byte StoreSize = (byte)(StoreRows * RowSize);

        /// <summary>
        /// The number of temporary storage rows.
        /// </summary>
        public static readonly byte TemporaryStorageRows = 4;

        /// <summary>
        /// The number of temporary storage slots.
        /// </summary>
        public static readonly byte TemporaryStorageSize = (byte)(TemporaryStorageRows * RowSize);

        /// <summary>
        /// The number of rows in the warehouse (vault).
        /// </summary>
        public static readonly byte WarehouseRows = 15;

        /// <summary>
        /// The number of warehouse item slots.
        /// </summary>
        public static readonly byte WarehouseSize = (byte)(WarehouseRows * RowSize);

        /// <summary>
        /// Gets the size of the inventory of the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The size of the inventory.</returns>
        public static byte GetInventorySize(Player player)
        {
            var size = EquippableSlotsCount +
                (InventoryRows * RowSize) +
                (RowsOfOneExtension * Math.Min(player.SelectedCharacter.InventoryExtensions, MaximumNumberOfExtensions));

            return (byte)size;
        }
    }
}
