// <copyright file="RandomAttackInRangeTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An AI which attacks a target which is in range of the trap.
    /// </summary>
    public class RandomAttackInRangeTrapIntelligence : TrapIntelligenceBase
    {
        private IAttackable currentTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAttackInRangeTrapIntelligence"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public RandomAttackInRangeTrapIntelligence(GameMap map)
            : base(map)
        {
        }

        /// <inheritdoc />
        protected override void Tick()
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

            double closestDistance = 100;
            IAttackable closest = null;

            foreach (var target in tempObservers.OfType<IAttackable>())
            {
                if (this.Map.Terrain.SafezoneMap[target.Position.X, target.Position.Y])
                {
                    continue;
                }

                double d = target.GetDistanceTo(this.Trap);
                if (closestDistance > d)
                {
                    closest = target;
                    closestDistance = d;
                }
            }

            return closest;
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
    }
}