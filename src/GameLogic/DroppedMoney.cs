// <copyright file="DroppedMoney.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Money which got dropped on the ground of a map.
    /// </summary>
    public sealed class DroppedMoney : IDisposable, ILocateable
    {
        /// <summary>
        /// Gets the pickup lock. Used to synchronize pick up requests from the players.
        /// </summary>
        private readonly object pickupLock;

        private Timer? removeTimer;

        private bool availableToPick = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="DroppedMoney" /> class.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="position">The position where the item was dropped on the map.</param>
        /// <param name="map">The map.</param>
        public DroppedMoney(uint amount, Point position, GameMap map)
        {
            this.Amount = amount;
            this.pickupLock = new object();
            this.Position = position;
            this.CurrentMap = map;
            this.removeTimer = new Timer((d) => this.Dispose(), null, map.ItemDropDuration * 1000, Timeout.Infinite);
        }

        /// <summary>
        /// Gets the money item.
        /// </summary>
        public uint Amount { get; }

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
            player.Logger.LogDebug("Player {0} tries to pick up {1}", player, this);
            lock (this.pickupLock)
            {
                if (!this.availableToPick)
                {
                    player.Logger.LogDebug("Picked up by another player in the mean time, Player {0}, Money {1}", player, this);
                    return false;
                }

                if (!player.TryAddMoney((int)this.Amount))
                {
                    player.Logger.LogDebug("Money could not be added to the inventory, Player {0}, Money {1}", player, this);
                    return false;
                }

                this.availableToPick = false;
            }

            player.Logger.LogInformation("Money '{0}' was picked up by player '{1}' and added to his inventory.", this, player);
            this.Dispose();

            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Money: {this.Amount} at {this.CurrentMap.Definition.Name} ({this.Position})";
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
            }
        }
    }
}