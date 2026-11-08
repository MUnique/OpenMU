// <copyright file="ResetParameter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// A value object that contains the parameters for a character reset.
/// </summary>
/// <param name="NextResetCount">The resulting reset count after a successful reset.</param>
/// <param name="LevelAfterReset">The level of the character after the reset.</param>
/// <param name="ReplacePointsPerReset">...</param>
/// <param name="PointsForReset">The number of points granted for the reset.</param>
/// <param name="TotalPointsAfterReset">The total number of points after the reset when replacement mode is active.</param>
/// <param name="MoveHome">Indicates whether a reset moves the player home.</param>
/// <param name="LogOut">Indicates whether a reset logs the player out back to character selection.</param>
/// <param name="ResetStats">Indicates whether a reset sets the stat points back to the initial values.</param>
internal readonly record struct ResetParameter(
    int NextResetCount,
    int LevelAfterReset,
    bool ReplacePointsPerReset,
    int TotalPointsAfterReset,
    int PointsForReset,
    bool MoveHome,
    bool LogOut,
    bool ResetStats);