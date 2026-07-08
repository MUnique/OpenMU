// <copyright file="MovementHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Handles movement logic including walking, regrouping, and return-to-origin.
/// </summary>
public sealed class MovementHandler
{
    private const byte RegroupDistanceThreshold = 1;

    private readonly OfflinePlayer _player;
    private readonly IMuHelperSettings? _config;

    private DateTime? _outOfRangeSince;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovementHandler"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public MovementHandler(OfflinePlayer player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Gets the position to hunt around. Dynamic so bots can roam between hunting grounds.
    /// </summary>
    private Point OriginPosition => this._player.HuntingOrigin;

    /// <summary>
    /// Gets the hunting range in tiles.
    /// </summary>
    private byte HuntingRange => CombatHandler.CalculateHuntingRange(this._config);

    /// <summary>
    /// Returns the character to the original position if configured and distance/time thresholds are met.
    /// </summary>
    /// <returns>True, if the loop can continue; False, if a regrouping walk was initiated.</returns>
    public async ValueTask<bool> RegroupAsync()
    {
        if (this._config is null || !this._config.ReturnToOriginalPosition)
        {
            return true;
        }

        if (this.ShouldRegroup(out var distance))
        {
            await this.WalkToAsync(this.OriginPosition).ConfigureAwait(false);
            this._outOfRangeSince = null;
            return false;
        }

        if (distance <= RegroupDistanceThreshold)
        {
            this._outOfRangeSince = null;
        }

        return true;
    }

    /// <summary>
    /// Moves the player closer to a target within the specified range.
    /// </summary>
    /// <param name="target">The target to move closer to.</param>
    /// <param name="range">The range to stop within.</param>
    /// <returns>True, if a walk towards the target was started; false, if no path exists or walking is not possible.</returns>
    public async ValueTask<bool> MoveCloserToTargetAsync(IAttackable target, byte range)
    {
        if (this._player.CurrentMap is { } map && target.IsInRange(this.OriginPosition, this.HuntingRange))
        {
            var walkTarget = GetApproachPoint(map, this._player.Position, target.Position, range);
            return await this.WalkToAsync(walkTarget).ConfigureAwait(false);
        }

        return false;
    }

    /// <summary>
    /// Picks the point to walk to when closing in on a target: straight along the line towards it,
    /// stopping at attack range. The previous behavior re-randomized a point around the target every
    /// tick, which made the character zig-zag visibly towards its prey and re-path constantly.
    /// Falls back to a random point near the target when the straight-line point is not walkable.
    /// </summary>
    private static Point GetApproachPoint(GameMap map, Point from, Point to, byte stopRange)
    {
        var dx = to.X - from.X;
        var dy = to.Y - from.Y;
        var distance = Math.Max(Math.Abs(dx), Math.Abs(dy));
        if (distance <= stopRange)
        {
            return from;
        }

        var factor = (double)(distance - stopRange) / distance;
        var x = (byte)Math.Clamp(from.X + (int)Math.Round(dx * factor), 0, 255);
        var y = (byte)Math.Clamp(from.Y + (int)Math.Round(dy * factor), 0, 255);
        if (map.Terrain.WalkMap[x, y] && !map.Terrain.SafezoneMap[x, y])
        {
            return new Point(x, y);
        }

        return map.Terrain.GetRandomCoordinate(to, stopRange);
    }

    /// <summary>
    /// Walks the player to the specified target position.
    /// </summary>
    /// <param name="target">The target position to walk to.</param>
    /// <returns>True if the walk was successful; otherwise, false.</returns>
    private async ValueTask<bool> WalkToAsync(Point target)
    {
        if (this._player.IsWalking || this._player.CurrentMap is not { } map)
        {
            return false;
        }

        var pathFinder = await this._player.GameContext.PathFinderPool.GetAsync().ConfigureAwait(false);
        try
        {
            pathFinder.ResetPathFinder();
            var path = pathFinder.FindPath(this._player.Position, target, map.Terrain.AIgrid, false);
            if (path is null || path.Count == 0)
            {
                return false;
            }

            var stepsCount = Math.Min(path.Count, 16);
            var steps = new WalkingStep[stepsCount];
            for (int i = 0; i < stepsCount; i++)
            {
                var node = path[i];
                var prevPos = i == 0 ? this._player.Position : steps[i - 1].To;
                steps[i] = new WalkingStep(prevPos, node.Point, prevPos.GetDirectionTo(node.Point));
            }

            await this._player.WalkToAsync(target, steps).ConfigureAwait(false);
            return true;
        }
        finally
        {
            this._player.GameContext.PathFinderPool.Return(pathFinder);
        }
    }

    private bool ShouldRegroup(out double distance)
    {
        distance = this._player.GetDistanceTo(this.OriginPosition);
        if (distance <= RegroupDistanceThreshold)
        {
            return false;
        }

        this._outOfRangeSince ??= DateTime.UtcNow;
        var secondsAway = (DateTime.UtcNow - this._outOfRangeSince.Value).TotalSeconds;

        return secondsAway >= this._config!.MaxSecondsAway || distance > this.HuntingRange;
    }
}
