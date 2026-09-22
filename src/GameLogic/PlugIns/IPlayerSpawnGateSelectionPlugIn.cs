// <copyright file="IPlayerSpawnGateSelectionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when the gate is determined at which a player is spawned,
/// e.g. after a death or when it's moved to the safezone.
/// </summary>
/// <remarks>
/// It allows game features which keep the player at a special place - like duels, the guild war
/// soccer match or mini games - to define their own spawn gate, instead of the player being sent
/// to the safezone of its current map.
/// </remarks>
[Guid("9E5A0B4C-6D71-4E28-9F2A-1D3B7C8E0A56")]
[PlugInPoint("Player spawn gate selection", "Plugins which determine the gate at which a player is spawned.")]
public interface IPlayerSpawnGateSelectionPlugIn
{
    /// <summary>
    /// Is called when the spawn gate for the player is determined.
    /// </summary>
    /// <param name="player">The player which is spawned.</param>
    /// <param name="args">The arguments, which take the selected gate.</param>
    ValueTask SelectSpawnGateAsync(Player player, SpawnGateSelectionArgs args);
}
