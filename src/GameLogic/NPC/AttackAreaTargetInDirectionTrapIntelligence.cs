// <copyright file="AttackAreaTargetInDirectionTrapIntelligence.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC
{
    using System.Linq;
    using MUnique.OpenMU.GameLogic.Views.World;

    /// <summary>
    /// An AI which attacks all targets in an directed area, when it's triggered by at least one target in range.
    /// </summary>
    public class AttackAreaTargetInDirectionTrapIntelligence : TrapIntelligenceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackAreaTargetInDirectionTrapIntelligence"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        public AttackAreaTargetInDirectionTrapIntelligence(GameMap map)
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

            var targetsInRange = this.GetAllTargets()
                .Where(target => this.Trap.GetDirectionTo(target) == this.Trap.Rotation)
                .Where(target => this.Trap.IsInRange(target.Position, this.Trap.Definition.AttackRange + 1))
                .Where(target => !this.Map.Terrain.SafezoneMap[target.Position.X, target.Position.Y]);

            bool hasAttacked = false;
            foreach (var target in targetsInRange)
            {
                this.Trap.Attack(target);
                hasAttacked = true;
            }

            if (hasAttacked && this.Trap.Definition.AttackSkill is { } attackSkill)
            {
                this.Trap.ForEachWorldObserver(p => p.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(this.Trap, null, attackSkill, true), true);
            }
        }
    }
}