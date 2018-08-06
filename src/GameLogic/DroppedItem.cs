// <copyright file="DroppedItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Threading;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// An item which got dropped on the ground of a map.
    /// </summary>
    public sealed class DroppedItem : IDisposable, ILocateable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DroppedItem));

        /// <summary>
        /// Gets the pickup lock. Used to synchronize pick up requests from the players.
        /// </summary>
        private readonly object pickupLock;

        private Player dropper;

        private Timer removeTimer;

        private bool availableToPick = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedItem" /> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="map">The map.</param>
        /// <param name="dropper">The dropper.</param>
        public DroppedItem(Item item, byte x, byte y, GameMap map, Player dropper)
        {
            this.Item = item;
            this.pickupLock = new object();
            this.X = x;
            this.Y = y;
            this.CurrentMap = map;
            this.dropper = dropper;
            this.removeTimer = new Timer(this.DisposeAndDelete, null, map.ItemDropDuration * 1000, Timeout.Infinite);
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
        public GameMap CurrentMap { get; }

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
                if (!this.availableToPick)
                {
                    return false;
                }

                if (!player.Inventory.AddItem((byte)slot, this.Item))
                {
                    return false;
                }

                this.availableToPick = false;
            }

            Log.Info($"Item '{this.Item}' is getting picked by by player '{player}'.");
            this.Dispose();
            if (this.dropper != null && !this.dropper.PlayerState.Finished)
            {
                this.dropper.PersistenceContext.SaveChanges(); // Otherwise, if the item got modified since last save point by the dropper, changes would not be saved by the picking up player!
                this.dropper.PersistenceContext.Detach(this.Item);
            }

            player.PersistenceContext.Attach(this.Item);

            return true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            var timer = this.removeTimer;
            if (timer != null)
            {
                this.removeTimer = null;
                timer.Dispose();
                this.CurrentMap.Remove(this);
                this.dropper = null;
            }
        }

        private void DisposeAndDelete(object state)
        {
            try
            {
                var player = this.dropper;
                this.Dispose();
                if (player != null)
                {
                    this.DeleteItem(player);
                }
            }
            catch (Exception ex)
            {
                // we have to catch all errors, because it runs under a pooled thread without an additional safety net ;-)
                Log.Error("Error during DroppedItem.DisposeAndDelete", ex);
            }
        }

        private void DeleteItem(Player player)
        {
            Log.Info($"Item '{this.Item}' which was dropped by player {player} is getting deleted.");
            if (!player.PlayerState.Finished)
            {
                player.PersistenceContext.Detach(this.Item);
            }

            var repositoryManager = player.GameContext.PersistenceContextProvider;

            // We could use here the persistence context of the dropper - but if it logged out and is not saving anymore, the deletion would not be saved.
            // So we use a new temporary persistence context instead.
            using (var context = repositoryManager.CreateNewContext())
            {
                context.Delete(this.Item);
                context.SaveChanges();
            }
        }
    }
}