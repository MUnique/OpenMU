// <copyright file="PlayerMovement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Threading;
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

    /// <summary>
    /// The distance a client-reported position may be away from the server's position when no
    /// anti-cheat plugin is configured. It mirrors the default of
    /// <see cref="SpeedHackDetectConfiguration.MaxAllowedWalkStartOffset"/>.
    /// </summary>
    private const int DefaultMaxAllowedWalkStartOffset = 5;

    /// <summary>
    /// The distance of a diagonal neighbour tile. A reported position this close to ours is
    /// the client rounding a position between two tiles.
    /// </summary>
    private const double NextTileDistance = 1.5;

    /// <summary>
    /// The extra tiles a client may be ahead of the server due to network lag.
    /// </summary>
    private const double PositionLagSlack = 1.0;

    /// <summary>
    /// The total distance of small position corrections (up to <see cref="NextTileDistance"/> each)
    /// which may stay uncharged to the walk speed check at a time.
    /// </summary>
    private const double MaxUnchargedCorrectionDistance = 3.0;

    /// <summary>
    /// The share of the walking speed at which uncharged corrections are forgiven over time.
    /// It's well below 1, otherwise a client could gain a free tile with every step.
    /// </summary>
    private const double UnchargedCorrectionRecoveryFactor = 0.5;

    private readonly Player _player;

    private readonly object _correctionLock = new();

    private readonly AsyncLock _moveLock = new();

    private readonly Walker _walker;

    /// <summary>
    /// The timestamp of the last position change of the player. It's written by the walker and by the
    /// packet handling, so it's only accessed through <see cref="MarkPositionChanged"/> and
    /// <see cref="GetTimeSincePositionChange"/>.
    /// </summary>
    private long _lastPositionChangeTimestamp = TimeProvider.System.GetTimestamp();

    /// <summary>
    /// The distance of small position corrections which was not charged to the walk speed check yet.
    /// </summary>
    private double _unchargedCorrectionDistance;

    private long _lastCorrectionTimestamp = TimeProvider.System.GetTimestamp();

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerMovement"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerMovement(Player player)
    {
        this._player = player;
        this._walker = new Walker(player, this.GetStepDelay, paceFirstStepAcrossWalks: true);
        this._walker.StepPerformed += this.NotifyWalkStepPerformed;
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
        this.ResetUnchargedCorrections();

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

        if (!await this.IsWalkRequestValidAsync(steps).ConfigureAwait(false))
        {
            await this.RejectWalkAndResynchronizeClientAsync().ConfigureAwait(false);
            return;
        }

        // The path is checked first, so a rejected path can't change the server position.
        var requestedTarget = target;
        var walkableStepCount = GetWalkableStepCount(currentMap.Terrain, steps.Span);
        if (walkableStepCount == 0)
        {
            this._player.Logger.LogDebug(
                "WalkToAsync: Player requested to walk to {0}, but the first path step is blocked. Keeping the server position.",
                requestedTarget);
            await this.RejectWalkAndResynchronizeClientAsync().ConfigureAwait(false);
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

        if (!await this.TrySynchronizeServerWithClientPositionAsync(steps.Span[0].From).ConfigureAwait(false))
        {
            await this.RejectWalkAndResynchronizeClientAsync().ConfigureAwait(false);
            return;
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
    /// <param name="notifyMovedObject">
    /// If set to <c>true</c>, the player is notified about its own move as well.
    /// </param>
    public async ValueTask MoveOnMapAsync(GameMap map, Point target, MoveType moveType, bool notifyMovedObject = true)
    {
        await map.MoveAsync(this._player, target, this._moveLock, moveType, notifyMovedObject).ConfigureAwait(false);
        if (moveType is MoveType.Instant or MoveType.Teleport)
        {
            this.MarkPositionChanged();
        }
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
    /// Ends a running walk early, at the position the game client reports it stopped at, and tells the
    /// other players in view about it.
    /// </summary>
    /// <param name="stopPoint">The position the client reports it stopped the walk at.</param>
    /// <remarks>
    /// The client interrupts a walk between two tiles when the player attacks or casts a skill, and
    /// reports that as a walk request without any steps. Without acting on it, our walker carries on to
    /// the original target: the player ends up a few tiles from where its client has it, which in turn
    /// makes the next walk request start too far away and rubberband the client, and everybody else in
    /// view sees it walk on past the attack and then get pulled back.
    /// The reported position is taken when it belongs to the walk which is currently running and is
    /// no further from our position than the time since our last step allows. A position next to ours
    /// is also accepted, because the client rounds positions between two tiles. Anything else ends the
    /// walk where we are and tells everybody, the player included, so the difference is closed at once.
    /// Neither can be used to travel: the player never gets further than the lag allows.
    /// The same correction is sent when we already stood still and the client reports another tile.
    /// The moving player is not notified when its position is taken: it told us where it stopped, and
    /// confirming it would make its client pull the character onto the tile centre and drop the attack
    /// animation it just started.
    /// </remarks>
    public async ValueTask StopWalkAtAsync(Point stopPoint)
    {
        if (this._player.CurrentMap is not { } currentMap)
        {
            return;
        }

        if (!this.IsWalking)
        {
            await this.ResynchronizeIfApartAsync(currentMap, stopPoint).ConfigureAwait(false);
            return;
        }

        if (await this.IsStopPointAcceptableAsync(stopPoint).ConfigureAwait(false)
            || this.IsNextToCurrentPosition(currentMap, stopPoint))
        {
            this._player.Logger.LogDebug("StopWalkAtAsync: Player stopped its walk to {0} at {1}", this.WalkTarget, stopPoint);
            await this._walker.StopAsync().ConfigureAwait(false);
            var previousPosition = this._player.Position;
            await this.MoveOnMapAsync(currentMap, stopPoint, MoveType.Instant, notifyMovedObject: false).ConfigureAwait(false);
            this.ReportTakenOverPosition(previousPosition, stopPoint);
            return;
        }

        var walkTarget = this.WalkTarget;
        await this._walker.StopAsync().ConfigureAwait(false);
        var position = this._player.Position;
        this._player.Logger.LogWarning("StopWalkAtAsync: Player reported it stopped walking at {0}, which can't be taken (walk to {1}). Ended the walk at {2} and resynchronized the client.", stopPoint, walkTarget, position);
        await this.MoveOnMapAsync(currentMap, position, MoveType.Instant).ConfigureAwait(false);
    }

    /// <summary>
    /// Resets the movement state of registered plugins, e.g. after a teleport.
    /// </summary>
    public async ValueTask ResetMovementStateAsync()
    {
        if (this._player.GameContext.PlugInManager.GetPlugInPoint<IMovementStateResetPlugIn>() is { } resetPlugIn)
        {
            await resetPlugIn.ResetMovementStateAsync(this._player).ConfigureAwait(false);
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

    private void MarkPositionChanged()
    {
        Volatile.Write(ref this._lastPositionChangeTimestamp, TimeProvider.System.GetTimestamp());
    }

    private TimeSpan GetTimeSincePositionChange()
    {
        return TimeProvider.System.GetElapsedTime(Volatile.Read(ref this._lastPositionChangeTimestamp));
    }

    /// <summary>
    /// Reports a position which the server took over from the game client to the registered walk step
    /// plugins, as if the server had walked there. Without this, a client could cover distance by
    /// reporting positions further ahead with every walk request, which the walk speed check
    /// would never see, because it only counts the steps which the server executed itself.
    /// </summary>
    /// <param name="from">The position of the server before it took over the client position.</param>
    /// <param name="to">The position of the client which was taken over.</param>
    /// <remarks>
    /// Differences up to <see cref="NextTileDistance"/> are not reported right away: the server executes a step
    /// at its start, so it is regularly a tile ahead of or behind the client, and the client rounds between tiles.
    /// Counting these would make legitimately frequent walk requests look like speed hacks.
    /// But they can't stay free in sum either, so they are collected up to a bound
    /// (see <see cref="TryAbsorbSmallCorrection"/>) and reported as soon as it is exceeded.
    /// </remarks>
    private void ReportTakenOverPosition(Point from, Point to)
    {
        var distance = from.EuclideanDistanceTo(to);
        if (distance <= 0)
        {
            return;
        }

        if (distance <= NextTileDistance && this.TryAbsorbSmallCorrection(distance))
        {
            return;
        }

        var step = new WalkingStep(from, to, Direction.Undefined);
        this.NotifyWalkStepPerformed(step, this.GetStepDelay(step));
    }

    /// <summary>
    /// Collects the distance of a small position correction, if the total of uncharged corrections
    /// stays within <see cref="MaxUnchargedCorrectionDistance"/>. The total is forgiven over time, at a
    /// fraction of the walking speed.
    /// </summary>
    /// <param name="distance">The distance of the correction.</param>
    /// <returns><c>True</c>, if the correction may stay uncharged; Otherwise, <c>false</c>.</returns>
    private bool TryAbsorbSmallCorrection(double distance)
    {
        var now = TimeProvider.System.GetTimestamp();
        var stepDelayMs = Math.Max(1.0, this.StepDelay.TotalMilliseconds);
        lock (this._correctionLock)
        {
            var elapsedMs = TimeProvider.System.GetElapsedTime(this._lastCorrectionTimestamp, now).TotalMilliseconds;
            this._lastCorrectionTimestamp = now;
            var recovered = elapsedMs / stepDelayMs * UnchargedCorrectionRecoveryFactor;
            this._unchargedCorrectionDistance = Math.Max(0.0, this._unchargedCorrectionDistance - recovered);
            if (this._unchargedCorrectionDistance + distance > MaxUnchargedCorrectionDistance)
            {
                return false;
            }

            this._unchargedCorrectionDistance += distance;
            return true;
        }
    }

    private void ResetUnchargedCorrections()
    {
        lock (this._correctionLock)
        {
            this._unchargedCorrectionDistance = 0.0;
            this._lastCorrectionTimestamp = TimeProvider.System.GetTimestamp();
        }
    }

    /// <summary>
    /// Notifies the registered plugins about a walk step which was actually executed by the server.
    /// </summary>
    /// <param name="step">The executed step.</param>
    /// <param name="stepDelay">The delay which was used for pacing the step.</param>
    private void NotifyWalkStepPerformed(WalkingStep step, TimeSpan stepDelay)
    {
        this.MarkPositionChanged();
        this._player.GameContext.PlugInManager.GetPlugInPoint<IWalkStepPlugIn>()?.WalkStepPerformed(this._player, step, stepDelay);
    }

    /// <summary>
    /// Determines whether a registered validation plugin allows the walk request.
    /// </summary>
    /// <param name="steps">The requested steps.</param>
    /// <returns><c>True</c>, if the walk may be performed; Otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> IsWalkRequestValidAsync(Memory<WalkingStep> steps)
    {
        if (this._player.GameContext.PlugInManager.GetPlugInPoint<IWalkRequestValidationPlugIn>() is not { } validator)
        {
            return true;
        }

        var eventArgs = new WalkRequestValidationArgs();
        await validator.ValidateWalkRequestAsync(this._player, steps, eventArgs).ConfigureAwait(false);
        return eventArgs.IsWalkAllowed;
    }

    /// <summary>
    /// Determines whether a walk may be ended at the position the client reports. It has to be a
    /// position of the walk which is currently running.
    /// </summary>
    /// <param name="stopPoint">The position the client reports it stopped the walk at.</param>
    /// <returns><c>True</c>, if the walk may be ended there; Otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> IsStopPointAcceptableAsync(Point stopPoint)
    {
        if (stopPoint == this._player.Position)
        {
            return true;
        }

        if (!await this._walker.IsPointOfCurrentWalkAsync(stopPoint).ConfigureAwait(false))
        {
            this._player.Logger.LogDebug("StopWalkAtAsync: Player reported it stopped walking at {0}, which is not a position of its current walk to {1}. Not taken as it is.", stopPoint, this.WalkTarget);
            return false;
        }

        var offset = stopPoint.EuclideanDistanceTo(this._player.Position);
        var maxOffset = this.GetMaxAllowedLagOffset();
        if (offset > maxOffset)
        {
            this._player.Logger.LogWarning("StopWalkAtAsync: Player reported it stopped walking at {0}, which is {1:F1} tiles away from the server position, exceeding the allowed offset of {2:F1}. Keeping the walk.", stopPoint, offset, maxOffset);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether the reported position is one tile (or less) away from our position
    /// and can be walked on, which is just the client rounding a position between two tiles.
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="point">The reported position.</param>
    /// <returns><c>True</c>, if the position may be taken as it is; Otherwise, <c>false</c>.</returns>
    private bool IsNextToCurrentPosition(GameMap map, Point point)
    {
        return point.EuclideanDistanceTo(this._player.Position) <= NextTileDistance
            && map.Terrain.WalkMap[point.X, point.Y];
    }

    /// <summary>
    /// The player stood still and its client reports another position. If it's further than
    /// the next tile, everybody (the player included) is told where we have it.
    /// </summary>
    /// <param name="map">The current map.</param>
    /// <param name="reportedPoint">The position the client reports.</param>
    private async ValueTask ResynchronizeIfApartAsync(GameMap map, Point reportedPoint)
    {
        var position = this._player.Position;
        if (reportedPoint.EuclideanDistanceTo(position) <= NextTileDistance)
        {
            return;
        }

        this._player.Logger.LogWarning("StopWalkAtAsync: Player reported it stopped at {0}, but it's standing at {1}. Resynchronized the client.", reportedPoint, position);
        await this.MoveOnMapAsync(map, position, MoveType.Instant).ConfigureAwait(false);
    }

    /// <summary>
    /// Moves the server-side position to the position reported by the client. The client is not
    /// notified, because it already shows this position; other observers are notified.
    /// Positions on blocked terrain are rejected, so a client cannot move the server onto a wall.
    /// </summary>
    /// <param name="clientPosition">The position reported by the client.</param>
    /// <returns><c>true</c> if the server position now matches the client; otherwise, <c>false</c>.</returns>
    private async ValueTask<bool> TrySynchronizeServerWithClientPositionAsync(Point clientPosition)
    {
        if (this._player.CurrentMap is not { } currentMap)
        {
            return false;
        }

        if (this._player.Position == clientPosition)
        {
            return true;
        }

        var offset = clientPosition.EuclideanDistanceTo(this._player.Position);
        var isOnCurrentWalk = this.IsWalking
            && await this._walker.IsPointOfCurrentWalkAsync(clientPosition).ConfigureAwait(false);

        // A diagonal neighbour tile is always ~1.41 tiles away, e.g. when the client turns. The lag bound
        // is lower than that right after a step, so the rounding tolerance is the lower limit.
        var maxOffset = Math.Max(NextTileDistance, isOnCurrentWalk ? this.GetMaxAllowedLagOffset() : this.GetMaxAllowedWalkStartOffset());
        if (offset > maxOffset)
        {
            this._player.Logger.LogWarning(
                "Client reported a walk position {0} which is {1:F1} tiles away from the server position {2}, exceeding the allowed offset of {3:F1}. Keeping the server position.",
                clientPosition,
                offset,
                this._player.Position,
                maxOffset);
            return false;
        }

        if (!currentMap.Terrain.WalkMap[clientPosition.X, clientPosition.Y])
        {
            this._player.Logger.LogWarning(
                "Client reported a walk position on blocked terrain ({0}); keeping the server position at {1}.",
                clientPosition,
                this._player.Position);
            return false;
        }

        await this._walker.StopAsync().ConfigureAwait(false);
        var previousPosition = this._player.Position;
        this._player.Logger.LogDebug(
            "Synchronizing server position with client position {0} (server was at {1}).",
            clientPosition,
            previousPosition);
        await this.MoveOnMapAsync(currentMap, clientPosition, MoveType.Instant, notifyMovedObject: false).ConfigureAwait(false);
        this.ReportTakenOverPosition(previousPosition, clientPosition);
        return true;
    }

    /// <summary>
    /// Rejects a walk (e.g. a rejected speed check, a blocked path or a start position which can't be
    /// adopted), stops the running walk and pulls the client back to the server position.
    /// </summary>
    private async ValueTask RejectWalkAndResynchronizeClientAsync()
    {
        await this._walker.StopAsync().ConfigureAwait(false);
        await this.ResetMovementStateAsync().ConfigureAwait(false);
        await this.ResynchronizeClientAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the maximum offset which is plausible while the server is behind the client during a walk.
    /// The client can't be farther ahead than the time which passed since the last server-side step.
    /// </summary>
    /// <returns>The maximum offset in tiles.</returns>
    private double GetMaxAllowedLagOffset()
    {
        var elapsed = this.GetTimeSincePositionChange();
        var stepDelay = this.StepDelay.TotalMilliseconds;
        return PositionLagSlack + (elapsed.TotalMilliseconds / Math.Max(1.0, stepDelay));
    }

    /// <summary>
    /// Gets the maximum distance a client-reported position may be away from the server position
    /// without an ongoing walk which could explain the lag.
    /// </summary>
    /// <returns>The maximum allowed offset in tiles.</returns>
    private int GetMaxAllowedWalkStartOffset()
    {
        var config = this._player.GameContext.FeaturePlugIns.GetPlugIn<SpeedHackDetectPlugIn>()?.Configuration;
        return config?.MaxAllowedWalkStartOffset ?? DefaultMaxAllowedWalkStartOffset;
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
