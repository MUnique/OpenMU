// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using log4net;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Class which manages walking for instances of <see cref="ISupportWalk"/>.
    /// </summary>
    public sealed class Walker : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Walker));

        private readonly ISupportWalk walkSupporter;
        private readonly Func<TimeSpan> stepDelay;
        private readonly Stack<WalkingStep> nextSteps = new Stack<WalkingStep>(5);
        private Timer walkTimer;
        private ReaderWriterLockSlim walkLock;

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
            this.walkLock.EnterWriteLock();
            try
            {
                this.CurrentTarget = target;
                this.nextSteps.Clear();
                foreach (var step in steps)
                {
                    this.nextSteps.Push(step);
                }
            }
            finally
            {
                this.walkLock.ExitWriteLock();
            }

            this.walkTimer = new Timer(this.WalkStep, null, TimeSpan.Zero, this.stepDelay());
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
                if (this.walkTimer != null)
                {
                    this.walkTimer.Dispose(); // reuse timer?
                    this.walkTimer = null;
                    this.nextSteps.Clear();
                    this.CurrentTarget = default(Point);
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
            if (this.walkLock != null)
            {
                this.Stop();
                this.walkLock.Dispose();
                this.walkLock = null;
            }
        }

        /// <summary>
        /// Performs the next step of a walk.
        /// </summary>
        /// <param name="state">The state.</param>
        private void WalkStep(object state)
        {
            try
            {
                if (this.walkLock == null)
                {
                    Log.Debug("walker already disposed");
                    return;
                }

                bool stop;
                this.walkLock.EnterReadLock();
                try
                {
                    stop = this.ShouldWalkerStop();
                }
                finally
                {
                    this.walkLock.ExitReadLock();
                }

                if (stop)
                {
                    this.Stop();
                    return;
                }

                // Update new coords
                this.walkLock.EnterWriteLock();
                try
                {
                    this.WalkNextStepIfStepAvailable();
                }
                finally
                {
                    this.walkLock.ExitWriteLock();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }

        private void WalkNextStepIfStepAvailable()
        {
            if (!this.ShouldWalkerStop())
            {
                var nextStep = this.nextSteps.Pop();
                this.walkSupporter.Position = nextStep.To;

                if (this.walkSupporter is IRotatable rotateable)
                {
                    rotateable.Rotation = nextStep.Direction;
                }
            }
        }

        private bool ShouldWalkerStop() => !((this.walkSupporter as IAttackable)?.Alive ?? false) || this.nextSteps.Count <= 0;
    }
}