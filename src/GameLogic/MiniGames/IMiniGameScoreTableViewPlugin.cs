// <copyright file="IMiniGameScoreTableViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames
{
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Interface of a view whose implementation informs about the score table of a mini game.
    /// </summary>
    public interface IMiniGameScoreTableViewPlugin : IViewPlugIn
    {
        /// <summary>
        /// Shows the score table to the player.
        /// </summary>
        /// <param name="playerRank">The rank of this player.</param>
        /// <param name="scores">The scores of the players of the same mini game instance.</param>
        void ShowScoreTable(byte playerRank, IReadOnlyCollection<(string Player, int Score, int BonusExp, int BonusMoney)> scores);
    }
}