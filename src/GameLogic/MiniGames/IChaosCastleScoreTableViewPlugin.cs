// <copyright file="IChaosCastleScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the result of a chaos castle event.
/// </summary>
public interface IChaosCastleScoreTableViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Shows the result of the chaos castle event to the player.
    /// </summary>
    /// <param name="success">If the player won the event.</param>
    /// <param name="playerName">The player name.</param>
    /// <param name="monsterKillCount">The number of monsters which the player killed.</param>
    /// <param name="playerKillCount">The number of players which the player killed.</param>
    /// <param name="bonusExp">The bonus experience.</param>
    ValueTask ShowScoreTableAsync(bool success, string playerName, int monsterKillCount, int playerKillCount, int bonusExp);
}
