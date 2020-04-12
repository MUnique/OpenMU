// <copyright file="TrapIntelligenceBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// An abstract trap AI.
    /// </summary>
    public abstract class TrapIntelligenceBase : INpcIntelligence, IDisposable
    {
        private Timer aiTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapIntelligenceBase"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        protected TrapIntelligenceBase(GameMap map)
        {
            this.Map = map;
        }

        /// <summary>
        /// Gets or sets the trap.
        /// </summary>
        public Trap Trap { get; set; }

        /// <inheritdoc/>
        public NonPlayerCharacter Npc
        {
            get => this.Trap;
            set => this.Trap = (Trap)value;
        }

        /// <summary>
        /// Gets the map.
        /// </summary>
        protected GameMap Map { get; }

        /// <inheritdoc/>
        public void RegisterHit(IAttacker attacker)
        {
            throw new NotImplementedException("A trap can't be attacked");
        }

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

        /// <summary>
        /// Gets all possible targets.
        /// </summary>
        /// <returns>All possible targets.</returns>
        protected IEnumerable<IAttackable> GetAllTargets()
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

            return tempObservers.OfType<IAttackable>();
        }

        /// <summary>
        /// Function which is executed in an interval.
        /// </summary>
        protected abstract void Tick();
    }
}