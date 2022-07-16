// <copyright file="INewPlayersInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about players which went into the observing scope.
/// </summary>
public interface INewPlayersInScopePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the new players in scope.
    /// </summary>
    /// <param name="newObjects">The new objects.</param>
    /// <param name="isSpawned">If set to <c>true</c>, the players spawned on the map.</param>
    ValueTask NewPlayersInScopeAsync(IEnumerable<Player> newObjects, bool isSpawned = true);
}