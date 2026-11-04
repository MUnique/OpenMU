// <copyright file="IAssignPlayersToGensPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// The view plugin to visibly assign players to their gens.
/// </summary>
public interface IAssignPlayersToGensPlugIn : IViewPlugIn
{
    /// <summary>
    /// Assigns the players to their gens, so that the gens mark and rank is shown next to their names.
    /// Players which are not member of a gens get their gens mark removed.
    /// </summary>
    /// <param name="players">The players.</param>
    ValueTask AssignPlayersToGensAsync(ICollection<Player> players);
}
