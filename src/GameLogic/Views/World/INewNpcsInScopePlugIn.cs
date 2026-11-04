// <copyright file="INewNpcsInScopePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Interface of a view whose implementation informs about non-player-characters which went into the observing scope.
/// </summary>
public interface INewNpcsInScopePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the new npcs in scope.
    /// </summary>
    /// <param name="newObjects">The new objects.</param>
    /// <param name="isSpawned">If set to <c>true</c>, the players spawned on the map.</param>
    ValueTask NewNpcsInScopeAsync(IEnumerable<NonPlayerCharacter> newObjects, bool isSpawned = true);
}