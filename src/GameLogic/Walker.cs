// <copyright file="Walker.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Threading;
    using log4net;

    /// <summary>
    /// Class which manages walking for instances of <see cref="ISupportWalk"/>.
    /// </summary>
    public sealed class Walker : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Walker));

        private readonly ISupportWalk walkSupporter;
        private Timer walkTimer;
        private ReaderWriterLockSlim walkLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="Walker"/> class.
        /// </summary>
        /// <param name="walkSupporter">The walk supporter.</param>
        public Walker(ISupportWalk walkSupporter)
        {
            this.walkSupporter = walkSupporter;
            this.walkLock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Starts to walk.
        /// </summary>
        public void Start()
        {
            this.walkTimer = new Timer(this.WalkStep, null, TimeSpan.Zero, this.walkSupporter.StepDelay);
        }

        /// <summary>
        /// Stops the walk.
        /// </summary>
        public void Stop()
        {
            this.walkLock.EnterWriteLock();
            try
            {
                this.walkSupporter.NextDirections.Clear();
                if (this.walkTimer != null)
                {
                    this.walkTimer.Dispose(); // reuse timer?
                    this.walkTimer = null;
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

                var attackable = this.walkSupporter as IAttackable;
                bool stop;
                this.walkLock.EnterReadLock();
                try
                {
                    stop = !(attackable?.Alive ?? false) || this.walkSupporter.NextDirections.Count <= 0;
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
                    var nextStep = this.walkSupporter.NextDirections.Pop();
                    this.walkSupporter.X = nextStep.To.X;
                    this.walkSupporter.Y = nextStep.To.Y;

                    if (this.walkSupporter is IRotateable rotateable)
                    {
                        rotateable.Rotation = nextStep.Direction;
                    }
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
    }
}