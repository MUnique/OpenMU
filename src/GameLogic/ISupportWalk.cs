﻿// <copyright file="ISupportWalk.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

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
    /// Gets the walk target coordinate.
    /// </summary>
    Point WalkTarget { get; }

    /// <summary>
    /// Gets the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="steps">The steps.</param>
    /// <returns>The number of written steps.</returns>
    int GetSteps(Span<WalkingStep> steps);

    /// <summary>
    /// Gets the directions of the steps which are about to happen next by writing them into the given span.
    /// </summary>
    /// <param name="directions">The directions.</param>
    /// <returns>The number of written directions.</returns>
    int GetDirections(Span<Direction> directions);
}