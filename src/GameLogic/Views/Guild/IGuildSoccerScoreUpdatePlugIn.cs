// <copyright file="IGuildSoccerScoreUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// Interface of a view whose implementation shows the score board of a battle soccer match
/// to everyone on the soccer map, players of the match and spectators alike.
/// </summary>
public interface IGuildSoccerScoreUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows or updates the score board.
    /// </summary>
    /// <param name="redTeamName">The name of the red team (the first guild).</param>
    /// <param name="redTeamGoals">The goals of the red team.</param>
    /// <param name="blueTeamName">The name of the blue team (the second guild).</param>
    /// <param name="blueTeamGoals">The goals of the blue team.</param>
    ValueTask UpdateSoccerScoreAsync(string redTeamName, byte redTeamGoals, string blueTeamName, byte blueTeamGoals);

    /// <summary>
    /// Hides the score board, because the match is over.
    /// </summary>
    ValueTask HideSoccerScoreAsync();
}
