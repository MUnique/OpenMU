// <copyright file="IBloodCastleScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the score table of a blood castle event.
/// </summary>
public interface IBloodCastleScoreTableViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Shows the score table to the player.
    /// </summary>
    /// <param name="success">The success.</param>
    /// <param name="playerName">The player name.</param>
    /// <param name="totalScore">The total score.</param>
    /// <param name="bonusExp">The bonus experience.</param>
    /// <param name="bonusMoney">The bonus money.</param>
    ValueTask ShowScoreTableAsync(bool success, string playerName, int totalScore, int bonusExp, int bonusMoney);
}