// <copyright file="ImperialGuardianExperienceReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.ImperialGuardian;

/// <summary>
/// The experience reward of the imperial guardian event for players up to a level.
/// </summary>
public class ImperialGuardianExperienceReward
{
    /// <summary>
    /// Gets or sets the maximum level of the player, including the master level.
    /// </summary>
    public int MaximumPlayerLevel { get; set; }

    /// <summary>
    /// Gets or sets the experience.
    /// </summary>
    public int Experience { get; set; }
}
