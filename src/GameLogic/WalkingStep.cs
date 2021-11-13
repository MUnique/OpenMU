// <copyright file="WalkingStep.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A walking step information.
/// </summary>
/// <param name="From">The point from where the step originates.</param>
/// <param name="To">The point where the step targets.</param>
/// <param name="Direction">The direction.</param>
public record struct WalkingStep(Point From, Point To, Direction Direction);