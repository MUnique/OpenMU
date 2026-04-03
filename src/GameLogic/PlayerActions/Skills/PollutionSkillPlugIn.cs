// <copyright file="PollutionSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the pollution skill (book of lagle) of the summoner class. Based on a chance, it may push the targets 2 squares away from the attacker.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.PollutionSkillPlugIn_Name), Description = nameof(PlugInResources.PollutionSkillPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("9F4B2C1D-E7A6-4B3C-8D9E-0FAB12C3D4E5")]
public class PollutionSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public short Key => 225;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        if (!target.IsAlive
            || target is not IMovable movableTarget
            || target.CurrentMap is not { } currentMap
            || !Rand.NextRandomBool(Convert.ToDouble(attacker.Attributes[Stats.PollutionMoveTargetChance])))
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

        for (int i = 0; i < 2; i++)
        {
            var nextTarget = currentTarget.CalculateTargetPoint(direction);
            if (!currentMap.Terrain.WalkMap[nextTarget.X, nextTarget.Y]
                || (target is NonPlayerCharacter && target.CurrentMap.Terrain.SafezoneMap[nextTarget.X, nextTarget.Y]))
            {
                // we don't want to push the target into a non-reachable area, through walls or monsters into the safe zone.
                break;
            }

            currentTarget = nextTarget;
        }

        await movableTarget.MoveAsync(currentTarget).ConfigureAwait(false);
    }
}