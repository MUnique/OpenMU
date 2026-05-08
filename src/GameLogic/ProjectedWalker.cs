// <copyright file="ProjectedWalker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System;
using System.Diagnostics;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Projects walking state based on the elapsed time since a walk started.
/// </summary>
internal sealed class ProjectedWalker
{
    private readonly object _walkStateLock = new();
    private readonly WalkingStep[] _currentWalkSteps = new WalkingStep[16];
    private readonly TimeSpan[] _currentWalkStepEndTimes = new TimeSpan[16];
    private readonly Func<Point> _getStoredPosition;
    private readonly Func<Point, bool> _setStoredPosition;
    private readonly Func<WalkingStep?, TimeSpan> _getStepDuration;
    private int _currentWalkStepCount;
    private long _walkStartedTimestamp;
    private Point _walkTarget;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectedWalker"/> class.
    /// </summary>
    /// <param name="getStoredPosition">Gets the persisted position of the walk supporter.</param>
    /// <param name="setStoredPosition">Sets the persisted position of the walk supporter and returns if it changed.</param>
    /// <param name="getStepDuration">Gets the duration of the specified walking step.</param>
    public ProjectedWalker(
        Func<Point> getStoredPosition,
        Func<Point, bool> setStoredPosition,
        Func<WalkingStep?, TimeSpan> getStepDuration)
    {
        this._getStoredPosition = getStoredPosition;
        this._setStoredPosition = setStoredPosition;
        this._getStepDuration = getStepDuration;
    }

    /// <summary>
    /// Gets a value indicating whether a walk is currently projected.
    /// </summary>
    public bool IsWalking
    {
        get
        {
            this.RefreshStoredPosition();
            lock (this._walkStateLock)
            {
                return this._currentWalkStepCount > 0;
            }
        }
    }

    /// <summary>
    /// Gets the current walk target coordinate.
    /// </summary>
    public Point CurrentTarget
    {
        get
        {
            this.RefreshStoredPosition();
            lock (this._walkStateLock)
            {
                return this._walkTarget;
            }
        }
    }

    /// <summary>
    /// Gets or sets the projected position.
    /// </summary>
    public Point Position
    {
        get
        {
            this.RefreshStoredPosition();
            lock (this._walkStateLock)
            {
                return this.GetProjectedPositionNoLock(out _);
            }
        }

        set
        {
            lock (this._walkStateLock)
            {
                this.ClearNoLock();
                this._setStoredPosition(value);
            }
        }
    }

    /// <summary>
    /// Initializes a projected walk to the specified target.
    /// </summary>
    /// <param name="target">The target coordinate.</param>
    /// <param name="steps">The walk steps.</param>
    public void Initialize(Point target, Memory<WalkingStep> steps)
    {
        if (steps.Length > this._currentWalkSteps.Length)
        {
            throw new ArgumentException("Maximum number of steps (16) exceeded.", nameof(steps));
        }

        lock (this._walkStateLock)
        {
            var elapsed = TimeSpan.Zero;
            this.ClearNoLock();

            for (var i = 0; i < steps.Length; i++)
            {
                var step = steps.Span[i];
                this._currentWalkSteps[i] = step;
                elapsed += this._getStepDuration(step);
                this._currentWalkStepEndTimes[i] = elapsed;
            }

            this._walkTarget = target;
            this._walkStartedTimestamp = Stopwatch.GetTimestamp();
            this._currentWalkStepCount = steps.Length;
        }
    }

    /// <summary>
    /// Stops the projected walk and persists the current projected position.
    /// </summary>
    public void Stop()
    {
        lock (this._walkStateLock)
        {
            var currentPosition = this.GetProjectedPositionNoLock(out _);
            this.ClearNoLock();
            this._setStoredPosition(currentPosition);
        }
    }

    /// <summary>
    /// Gets the directions of the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="directions">The directions.</param>
    /// <returns>The number of written directions.</returns>
    public ValueTask<int> GetDirectionsAsync(Memory<Direction> directions)
    {
        this.RefreshStoredPosition();
        var count = 0;
        lock (this._walkStateLock)
        {
            for (var index = this._currentWalkStepCount - 1; index >= 0; index--)
            {
                var step = this._currentWalkSteps[index];
                directions.Span[count] = step.Direction;
                count++;
            }
        }

        return ValueTask.FromResult(count);
    }

    /// <summary>
    /// Gets the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <returns>The number of written steps.</returns>
    public ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps)
    {
        this.RefreshStoredPosition();
        var count = 0;
        lock (this._walkStateLock)
        {
            for (var index = this._currentWalkStepCount - 1; index >= 0; index--)
            {
                var step = this._currentWalkSteps[index];
                steps.Span[count] = step;
                count++;
            }
        }

        return ValueTask.FromResult(count);
    }

    /// <summary>
    /// Validates that the specified steps represent a coherent walk over walkable terrain.
    /// </summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="steps">The steps.</param>
    /// <param name="target">The expected target.</param>
    /// <param name="invalidPoint">The invalid point.</param>
    /// <returns><c>true</c>, if the steps are valid; otherwise, <c>false</c>.</returns>
    public static bool TryValidateWalkSteps(GameMapTerrain terrain, Memory<WalkingStep> steps, Point target, out Point invalidPoint)
    {
        var span = steps.Span;
        var expectedFrom = span[0].From;
        invalidPoint = expectedFrom;

        if (!terrain.WalkMap[expectedFrom.X, expectedFrom.Y])
        {
            return false;
        }

        foreach (var step in span)
        {
            if (step.From != expectedFrom || step.Direction == Direction.Undefined)
            {
                invalidPoint = step.From;
                return false;
            }

            var calculatedTarget = step.From.CalculateTargetPoint(step.Direction);
            if (step.To != calculatedTarget || !terrain.WalkMap[step.To.X, step.To.Y])
            {
                invalidPoint = step.To;
                return false;
            }

            expectedFrom = step.To;
        }

        invalidPoint = expectedFrom;
        return expectedFrom == target;
    }

    private void RefreshStoredPosition()
    {
        lock (this._walkStateLock)
        {
            if (this._currentWalkStepCount == 0)
            {
                return;
            }

            var currentPosition = this.GetProjectedPositionNoLock(out var completed);
            if (completed)
            {
                this.ClearNoLock();
                this._setStoredPosition(currentPosition);
            }
        }
    }

    private Point GetProjectedPositionNoLock(out bool completed)
    {
        completed = false;
        if (this._currentWalkStepCount == 0)
        {
            return this._getStoredPosition();
        }

        var elapsed = Stopwatch.GetElapsedTime(this._walkStartedTimestamp);
        for (var i = 0; i < this._currentWalkStepCount; i++)
        {
            if (elapsed < this._currentWalkStepEndTimes[i])
            {
                return this._currentWalkSteps[i].From;
            }
        }

        completed = true;
        return this._walkTarget;
    }

    private void ClearNoLock()
    {
        this._currentWalkStepCount = 0;
        this._walkStartedTimestamp = 0;
        this._walkTarget = default;
    }
}
