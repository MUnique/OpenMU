// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Class which manages walking for instances of <see cref="ISupportWalk"/>.
/// </summary>
public sealed class Walker : IDisposable
{
    private readonly ISupportWalk _walkSupporter;
    private readonly Func<TimeSpan> _stepDelay;
    private readonly Queue<WalkingStep> _nextSteps = new (5);
    private readonly ReaderWriterLockSlim _walkLock;
    private CancellationTokenSource? _walkCts;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Walker" /> class.
    /// </summary>
    /// <param name="walkSupporter">The walk supporter.</param>
    /// <param name="stepDelay">The delay between performing a step.</param>
    public Walker(ISupportWalk walkSupporter, Func<TimeSpan> stepDelay)
    {
        this._walkSupporter = walkSupporter;
        this._stepDelay = stepDelay;
        this._walkLock = new ReaderWriterLockSlim();
    }

    /// <summary>
    /// Gets the current walk target.
    /// </summary>
    public Point CurrentTarget { get; private set; }

    /// <summary>
    /// Starts to walk to the specified target with the specified steps.
    /// </summary>
    /// <param name="target">The target coordinates.</param>
    /// <param name="steps">The steps.</param>
    public void WalkTo(Point target, Span<WalkingStep> steps)
    {
        if (this._isDisposed)
        {
            return;
        }

        this._walkLock.EnterWriteLock();
        try
        {
            this.CurrentTarget = target;
            this._nextSteps.Clear();
            foreach (var step in steps)
            {
                this._nextSteps.Enqueue(step);
            }

            var cts = new CancellationTokenSource();
            this._walkCts = cts;
            Task.Run(async () => await this.WalkLoop(cts.Token), cts.Token);
        }
        finally
        {
            this._walkLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Gets the directions of the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="directions">The directions.</param>
    /// <returns>The number of written directions.</returns>
    public int GetDirections(Span<Direction> directions)
    {
        var count = 0;
        this._walkLock.EnterReadLock();
        try
        {
            foreach (var direction in this._nextSteps.Reverse().Select(step => step.Direction))
            {
                directions[count] = direction;
                count++;
            }
        }
        finally
        {
            this._walkLock.ExitReadLock();
        }

        return count;
    }

    /// <summary>
    /// Gets the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <returns>The number of written steps.</returns>
    public int GetSteps(Span<WalkingStep> steps)
    {
        var count = 0;
        this._walkLock.EnterReadLock();
        try
        {
            foreach (var direction in this._nextSteps.Reverse())
            {
                steps[count] = direction;
                count++;
            }
        }
        finally
        {
            this._walkLock.ExitReadLock();
        }

        return count;
    }

    /// <summary>
    /// Stops the walk.
    /// </summary>
    public void Stop()
    {
        this._walkLock.EnterWriteLock();
        try
        {
            if (this._walkCts != null)
            {
                this._walkCts.Cancel(false);
                this._walkCts.Dispose();
                this._walkCts = null;
                this._nextSteps.Clear();
                this.CurrentTarget = default;
            }
        }
        finally
        {
            this._walkLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this._isDisposed = true;
        if (this._walkCts != null)
        {
            this.Stop();
        }

        this._walkLock.Dispose();
    }

    private async Task WalkLoop(CancellationToken cancellationToken)
    {
        var delay = this._stepDelay().Subtract(TimeSpan.FromMilliseconds(50));

        // Task.Delay might take longer than we specify. We need to compensate that.
        var lastOffset = TimeSpan.Zero;
        while (!cancellationToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            this.WalkStep(cancellationToken);

            var nextDelay = delay - lastOffset;
            if (nextDelay > TimeSpan.Zero)
            {
                // ReSharper disable once MethodSupportsCancellation if we pass this, we get a lot of unwanted TaskCancelledExceptions, so we rather wait.
                await Task.Delay(nextDelay).ConfigureAwait(false);
                sw.Stop();
                lastOffset = sw.Elapsed - delay;
            }
            else
            {
                lastOffset = nextDelay.Negate();
            }
        }
    }

    /// <summary>
    /// Performs the next step of a walk.
    /// </summary>
    private void WalkStep(CancellationToken cancellationToken)
    {
        try
        {
            if (this._isDisposed)
            {
                Debug.WriteLine("walker already disposed");
                return;
            }

            bool stop;
            this._walkLock.EnterReadLock();
            try
            {
                stop = !cancellationToken.IsCancellationRequested && this.ShouldWalkerStop();
            }
            finally
            {
                this._walkLock.ExitReadLock();
            }

            if (stop)
            {
                this.Stop();
            }

            // Update new coords
            this._walkLock.EnterWriteLock();
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                this.WalkNextStepIfStepAvailable();
            }
            finally
            {
                this._walkLock.ExitWriteLock();
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private void WalkNextStepIfStepAvailable()
    {
        if (!this.ShouldWalkerStop())
        {
            var nextStep = this._nextSteps.Dequeue();
            this._walkSupporter.Position = nextStep.To;

            if (this._walkSupporter is IRotatable rotatable)
            {
                rotatable.Rotation = nextStep.Direction;
            }
        }
    }

    private bool ShouldWalkerStop() => !((this._walkSupporter as IAttackable)?.IsActive() ?? false) || this._nextSteps.Count <= 0;
}