// <copyright file="ResetConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Configuration of the Reset System.
/// </summary>
public class ResetConfiguration
{
    /// <summary>
    /// Gets or sets the reset limit, which is the maximum amount of possible resets.
    /// </summary>
    public int? ResetLimit { get; set; }

    /// <summary>
    /// Gets or sets the required level for a reset.
    /// </summary>
    public int RequiredLevel { get; set; } = 400;

    /// <summary>
    /// Gets or sets the character level after a reset.
    /// </summary>
    public int LevelAfterReset { get; set; } = 10;

    /// <summary>
    /// Gets or sets the required money for a reset.
    /// </summary>
    public int RequiredMoney { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the required money should
    /// be multiplied with the current reset count.
    /// </summary>
    public bool MultiplyRequiredMoneyByResetCount { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a reset sets the stat points back to the initial values.
    /// </summary>
    public bool ResetStats { get; set; } = true;

    /// <summary>
    /// Gets or sets the amount of points which will be set at the <see cref="Character.LevelUpPoints"/> when doing a reset.
    /// </summary>
    public int PointsPerReset { get; set; } = 1500;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="PointsPerReset"/> should be multiplied with the current reset count.
    /// </summary>
    public bool MultiplyPointsByResetCount { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a reset will replace (true) or add (false) the <see cref="Character.LevelUpPoints"/>.
    /// </summary>
    public bool ReplacePointsPerReset { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a reset moves the player home.
    /// </summary>
    public bool MoveHome { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether a reset logs the player out back to character selection.
    /// </summary>
    public bool LogOut { get; set; } = true;
}