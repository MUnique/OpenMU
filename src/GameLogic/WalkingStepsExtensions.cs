// <copyright file="WalkingStepsExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions for <see cref="WalkingStep"/>s.
/// </summary>
public static class WalkingStepsExtensions
{
    /// <summary>
    /// Gets the starting point of the specified walking steps.
    /// </summary>
    /// <param name="steps">The walking steps.</param>
    /// <returns>The starting point of the specified walking steps.</returns>
    public static Point GetStart(this Memory<WalkingStep> steps)
    {
        return steps.Length == 0 ? default : steps.Span[0].From;
    }

    /// <summary>
    /// Gets the target point of the specified walking steps.
    /// </summary>
    /// <param name="steps">The walking steps.</param>
    /// <returns>The target point of the specified walking steps.</returns>
    public static Point GetTarget(this Memory<WalkingStep> steps)
    {
        return steps.Length == 0 ? default : steps.Span[^1].To;
    }
}