// <copyright file="PlayerMovement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;

/// <summary>
/// The movement of a <see cref="Player"/>. It owns the <see cref="Walker"/> of the player,
/// determines its speed and validates the walk requests of the game client.
/// </summary>
internal sealed class PlayerMovement : IDisposable
{
    /// <summary>
    /// The movement speed of a walking (not running) character, in the unit which is used by the
    /// game client. It's the lower limit of the speed, e.g. at the safe zone.
    /// </summary>
    private const double WalkMovementSpeed = 12.0;

    private readonly Player _player;

    private readonly AsyncLock _moveLock = new();

    private readonly Walker _walker;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerMovement"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerMovement(Player player)
    {
        this._player = player;
        this._walker = new Walker(player, this.GetStepDelay);
    }

    /// <summary>
    /// Gets a value indicating whether the player is currently walking.
    /// </summary>
    public bool IsWalking => this._walker.CurrentTarget != default;

    /// <summary>
    /// Gets the current target of the walk.
    /// </summary>
    public Point WalkTarget => this._walker.CurrentTarget;

    /// <summary>
    /// Gets the delay between two steps, based on the current movement speed.
    /// </summary>
    public TimeSpan StepDelay => this.GetStepDelay(null);

    /// <summary>
    /// Moves the player instantly to the specified coordinate.
    /// </summary>
    /// <param name="target">The target.</param>
    public async ValueTask MoveAsync(Point target)
    {
        this._player.Logger.LogDebug("MoveAsync: Player is moving to {0}", target);
        await this._walker.StopAsync().ConfigureAwait(false);
        await this.ResetMovementStateAsync().ConfigureAwait(false);

        await this.MoveOnMapAsync(this._player.CurrentMap!, target, MoveType.Instant).ConfigureAwait(false);
        this._player.Logger.LogDebug("MoveAsync: Observer Count: {0}", this._player.Observers.Count);
    }

    /// <summary>
    /// Walks to the specified target coordinates using the specified steps.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="steps">The steps.</param>
    public async ValueTask WalkToAsync(Point target, Memory<WalkingStep> steps)
    {
        var currentMap = this._player.CurrentMap;
        if (currentMap is null)
        {
            return;
        }

        if (this._player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsFrozen] > 0 || attributes[Stats.IsStunned] > 0 || attributes[Stats.IsAsleep] > 0)
        {
            return;
        }

        if (steps.IsEmpty)
        {
            return;
        }

        await this._walker.StopAsync().ConfigureAwait(false);

        if (!await this.IsWalkRequestValidAsync(steps).ConfigureAwait(false))
        {
            return;
        }

        var requestedTarget = target;
        var walkableStepCount = GetWalkableStepCount(currentMap.Terrain, steps.Span);
        if (walkableStepCount == 0)
        {
            this._player.Logger.LogDebug(
                "WalkToAsync: Player requested to walk to {0}, but the first path step is blocked. Resynchronizing client.",
                requestedTarget);
            await this.ResynchronizeClientAsync().ConfigureAwait(false);
            return;
        }

        if (walkableStepCount < steps.Length)
        {
            steps = steps[..walkableStepCount];
            target = steps.Span[^1].To;
            this._player.Logger.LogDebug(
                "WalkToAsync: Truncated path from {0} to the last reachable position {1} because a later step is blocked.",
                requestedTarget,
                target);
        }

        this._player.Logger.LogDebug("WalkToAsync: Player is walking to {0}", target);

        var token = await this._walker.InitializeWalkToAsync(target, steps).ConfigureAwait(false);
        await this.MoveOnMapAsync(currentMap, target, MoveType.Walk).ConfigureAwait(false);
        await this._walker.StartWalkAsync(token).ConfigureAwait(false);

