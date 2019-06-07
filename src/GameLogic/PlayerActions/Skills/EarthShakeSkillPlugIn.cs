// <copyright file="EarthShakeSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.Pathfinding;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handles the earth shake skill of the dark horse. Pushes the targets away from the attacker.
    /// </summary>
    [Guid("5D00F012-B0A3-41D6-B2FD-66D37A81615C")]
    [PlugIn("Earth shake skill", "Handles the earth shake skill of the dark horse. Pushes the targets away from the attacker.")]
    public class EarthShakeSkillPlugIn : IAreaSkillPlugIn
    {
        /// <inheritdoc />
        public short Key => 62;

        /// <inheritdoc />
        public void AfterTargetGotAttacked(IAttackable attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter)
        {
            if (!target.Alive || !(target is IMovable movableTarget))
            {
                return;
            }

            var startingPoint = attacker.Position;
            var currentTarget = target.Position;
            var direction = startingPoint.GetDirectionTo(currentTarget);
            if (direction == Direction.Undefined)
            {
                direction = (Direction)Rand.NextInt(1, 9);
            }

            var currentDistance = startingPoint.EuclideanDistanceTo(currentTarget);
            while (currentDistance < skillEntry.Skill.Range)
            {
                var nextTarget = currentTarget.CalculateTargetPoint(direction);
                if (!target.CurrentMap.Terrain.WalkMap[nextTarget.X, nextTarget.Y]
                    || (target is NonPlayerCharacter && target.CurrentMap.Terrain.SafezoneMap[nextTarget.X, nextTarget.Y]))
                {
                    // we don't want to push the target into a non-reachable area, through walls or monsters into the safe zone.
                    break;
                }

                currentTarget = nextTarget;
                currentDistance = startingPoint.EuclideanDistanceTo(currentTarget);
            }

            movableTarget.Move(currentTarget);
        }
    }
}