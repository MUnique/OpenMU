// <copyright file="BasicMonsterIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// A basic monster AI which is pretty basic.
    /// </summary>
    public sealed class BasicMonsterIntelligence : IMonsterIntelligence, IDisposable
    {
        private readonly GameMap map;

        private IAttackable currentTarget;

        private Timer aiTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMonsterIntelligence"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public BasicMonsterIntelligence(GameMap map)
        {
            this.map = map;
        }

        /// <inheritdoc/>
        public Monster Monster { get; set; }

        /// <inheritdoc/>
        public void Start()
        {
            this.aiTimer = new Timer(state => this.Tick(), null, this.Monster.Definition.AttackDelay, this.Monster.Definition.AttackDelay);
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

        /// <inheritdoc/>
        public void RegisterHit(IAttackable attacker)
        {
            if (this.currentTarget == null)
            {
                this.currentTarget = attacker;
            }
        }

        private IAttackable SearchNextTarget()
        {
            List<IWorldObserver> tempObservers;
            this.Monster.ObserverLock.EnterReadLock();
            try
            {
                tempObservers = new List<IWorldObserver>(this.Monster.Observers);
            }
            finally
            {
                this.Monster.ObserverLock.ExitReadLock();
            }

            double closestdistance = 100;
            IAttackable closest = null;

            foreach (var attackable in tempObservers.OfType<IAttackable>())
            {
                if (this.map.Terrain.SafezoneMap[attackable.Position.X, attackable.Position.Y])
                {
                    continue;
                }

                double d = attackable.GetDistanceTo(this.Monster);
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
            this.Monster.ObserverLock.EnterReadLock();
            try
            {
                return this.Monster.Observers.Contains(this.currentTarget as IWorldObserver);
            }
            finally
            {
                this.Monster.ObserverLock.ExitReadLock();
            }
        }

        private void Tick()
        {
            if (!this.Monster.Alive)
            {
                return;
            }

            if (this.Monster.IsWalking)
            {
                return;
            }

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
            ushort dist = (ushort)this.currentTarget.GetDistanceTo(this.Monster);
            if (this.Monster.Definition.AttackRange + 1 >= dist)
            {
                this.Monster.Attack(this.currentTarget);  // yes, attack
            }

            // Target in View Range?
            else if (this.Monster.Definition.ViewRange + 1 >= dist)
            {
                this.Monster.WalkTo(this.currentTarget);  // no, walk to the target
            }
            else
            {
                // we move around randomly, so the monster does not look dead when watched from distance.
                this.Monster.RandomMove();
            }
        }
    }
}
