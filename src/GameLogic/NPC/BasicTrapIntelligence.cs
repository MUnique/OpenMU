// <copyright file="BasicTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// A trap AI which is REALLY basic.
    /// </summary>
    public sealed class BasicTrapIntelligence : ITrapIntelligence, IDisposable
    {
        private readonly GameMap map;

        private IAttackable currentTarget;

        private Timer aiTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicTrapIntelligence"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public BasicTrapIntelligence(GameMap map)
        {
            this.map = map;
        }

        /// <inheritdoc/>
        public Trap Trap { get; set; }

        /// <inheritdoc/>
        public void Start()
        {
            this.aiTimer = new Timer(state => this.Tick(), null, this.Trap.Definition.AttackDelay, this.Trap.Definition.AttackDelay);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.aiTimer != null)
            {
                this.aiTimer.Dispose();
                this.aiTimer = null;
            }
        }

        private IEnumerable<IAttackable> SearchAllTargets()
        {
            List<IWorldObserver> tempObservers;
            IEnumerable<IAttackable> allTargets = null;

            this.Trap.ObserverLock.EnterReadLock();
            try
            {
                tempObservers = new List<IWorldObserver>(this.Trap.Observers);
            }
            finally
            {
                this.Trap.ObserverLock.ExitReadLock();
            }

            allTargets = tempObservers.OfType<IAttackable>();

            return allTargets;
        }

        private IAttackable SearchNextTarget()
        {
            List<IWorldObserver> tempObservers;
            this.Trap.ObserverLock.EnterReadLock();
            try
            {
                tempObservers = new List<IWorldObserver>(this.Trap.Observers);
            }
            finally
            {
                this.Trap.ObserverLock.ExitReadLock();
            }

            double closestdistance = 100;
            IAttackable closest = null;

            foreach (var attackable in tempObservers.OfType<IAttackable>())
            {
                if (this.map.Terrain.SafezoneMap[attackable.Position.X, attackable.Position.Y])
                {
                    continue;
                }

                double d = attackable.GetDistanceTo(this.Trap);
                if (closestdistance > d)
                {
                    closest = attackable;
                    closestdistance = d;
                }
            }

            return closest;

            // todo: check the walk distance
        }

        private bool IsTargetInObservers()
        {
            this.Trap.ObserverLock.EnterReadLock();
            try
            {
                return this.Trap.Observers.Contains(this.currentTarget as IWorldObserver);
            }
            finally
            {
                this.Trap.ObserverLock.ExitReadLock();
            }
        }

        private void Tick()
        {
            // When trap is defined as 'WhenPressed' target player should stand on them for activation
            if (this.Trap.Definition.TrapType == DataModel.Configuration.TrapType.WhenPressedAttackArea)
            {
                IEnumerable<IAttackable> playersInRange = this.SearchAllTargets();

                if (playersInRange.Any(player => player.Position.X == this.Trap.Position.X && player.Position.Y == this.Trap.Position.Y))
                {
                    foreach (var attackable in playersInRange)
                    {
                        if (this.map.Terrain.SafezoneMap[attackable.Position.X, attackable.Position.Y])
                        {
                            continue;
                        }

                        this.Trap.Attack(attackable);
                    }
                }

            }
            else if (this.Trap.Definition.TrapType == DataModel.Configuration.TrapType.WhenPressed)
            {
                IEnumerable<IAttackable> playersInRange = this.SearchAllTargets();
                var playersOnTrap = playersInRange.Where(player => player.Position.X == this.Trap.Position.X && player.Position.Y == this.Trap.Position.Y);

                foreach (var player in playersOnTrap)
                {
                    this.Trap.Attack(player);
                }
            }
            else
            {
                if (this.currentTarget != null)
                {
                    // Old Target out of Range?
                    if (!this.currentTarget.Alive
                        || this.currentTarget.IsAtSafezone()
                        || !this.IsTargetInObservers())
                    {
                        this.currentTarget = this.SearchNextTarget();
                    }
                }
                else
                {
                    this.currentTarget = this.SearchNextTarget();
                }

                // no target?
                if (this.currentTarget == null)
                {
                    return;
                }

                // Target in Attack Range?
                ushort dist = (ushort)this.currentTarget.GetDistanceTo(this.Trap);
                if (this.Trap.Definition.AttackRange + 1 >= dist)
                {
                    this.Trap.Attack(this.currentTarget);  // yes, attack
                }

            }
        }
    }
}