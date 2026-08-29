// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Class which manages walking for instances of <see cref="ISupportWalk"/>.
/// </summary>
public sealed class Walker : IDisposable
{
    private readonly ISupportWalk _walkSupporter;
    private readonly Queue<WalkingStep> _nextSteps = new(5);

    /// <summary>
    /// This array keeps all steps of the current walk.
    /// </summary>
    private readonly WalkingStep[] _currentWalkSteps = new WalkingStep[16];

    private readonly AsyncReaderWriterLock _walkLock;

    /// <summary>
    /// The number of steps which are stored in <see cref="_currentWalkSteps"/> for the current walk.
    /// </summary>
    private int _currentWalkStepCount;
    private CancellationTokenSource? _walkCts;
    private bool _isDisposed;
    private Guid _currentWalkToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="Walker" /> class.
    /// </summary>
    /// <param name="walkSupporter">The walk supporter.</param>
    /// <param name="stepDelay">The delay between performing a step.</param>
    public Walker(ISupportWalk walkSupporter, Func<WalkingStep?, TimeSpan> stepDelay)
    {
        this._walkSupporter = walkSupporter;
        this.StepDelay = stepDelay;
        this._walkLock = new AsyncReaderWriterLock();
    }

    /// <summary>
    /// Gets the current walk target.
    /// </summary>
    public Point CurrentTarget { get; private set; }

    private Func<WalkingStep?, TimeSpan> StepDelay { get; }

    /// <summary>
    /// Initializes a new walk to the specified target with the specified steps.
    /// </summary>
    /// <param name="target">The target coordinates.</param>
    /// <param name="steps">The steps.</param>
    /// <returns>A walk token, if it was initialized.</returns>
    public async ValueTask<Guid> InitializeWalkToAsync(Point target, Memory<WalkingStep> steps)
    {
        if (this._isDisposed)
        {
            return Guid.Empty;
        }

        if (steps.Length > 16)
        {
            throw new ArgumentException("Maximum number of steps (16) exceeded.", nameof(steps));
        }

        using var writerLock = await this._walkLock.WriterLockAsync();

        void EnqueueSteps()
        {
            this._currentWalkStepCount = steps.Length;
            this._nextSteps.Clear();
            int i = steps.Length - 1;
            foreach (var step in steps.Span)
            {
                this._nextSteps.Enqueue(step);
                this._currentWalkSteps[i] = step;
                i--;
            }
        }

        // End the running walk before the new steps go into the queue: otherwise a loop of the previous
        // walk can dequeue and execute the new walk's steps in the window between this method and
        // StartWalkAsync, moving the supporter along the new path early and leaving the new loop one step
        // short. We hold the writer lock here, so this is safe to do without releasing it.
        await this.StopCurrentWalkAsync().ConfigureAwait(false);

        this.CurrentTarget = target;
        EnqueueSteps();

        var walkToken = Guid.CreateVersion7();
        this._currentWalkToken = walkToken;
        return walkToken;
    }

    /// <summary>
    /// Starts the previously initialized walk.
    /// </summary>
    /// <param name="walkToken">The walk token.</param>
    public async ValueTask StartWalkAsync(Guid walkToken)
    {
        using var writerLock = await this._walkLock.WriterLockAsync();

        if (walkToken != this._currentWalkToken)
        {
            // Another walk request was initialized in the meantime.
            return;
        }

        if (this._walkCts is { } previousCts)
        {
            // A walk loop is still running for a previous walk. Replacing the source without cancelling it
            // first orphans that loop: its token is the only thing which can end it, and once the field
            // points at the new source nobody holds a reference to the old one any more. The orphan would
            // then run forever - see the note in WalkLoopAsync - burning a core and keeping the walk
            // supporter alive.
            await previousCts.CancelAsync().ConfigureAwait(false);
            previousCts.Dispose();
        }

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        this._walkCts = cts;

        // The loop is handed its own source as well as its token: it needs the source to recognize whether
        // the walk it is running is still the current one before it stops anything. It cannot read that back
        // from the field later (that is the whole point), nor from cts.Token once the source is disposed.
        _ = Task.Run(async () => await this.WalkLoopAsync(cts, cancellationToken).ConfigureAwait(false), cancellationToken);
    }

