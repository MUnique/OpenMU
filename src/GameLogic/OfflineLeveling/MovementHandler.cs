// <copyright file="MovementHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Handles movement logic including walking, regrouping, and return-to-origin.
/// </summary>
public sealed class MovementHandler
{
    private const byte RegroupDistanceThreshold = 1;

    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperPlayerConfiguration? _config;
    private readonly Point _originPosition;
    private readonly Func<byte> _getHuntingRange;

    private int _secondsAwayFromOrigin;
    private int _tickCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MovementHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    /// <param name="originPosition">The original spawn position.</param>
    /// <param name="getHuntingRange">Function to get the current hunting range.</param>
    public MovementHandler(OfflineLevelingPlayer player, MuHelperPlayerConfiguration? config, Point originPosition, Func<byte> getHuntingRange)
    {
        this._player = player;
        this._config = config;
        this._originPosition = originPosition;
        this._getHuntingRange = getHuntingRange;
    }

    /// <summary>
    /// Updates the tick counter for timing-based operations.
    /// </summary>
    /// <param name="tickCounter">The current tick counter.</param>
    public void UpdateTickCounter(int tickCounter)
    {
        this._tickCounter = tickCounter;
    }

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

        var distance = this._player.GetDistanceTo(this._originPosition);
        if (distance > RegroupDistanceThreshold)
        {
            if (this._tickCounter == 0)
            {
                this._secondsAwayFromOrigin++;
            }
        }
        else
        {
            this._secondsAwayFromOrigin = 0;
            return true;
        }

        if (this._secondsAwayFromOrigin >= this._config.MaxSecondsAway || distance > this._getHuntingRange())
        {
            await this.WalkToAsync(this._originPosition).ConfigureAwait(false);
            this._secondsAwayFromOrigin = 0;
            return false;
        }

        return true;
    }

    /// <summary>
    /// Moves the player closer to a target within the specified range.
    /// </summary>
    /// <param name="target">The target to move closer to.</param>
    /// <param name="range">The range to stop within.</param>
    public async ValueTask MoveCloserToTargetAsync(IAttackable target, byte range)
    {
        if (target.IsInRange(this._originPosition, this._getHuntingRange()))
        {
            var walkTarget = this._player.CurrentMap!.Terrain.GetRandomCoordinate(target.Position, range);
            await this.WalkToAsync(walkTarget).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Walks the player to the specified target position.
    /// </summary>
    /// <param name="target">The target position to walk to.</param>
    /// <returns>True if the walk was successful; otherwise, false.</returns>
    public async ValueTask<bool> WalkToAsync(Point target)
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
}
