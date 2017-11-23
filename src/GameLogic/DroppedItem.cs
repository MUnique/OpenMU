// <copyright file="DroppedItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Threading;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// An item which got dropped on the ground of a map.
    /// </summary>
    public sealed class DroppedItem : IDisposable, ILocateable
    {
        /// <summary>
        /// Gets the pickup lock. Used to synchronize pick up requests from the players.
        /// </summary>
        private readonly object pickupLock;

        private readonly GameMap map;

        private Timer removeTimer;

        private bool availableToPick = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedItem"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="map">The map.</param>
        public DroppedItem(Item item, byte x, byte y, GameMap map)
        {
            this.Item = item;
            this.pickupLock = new object();
            this.X = x;
            this.Y = y;
            this.map = map;
            this.removeTimer = new Timer((state) => this.Dispose(), null, map.ItemDropDuration * 1000, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets the x coordinate on the map.
        /// </summary>
        public byte X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate on the map.
        /// </summary>
        public byte Y { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public ushort Id { get; set; }

        /// <inheritdoc/>
        public GameMap CurrentMap => this.map;

        /// <summary>
        /// Tries to pick the item up by the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The success.</returns>
        /// <remarks>Can be overwritten, for example for quest items which dropped only for a specific player.</remarks>
        public bool TryPickUpBy(Player player)
        {
            if (!this.availableToPick)
            {
                return false;
            }

            int slot = player.Inventory.CheckInvSpace(this.Item);
            if (slot < InventoryConstants.LastEquippableItemSlotIndex)
            {
                return false;
            }

            lock (this.pickupLock)
            {
                if (player.Inventory.AddItem((byte)slot, this.Item))
                {
                    this.availableToPick = false;

                    this.Dispose();
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.removeTimer != null)
            {
                this.Item.Storage = null; // TODO: Delete from database, if item is persistent ?
                this.removeTimer.Dispose();
                this.removeTimer = null;
                this.map.Remove(this);
            }
        }
    }
}
