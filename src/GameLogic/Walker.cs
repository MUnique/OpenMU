// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Class which manages walking for instances of <see cref="ISupportWalk"/>.
    /// </summary>
    public sealed class Walker : IDisposable
    {
        private readonly ISupportWalk walkSupporter;
        private readonly Func<TimeSpan> stepDelay;
        private readonly Queue<WalkingStep> nextSteps = new (5);
        private readonly ReaderWriterLockSlim walkLock;
        private CancellationTokenSource? walkCts;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Walker" /> class.
        /// </summary>
        /// <param name="walkSupporter">The walk supporter.</param>
        /// <param name="stepDelay">The delay between performing a step.</param>
        public Walker(ISupportWalk walkSupporter, Func<TimeSpan> stepDelay)
        {
            this.walkSupporter = walkSupporter;
            this.stepDelay = stepDelay;
            this.walkLock = new ReaderWriterLockSlim();
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
            if (this.isDisposed)
            {
                return;
            }

            this.walkLock.EnterWriteLock();
            try
            {
                this.CurrentTarget = target;
                this.nextSteps.Clear();
                foreach (var step in steps)
                {
                    this.nextSteps.Enqueue(step);
                }

                var cts = new CancellationTokenSource();
                this.walkCts = cts;
                Task.Run(async () => await this.WalkLoop(cts.Token), cts.Token);
            }
            finally
            {
                this.walkLock.ExitWriteLock();
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
            this.walkLock.EnterReadLock();
            try
            {
                foreach (var direction in this.nextSteps.Reverse().Select(step => step.Direction))
                {
                    directions[count] = direction;
                    count++;
                }
            }
            finally
            {
                this.walkLock.ExitReadLock();
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
            this.walkLock.EnterReadLock();
            try
            {
                foreach (var direction in this.nextSteps.Reverse())
                {
                    steps[count] = direction;
                    count++;
                }
            }
            finally
            {
                this.walkLock.ExitReadLock();
            }

            return count;
        }

        /// <summary>
        /// Stops the walk.
        /// </summary>
        public void Stop()
        {
            this.walkLock.EnterWriteLock();
            try
            {
                if (this.walkCts != null)
                {
                    this.walkCts.Cancel(false);
                    this.walkCts.Dispose();
                    this.walkCts = null;
                    this.nextSteps.Clear();
                    this.CurrentTarget = default;
                }
            }
            finally
            {
                this.walkLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.isDisposed = true;
            if (this.walkCts != null)
            {
                this.Stop();
            }

            this.walkLock.Dispose();
        }

        private async Task WalkLoop(CancellationToken cancellationToken)
        {
            var delay = this.stepDelay().Subtract(TimeSpan.FromMilliseconds(50));

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
                if (this.isDisposed)
                {
                    Debug.WriteLine("walker already disposed");
                    return;
                }

                bool stop;
                this.walkLock.EnterReadLock();
                try
                {
                    stop = !cancellationToken.IsCancellationRequested && this.ShouldWalkerStop();
                }
                finally
                {
                    this.walkLock.ExitReadLock();
                }

                if (stop)
                {
                    this.Stop();
                }

                // Update new coords
                this.walkLock.EnterWriteLock();
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
                    this.walkLock.ExitWriteLock();
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
                var nextStep = this.nextSteps.Dequeue();
                this.walkSupporter.Position = nextStep.To;

                if (this.walkSupporter is IRotatable rotatable)
                {
                    rotatable.Rotation = nextStep.Direction;
                }
            }
        }

        private bool ShouldWalkerStop() => !((this.walkSupporter as IAttackable)?.IsActive() ?? false) || this.nextSteps.Count <= 0;
    }
}