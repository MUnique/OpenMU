// <copyright file="ShopStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The storage of the personal store of a player.
    /// </summary>
    public class ShopStorage : Storage, IShopStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShopStorage"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShopStorage(Player player)
            : base(0, 0, InventoryConstants.StoreSize, new ItemStorageAdapter(player.SelectedCharacter.Inventory, InventoryConstants.FirstStoreItemSlotIndex, InventoryConstants.StoreSize))
        {
            this.StoreLock = new object();
        }

        /// <inheritdoc/>
        public object StoreLock { get; }

        /// <inheritdoc/>
        public string StoreName { get; set; }

        /// <inheritdoc/>
        public bool StoreOpen { get; set; }

        /// <inheritdoc/>
        public override bool AddItem(byte slot, Item item)
        {
            if (this.StoreOpen)
            {
                return false;
            }

            return base.AddItem((byte)(slot - InventoryConstants.FirstStoreItemSlotIndex), item);
        }

        /// <inheritdoc />
        protected override void SetItemSlot(Item item, byte slot)
        {
            base.SetItemSlot(item, (byte)(slot + InventoryConstants.FirstStoreItemSlotIndex));
        }
    }
}
