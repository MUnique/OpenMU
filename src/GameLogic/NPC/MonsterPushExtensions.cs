// <copyright file="MonsterPushExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions to push players away from monsters, like the knock-back of the monster skills of the original game.
/// </summary>
internal static class MonsterPushExtensions
{
    /// <summary>
    /// Pushes a player away from the monster, field by field, until the distance is reached or
    /// the next field isn't walkable or part of a safezone.
    /// Stunned and frozen players aren't pushed.
    /// </summary>
    /// <param name="monster">The monster.</param>
    /// <param name="player">The player which is pushed.</param>
    /// <param name="distance">The maximum number of fields the player is pushed.</param>
    public static async ValueTask PushAwayAsync(this Monster monster, Player player, int distance)
    {
        if (distance <= 0
            || !player.IsAlive
            || player.Attributes is not { } attributes
            || attributes[Stats.IsStunned] > 0
            || attributes[Stats.IsFrozen] > 0)
        {
            return;
        }

        var target = monster.CurrentMap.Terrain.GetPointAwayFrom(monster.Position, player.Position, distance);
        if (target != player.Position)
        {
            await player.MoveAsync(target).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Gets the point which is up to the specified distance away from the origin, in the direction from the origin to the start.
    /// It stops before fields which aren't walkable or part of a safezone.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="origin">The origin, from which the point is away.</param>
    /// <param name="start">The start point.</param>
    /// <param name="distance">The maximum number of fields.</param>
    /// <param name="canWalkOn">An optional additional condition for the fields, e.g. the walkable fields of a monster.</param>
    /// <returns>The point.</returns>
    public static Point GetPointAwayFrom(this GameMapTerrain terrain, Point origin, Point start, int distance, Func<Point, bool>? canWalkOn = null)
    {
        var direction = origin.GetDirectionTo(start);
        if (direction == Direction.Undefined)
        {
            direction = (Direction)Rand.NextInt(1, 9);
        }

        var target = start;
        for (var i = 0; i < distance; i++)
        {
            var next = target.CalculateTargetPoint(direction);
            if (!terrain.WalkMap[next.X, next.Y]
                || terrain.SafezoneMap[next.X, next.Y]
                || canWalkOn?.Invoke(next) == false)
            {
                break;
            }

            target = next;
        }

        return target;
    }
}
