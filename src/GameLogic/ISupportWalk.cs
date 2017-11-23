// <copyright file="ISupportWalk.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Interface for objects which support walking.
    /// </summary>
    public interface ISupportWalk : ILocateable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is walking.
        /// </summary>
        bool IsWalking { get; }

        /// <summary>
        /// Gets the delay between each step. Lower delay means faster walking.
        /// </summary>
        TimeSpan StepDelay { get; }

        /// <summary>
        /// Gets the next walking steps.
        /// </summary>
        Stack<WalkingStep> NextDirections { get; }

        /// <summary>
        /// Gets or sets the walk target coordinate.
        /// </summary>
        Point WalkTarget { get; set; }
    }

    /// <summary>
    /// A walking step information.
    /// </summary>
    public struct WalkingStep
    {
        /// <summary>
        /// Gets or sets point from where the step originates.
        /// </summary>
        public Point From { get; set; }

        /// <summary>
        /// Gets or sets the point where the step targets.
        /// </summary>
        public Point To { get; set; }

        /// <summary>
        /// Gets or sets the direction (1 - 8).
        /// </summary>
        public Direction Direction { get; set; }
    }
}