    /// <summary>
    /// Gets the directions of the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="directions">The directions.</param>
    /// <returns>The number of written directions.</returns>
    public async ValueTask<int> GetDirectionsAsync(Memory<Direction> directions)
    {
        var count = 0;
        using var readerLock = await this._walkLock.ReaderLockAsync();
        foreach (var direction in this._currentWalkSteps[..this._currentWalkStepCount].Select(step => step.Direction))
        {
            directions.Span[count] = direction;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Gets the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <returns>The number of written steps.</returns>
    public async ValueTask<int> GetStepsAsync(Memory<WalkingStep> steps)
    {
        var count = 0;
        using var readerLock = await this._walkLock.ReaderLockAsync();
        foreach (var direction in this._currentWalkSteps[..this._currentWalkStepCount])
        {
            steps.Span[count] = direction;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Stops the walk which is currently running - whichever one that is. This is what outside callers
    /// want: they are ending "the walk of this object", not one particular walk they started earlier.
    /// A walk loop stopping itself must use <see cref="StopOwnWalkAsync"/> instead.
    /// </summary>
    public async ValueTask StopAsync()
    {
        using var writeLock = await this._walkLock.WriterLockAsync();

        await this.StopCurrentWalkAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        this._isDisposed = true;
        if (this._walkCts is { IsCancellationRequested: false })
        {
            this.StopAsync().AsTask().WaitAndUnwrapException();
        }
    }

    /// <summary>
    /// Stops the walk, but only if <paramref name="ownCts"/> still belongs to the walk which is currently
    /// running. A walk loop must never stop a walk other than its own: by the time it notices it has nothing
    /// left to do, a newer walk may already be in place, and the unconditional <see cref="StopAsync"/> would
    /// cancel that one - killing a walk a single step after it began. The loop cannot rule this out by
    /// checking its own token either, because it may be parked on the writer lock, which is taken without a
    /// token and therefore is not released by cancelling it.
    /// </summary>
    /// <param name="ownCts">The cancellation token source of the calling walk loop.</param>
    private async ValueTask StopOwnWalkAsync(CancellationTokenSource ownCts)
    {
        using var writeLock = await this._walkLock.WriterLockAsync();

        if (!ReferenceEquals(this._walkCts, ownCts))
        {
            // Our walk is already over; whatever runs now belongs to someone else and is not ours to stop.
            return;
        }

        await this.StopCurrentWalkAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Ends the currently running walk and resets the walk state. The caller must hold the writer lock.
    /// </summary>
    private async ValueTask StopCurrentWalkAsync()
    {
        if (this._walkCts != null)
        {
            await this._walkCts.CancelAsync().ConfigureAwait(false);
            this._walkCts.Dispose();
            this._walkCts = null;
            this._nextSteps.Clear();
            this._currentWalkStepCount = 0;
            this._currentWalkToken = Guid.Empty;
            this.CurrentTarget = default;
        }
    }

    private async Task WalkLoopAsync(CancellationTokenSource ownCts, CancellationToken cancellationToken)
    {
        // Task.Delay might take longer than we specify. We need to compensate that.
        var lastOffset = TimeSpan.Zero;
        while (!cancellationToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            var step = await this.WalkStepAsync(ownCts, cancellationToken).ConfigureAwait(false);
            if (step is null)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await this.StopOwnWalkAsync(ownCts).ConfigureAwait(false);
                }

                // A null step means this walk is over for good - the walker is disposed, the supporter is
                // no longer active, the queue ran dry, or the token was cancelled. There is nothing to
                // retry, so leave the loop instead of continuing it.
                //
                // Continuing here used to be load-bearing: it relied on the StopAsync above having
                // cancelled our own token, so that the while-condition ended the loop on the next pass.
                // That assumption breaks whenever _walkCts no longer refers to this loop's source, because
                // StopAsync is then a no-op which leaves our token uncancelled - and since the null step
                // is reproducible, the loop spins at full speed without ever awaiting anything.
                break;
            }

            var delay = this.StepDelay(step);
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
    private async ValueTask<WalkingStep?> WalkStepAsync(CancellationTokenSource ownCts, CancellationToken cancellationToken)
    {
        try
        {
            if (this._isDisposed)
            {
                Debug.WriteLine("walker already disposed");
                return null;
            }

            bool stop;
            using (await this._walkLock.ReaderLockAsync(cancellationToken))
            {
                stop = !cancellationToken.IsCancellationRequested && this.ShouldWalkerStop();
            }

            if (stop)
            {
                await this.StopOwnWalkAsync(ownCts).ConfigureAwait(false);
                return null;
            }

            // Update new coords
            using (await this._walkLock.WriterLockAsync(cancellationToken))
            {
                return this.WalkNextStepIfStepAvailable();
            }
        }
        catch (OperationCanceledException)
        {
            // we can ignore those
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }

        return null;
    }

    private WalkingStep? WalkNextStepIfStepAvailable()
    {
        if (this.ShouldWalkerStop())
        {
            return null;
        }

        var nextStep = this._nextSteps.Dequeue();
        this._walkSupporter.Position = nextStep.To;

        if (this._walkSupporter is IRotatable rotatable)
        {
            rotatable.Rotation = nextStep.Direction;
        }

        return nextStep;
    }

    private bool ShouldWalkerStop() => !((this._walkSupporter as IAttackable)?.IsActive() ?? false) || this._nextSteps.Count <= 0;
}
