// <copyright file="AttackSingleWhenPressedTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An AI which attacks a single target when it stands on the traps coordinates.
    /// </summary>
    public class AttackSingleWhenPressedTrapIntelligence : TrapIntelligenceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackSingleWhenPressedTrapIntelligence"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public AttackSingleWhenPressedTrapIntelligence(GameMap map)
            : base(map)
        {
        }

        /// <inheritdoc />
        protected override void Tick()
        {
            if (this.Trap.Observers.Count == 0)
            {
                return;
            }

            IEnumerable<IAttackable> playersInRange = this.PossibleTargets;
            var playersOnTrap = playersInRange.Where(player => player.Position == this.Trap.Position);

            foreach (var player in playersOnTrap)
            {
                this.Trap.Attack(player);
            }
        }
    }
}