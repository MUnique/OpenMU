// <copyright file="ResetProgression.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// A value object that contains costs and rewards for the next reset.
/// </summary>
/// <param name="NextResetCount">The resulting reset count after a successful reset.</param>
/// <param name="RequiredZen">The required zen for the reset.</param>
/// <param name="RequiredItemAmount">The required number of configured reset items.</param>
/// <param name="PointsForReset">The number of points granted for the reset.</param>
/// <param name="TotalPointsAfterReset">The total number of points after the reset when replacement mode is active.</param>
public readonly record struct ResetProgression(
    int NextResetCount,
    int RequiredZen,
    int RequiredItemAmount,
    int PointsForReset,
    int TotalPointsAfterReset);
