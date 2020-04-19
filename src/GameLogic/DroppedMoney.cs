// <copyright file="DroppedMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using log4net;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Money which got dropped on the ground of a map.
    /// </summary>
    public sealed class DroppedMoney : IDisposable, ILocateable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DroppedMoney));

        private static readonly TimeSpan TimeUntilDropIsFree = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets the pickup lock. Used to synchronize pick up requests from the players.
        /// </summary>
        private readonly object pickupLock;

        private readonly DateTime dropTimestamp = DateTime.UtcNow;

        private Player dropper;

        private IEnumerable<object> owners;

        private Timer removeTimer;

        private bool availableToPick = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedMoney" /> class.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="position">The position where the item was dropped on the map.</param>
        /// <param name="map">The map.</param>
        /// <param name="dropper">The dropper.</param>
        public DroppedMoney(uint quantity, Point position, GameMap map, Player dropper)
            : this(quantity, position, map, dropper, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedMoney" /> class.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="position">The position where the item was dropped on the map.</param>
        /// <param name="map">The map.</param>
        /// <param name="dropper">The dropper.</param>
        /// <param name="owners">The owners.</param>
        public DroppedMoney(uint quantity, Point position, GameMap map, Player dropper, IEnumerable<object> owners)
        {
            this.Quantity = quantity;
            this.pickupLock = new object();
            this.Position = position;
            this.CurrentMap = map;
            this.dropper = dropper;
            this.owners = owners;
            this.removeTimer = new Timer((d) => this.Dispose(), null, map.ItemDropDuration * 1000, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public uint Quantity { get; }

        /// <inheritdoc />
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public ushort Id { get; set; }

        /// <inheritdoc/>
        public GameMap CurrentMap { get; }

        /// <summary>
        /// Tries to pick the money by the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>
        /// The success.
        /// </returns>
        /// <remarks>
        /// Can be overwritten, for example for quest items which dropped only for a specific player.
        /// </remarks>
        public bool TryPickUpBy(Player player)
        {
            if (!this.IsPlayerAnOwner(player)
                && DateTime.UtcNow < this.dropTimestamp.Add(TimeUntilDropIsFree))
            {
                return false;
            }

            return this.TryPickUp(player);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[MONEY]: ID:{this.Id} qty:{this.Quantity} at {this.CurrentMap.Definition.Name} ({this.Position})";
        }


        /// <inheritdoc />
        public void Dispose()
        {
            var timer = this.removeTimer;
            if (timer != null)
            {
                this.removeTimer = null;
                timer.Dispose();
                this.CurrentMap.Remove(this);
                this.dropper = null;
                this.owners = null;
            }
        }

        private bool IsPlayerAnOwner(Player player)
        {
            return this.owners?.Contains(player) ?? true;
        }

        private bool TryPickUp(Player player)
        {
            Log.DebugFormat("Player {0} tries to pick up {1}", player, this);
            lock (this.pickupLock)
            {
                if (!this.availableToPick)
                {
                    Log.DebugFormat("Picked up by another player in the mean time, Player {0}, Money {1}", player, this);
                    return false;
                }

                if (!player.TryAddMoney((int)this.Quantity))
                {
                    Log.DebugFormat("Money could not be added to the inventory, Player {0}, Money {1}", player, this);
                    return false;
                }

                this.availableToPick = false;
            }

            Log.InfoFormat("Money '{0}' was picked up by player '{1}' and added to his inventory.", this, player);
            this.Dispose();
            if (this.dropper != null && !this.dropper.PlayerState.Finished)
            {
                // Otherwise, if the item got modified since last save point by the dropper, changes would not be saved by the picking up player!
                this.dropper.PersistenceContext.SaveChanges();
            }

            return true;
        }
    }
}