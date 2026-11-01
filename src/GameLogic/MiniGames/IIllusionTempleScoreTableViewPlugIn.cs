// <copyright file="IIllusionTempleScoreTableViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the result of an illusion temple event.
/// </summary>
public interface IIllusionTempleScoreTableViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of the finished event to the player.
    /// </summary>
    /// <param name="alliedForcesPoints">The points which the allied forces scored.</param>
    /// <param name="illusionForcesPoints">The points which the illusion forces scored.</param>
    /// <param name="results">The result of each participant.</param>
    ValueTask ShowScoreTableAsync(byte alliedForcesPoints, byte illusionForcesPoints, IReadOnlyCollection<(string Name, byte MapNumber, IllusionTempleTeam Team, byte CharacterClass, int AddedExperience)> results);
}
