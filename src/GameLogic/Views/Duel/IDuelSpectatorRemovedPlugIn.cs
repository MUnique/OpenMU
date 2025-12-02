// <copyright file="IDuelSpectatorRemovedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Duel;

/// <summary>
/// Interface of a view whose implementation informs about a removed spectator.
/// </summary>
public interface IDuelSpectatorRemovedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Removes the spectator.
    /// </summary>
    /// <param name="spectator">The spectator.</param>
    ValueTask SpectatorRemovedAsync(Player spectator);
}