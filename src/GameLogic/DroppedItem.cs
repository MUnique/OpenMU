// <copyright file="DroppedItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Linq;
    using System.Threading;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.Pathfinding;

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
        /// <param name="position">The position where the item was dropped on the map.</param>
        /// <param name="map">The map.</param>
        /// <param name="dropper">The dropper.</param>
        public DroppedItem(Item item, Point position, GameMap map, Player dropper)
        {
            this.Item = item;
            this.pickupLock = new object();
            this.Position = position;
            this.CurrentMap = map;
            this.dropper = dropper;
            this.removeTimer = new Timer(this.DisposeAndDelete, null, map.ItemDropDuration * 1000, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public Item Item { get; }

        /// <inheritdoc />
        public Point Position { get; set; }

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
        /// <param name="stackTarget">If the success is <c>true</c>, and this is not <c>null</c>, this dropped item was stacked on an existing item of the players inventory.</param>
        /// <returns>
        /// The success.
        /// </returns>
        /// <remarks>
        /// Can be overwritten, for example for quest items which dropped only for a specific player.
        /// </remarks>
        public bool TryPickUpBy(Player player, out Item stackTarget)
        {
            stackTarget = null;
            if (!this.availableToPick)
            {
                return false;
            }

            if (this.Item.IsStackable())
            {
                stackTarget = player.Inventory.Items.FirstOrDefault(i => i.CanCompletelyStackOn(this.Item));
            }

            if (stackTarget != null)
            {
                if (this.TryStackOnItem(player, stackTarget))
                {
                    return true;
                }

                return false;
            }

            return this.TryPickUp(player);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Id}: {this.Item} at {this.CurrentMap.Definition.Name} ({this.Position})";
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

        private bool TryPickUp(Player player)
        {
            Log.DebugFormat("Player {0} tries to pick up {1}", player, this);
            int slot = player.Inventory.CheckInvSpace(this.Item);
            if (slot < InventoryConstants.LastEquippableItemSlotIndex)
            {
                Log.DebugFormat("Inventory full, Player {0}, Item {1}", player, this);
                return false;
            }

            lock (this.pickupLock)
            {
                if (!this.availableToPick)
                {
                    Log.DebugFormat("Picked up by another player in the mean time, Player {0}, Item {1}", player, this);
                    return false;
                }

                if (!player.Inventory.AddItem((byte)slot, this.Item))
                {
                    Log.DebugFormat("Item could not be added to the inventory, Player {0}, Item {1}", player, this);
                    return false;
                }

                this.availableToPick = false;
            }

            Log.InfoFormat("Item '{0}' was picked up by player '{1}' and added to his inventory.", this, player);
            this.Dispose();
            if (this.dropper != null && !this.dropper.PlayerState.Finished)
            {
                this.dropper.PersistenceContext.SaveChanges(); // Otherwise, if the item got modified since last save point by the dropper, changes would not be saved by the picking up player!
                this.dropper.PersistenceContext.Detach(this.Item);
            }

            player.PersistenceContext.Attach(this.Item);

            return true;
        }

        private bool TryStackOnItem(Player player, Item stackTarget)
        {
            Log.DebugFormat("Player {0} tries to pick up {1}, trying to add to an existing item at slot {2}", player, this, stackTarget.ItemSlot);
            lock (this.pickupLock)
            {
                if (!this.availableToPick)
                {
                    Log.DebugFormat("Picked up by another player in the mean time, Player {0}, Item {1}", player, this);
                    return false;
                }

                stackTarget.Durability += this.Item.Durability;
                this.availableToPick = false;
            }

            Log.InfoFormat("Item '{0}' got picked up by player '{1}'. Durability of available stack {2} increased to {3}", this, player, stackTarget, stackTarget.Durability);
            this.DisposeAndDelete(null);
            return true;
        }

        private void DeleteItem(Player player)
        {
            Log.InfoFormat("Item '{0}' which was dropped by player '{1}' is getting deleted.", this, player);
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

            player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>()?.ItemDestroyed(this.Item);
        }
    }
}