﻿// <copyright file="IPathFinder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

using System.Threading;

/// <summary>
/// Interface for a path finder.
/// </summary>
internal interface IPathFinder
{
    /// <summary>
    /// Finds the path between two points.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="end">The end point.</param>
    /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
    /// <returns>The path between start and end, including <paramref name="end"/>, but excluding <paramref name="start"/>.</returns>
    IList<PathResultNode>? FindPath(Point start, Point end, CancellationToken cancellationToken = default);
}