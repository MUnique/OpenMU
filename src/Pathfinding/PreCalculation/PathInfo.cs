// <copyright file="PathInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.Runtime.InteropServices;

/// <summary>
/// Information about which is the <see cref="NextStep"/> to reach the <see cref="PointCombination.End"/> from the <see cref="PointCombination.Start"/>.
/// </summary>
/// <param name="Combination">The start/end point combination which acts like a key for the next step.</param>
/// <param name="NextStep">The next step to get one step closer to the <see cref="PointCombination.End"/>.</param>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
public record struct PathInfo(PointCombination Combination, Point NextStep);
