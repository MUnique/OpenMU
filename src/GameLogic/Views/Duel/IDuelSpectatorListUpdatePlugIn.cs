// <copyright file="IDuelSpectatorListUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about the updated spectator list.
/// </summary>
public interface IDuelSpectatorListUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the spectator list.
    /// </summary>
    /// <param name="spectators">The spectators.</param>
    ValueTask UpdateSpectatorListAsync(IList<Player> spectators);
}