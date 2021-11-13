// <copyright file="PointCombination.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.Runtime.InteropServices;

/// <summary>
/// A combination of the start and end point, which acts like a key for the next step to reach the end point.
/// </summary>
/// <param name="Start">The start point.</param>
/// <param name="End">The end point.</param>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public record struct PointCombination(Point Start, Point End);
