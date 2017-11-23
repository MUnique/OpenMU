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

        /// <summary>
        /// Initializes a new instance of the <see cref="Walker"/> class.
        /// </summary>
        /// <param name="walkSupporter">The walk supporter.</param>
        public Walker(ISupportWalk walkSupporter)
        {
            this.walkSupporter = walkSupporter;
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
            this.walkSupporter.NextDirections.Clear();
            if (this.walkTimer != null)
            {
                this.walkTimer.Dispose(); // reuse timer?
                this.walkTimer = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        /// <summary>
        /// Performs the next step of a walk.
        /// </summary>
        /// <param name="state">The state.</param>
        private void WalkStep(object state)
        {
            try
            {
                var attackable = this.walkSupporter as IAttackable;
                if (!(attackable?.Alive ?? false) || this.walkSupporter.NextDirections.Count <= 0)
                {
                    this.Stop();
                    return;
                }

                // Update new coords
                var nextStep = this.walkSupporter.NextDirections.Pop();
                this.walkSupporter.X = nextStep.To.X;
                this.walkSupporter.Y = nextStep.To.Y;

                if (this.walkSupporter is IRotateable rotateable)
                {
                    rotateable.Rotation = nextStep.Direction;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
            }
        }
    }
}