// <copyright file="IRespawnAfterDeathPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about a respawn of the current player.
/// </summary>
/// <seealso cref="IMapChangePlugIn" />
public interface IRespawnAfterDeathPlugIn : IViewPlugIn
{
    /// <summary>
    /// Respawns the player. The new map and coordinates are defined in the player.SelectedCharacter.CurrentMap.
    /// </summary>
    ValueTask RespawnAsync();
}