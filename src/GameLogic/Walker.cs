// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics;
using System.Threading;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Class which manages walking for instances of <see cref="ISupportWalk"/>.
/// </summary>
public sealed class Walker : IDisposable
{
    private readonly ISupportWalk _walkSupporter;
    private readonly Func<TimeSpan> _stepDelay;
    private readonly Queue<WalkingStep> _nextSteps = new (5);

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

    /// <summary>
    /// Initializes a new instance of the <see cref="Walker" /> class.
    /// </summary>
    /// <param name="walkSupporter">The walk supporter.</param>
    /// <param name="stepDelay">The delay between performing a step.</param>
    public Walker(ISupportWalk walkSupporter, Func<TimeSpan> stepDelay)
    {
        this._walkSupporter = walkSupporter;
        this._stepDelay = stepDelay;
        this._walkLock = new AsyncReaderWriterLock();
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
    public async ValueTask WalkToAsync(Point target, Memory<WalkingStep> steps)
    {
        if (this._isDisposed)
        {
            return;
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

        this.CurrentTarget = target;
        EnqueueSteps();

        var cts = new CancellationTokenSource();
        this._walkCts = cts;
        _ = Task.Run(async () => await this.WalkLoopAsync(cts.Token).ConfigureAwait(false), cts.Token);
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
        foreach (var direction in this._currentWalkSteps[.._currentWalkStepCount])
        {
            steps.Span[count] = direction;
            count++;
        }

        return count;
    }

    /// <summary>
    /// Stops the walk.
    /// </summary>
    public async ValueTask StopAsync()
    {
        using var writeLock = await this._walkLock.WriterLockAsync();

        if (this._walkCts != null)
        {
            await this._walkCts.CancelAsync().ConfigureAwait(false);
            this._walkCts.Dispose();
            this._walkCts = null;
            this._nextSteps.Clear();
            this._currentWalkStepCount = 0;
            this.CurrentTarget = default;
        }
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

    private async Task WalkLoopAsync(CancellationToken cancellationToken)
    {
        var delay = this._stepDelay().Subtract(TimeSpan.FromMilliseconds(50));

        // Task.Delay might take longer than we specify. We need to compensate that.
        var lastOffset = TimeSpan.Zero;
        while (!cancellationToken.IsCancellationRequested)
        {
            var sw = Stopwatch.StartNew();
            await this.WalkStepAsync(cancellationToken).ConfigureAwait(false);

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
    private async ValueTask WalkStepAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (this._isDisposed)
            {
                Debug.WriteLine("walker already disposed");
                return;
            }

            bool stop;
            using (await this._walkLock.ReaderLockAsync(cancellationToken))
            {
                stop = !cancellationToken.IsCancellationRequested && this.ShouldWalkerStop();
            }

            if (stop)
            {
                await this.StopAsync().ConfigureAwait(false);
                return;
            }

            // Update new coords
            using (await this._walkLock.WriterLockAsync(cancellationToken))
            {
                this.WalkNextStepIfStepAvailable();
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
    }

    private void WalkNextStepIfStepAvailable()
    {
        if (this.ShouldWalkerStop())
        {
            return;
        }

        var nextStep = this._nextSteps.Dequeue();
        this._walkSupporter.Position = nextStep.To;

        if (this._walkSupporter is IRotatable rotatable)
        {
            rotatable.Rotation = nextStep.Direction;
        }
    }

    private bool ShouldWalkerStop() => !((this._walkSupporter as IAttackable)?.IsActive() ?? false) || this._nextSteps.Count <= 0;
}