        this._player.Logger.LogDebug("WalkToAsync: Observer Count: {0}", this._player.Observers.Count);
    }

    /// <summary>
    /// Moves the player on the specified map, without stopping a running walk.
    /// </summary>
    /// <param name="map">The map on which the player is moved.</param>
    /// <param name="target">The target coordinates.</param>
    /// <param name="moveType">Type of the move.</param>
    public ValueTask MoveOnMapAsync(GameMap map, Point target, MoveType moveType)
    {
        return map.MoveAsync(this._player, target, this._moveLock, moveType);
    }

    /// <summary>
    /// Gets the directions of the next steps.
    /// </summary>
    /// <param name="directions">The directions.</param>
    /// <returns>The number of written directions.</returns>
    public ValueTask<int> GetDirectionsAsync(Memory<Direction> directions) => this._walker.GetDirectionsAsync(directions);

    /// <summary>
    /// Gets the next steps.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <returns>The number of written steps.</returns>
    public ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps) => this._walker.GetStepsAsync(steps);

    /// <summary>
    /// Stops the currently running walk.
    /// </summary>
    public ValueTask StopWalkingAsync() => this._walker.StopAsync();

    /// <summary>
    /// Resets the movement state of the anti-cheat plugins, e.g. after a teleport.
    /// </summary>
    public async ValueTask ResetMovementStateAsync()
    {
        if (this._player.GameContext.PlugInManager.GetPlugInPoint<ISpeedHackCheatCheckPlugIn>() is { } speedCheck)
        {
            await speedCheck.ResetMovementStateAsync(this._player).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._walker.Dispose();
    }

    private static int GetWalkableStepCount(GameMapTerrain terrain, ReadOnlySpan<WalkingStep> steps)
    {
        for (var index = 0; index < steps.Length; index++)
        {
            var target = steps[index].To;
            if (!terrain.WalkMap[target.X, target.Y])
            {
                return index;
            }
        }

        return steps.Length;
    }

    /// <summary>
    /// Determines whether the walk request of the game client is valid, that means it's neither
    /// detected as a cheat, nor does it start too far away from the current position.
    /// </summary>
    /// <param name="steps">The requested steps.</param>
    /// <returns><c>True</c>, if the walk may be performed; Otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> IsWalkRequestValidAsync(Memory<WalkingStep> steps)
    {
        var speedCheck = this._player.GameContext.PlugInManager.GetPlugInPoint<ISpeedHackCheatCheckPlugIn>();
        if (speedCheck is { })
        {
            var eventArgs = new SpeedHackCheckEventArgs();
            await speedCheck.WalkCheatCheckAsync(this._player, steps, eventArgs).ConfigureAwait(false);
            if (eventArgs.IsCheatDetected)
            {
                return false;
            }
        }

        var config = this._player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.Configuration;
        var maxAllowedWalkStartOffset = config?.MaxAllowedWalkStartOffset ?? 5;

        var startPoint = steps.Span[0].From;
        var currentPosition = this._player.Position;
        var startOffset = startPoint.EuclideanDistanceTo(currentPosition);
        if (startOffset <= maxAllowedWalkStartOffset)
        {
            return true;
        }

        this._player.Logger.LogWarning("WalkToAsync: Player requested to walk from {0}, but it's currently at {1} (offset {2} > {3}). Resynchronizing client.", startPoint, currentPosition, startOffset, maxAllowedWalkStartOffset);
        if (speedCheck is { })
        {
            await speedCheck.ResetMovementStateAsync(this._player).ConfigureAwait(false);
        }

        await this.ResynchronizeClientAsync().ConfigureAwait(false);
        return false;
    }

    /// <summary>
    /// Sends the current position back to the client, so that it can re-synchronize (rubberband).
    /// </summary>
    private ValueTask ResynchronizeClientAsync()
    {
        return this._player.InvokeViewPlugInAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(this._player, MoveType.Instant));
    }

    /// <summary>
    /// Gets the step delay depending on the equipped items and current movement effects.
    /// </summary>
    /// <param name="step">The walking step for which the delay is calculated.</param>
    /// <returns>The current step delay, depending on equipped items.</returns>
    private TimeSpan GetStepDelay(WalkingStep? step)
    {
        const double referenceFrameTimeMilliseconds = 40.0;
        const double terrainScale = 100.0;

        var speed = this.GetClientMovementSpeed(step?.From);
        var tileDistance = step is { } walkingStep ? walkingStep.From.EuclideanDistanceTo(walkingStep.To) : 1.0;
        var movementMilliseconds = terrainScale * Math.Max(1.0, tileDistance) / speed * referenceFrameTimeMilliseconds;

        return TimeSpan.FromMilliseconds(movementMilliseconds);
    }

    private double GetClientMovementSpeed(Point? position = null)
    {
        if (this.IsInClientSafezone(position))
        {
            return this.ApplyMovementSpeedFactor(WalkMovementSpeed);
        }

        var speedAttribute = this._player.Attributes?[Stats.IsUnderwater] > 0
            ? Stats.MovementSpeedUnderwater
            : Stats.MovementSpeed;
        var speed = this._player.Attributes?[speedAttribute] ?? 0;

        return this.ApplyMovementSpeedFactor(Math.Max(WalkMovementSpeed, speed));
    }

    private double ApplyMovementSpeedFactor(double speed)
    {
        var movementSpeedFactor = this._player.Attributes?[Stats.MovementSpeedFactor] ?? 1.0;

        return speed * (movementSpeedFactor > 0 ? movementSpeedFactor : 1.0);
    }

    private bool IsInClientSafezone(Point? position = null)
    {
        var checkedPosition = position ?? this._player.Position;
        return this._player.CurrentMap?.Terrain.SafezoneMap[checkedPosition.X, checkedPosition.Y] ?? false;
    }
}
