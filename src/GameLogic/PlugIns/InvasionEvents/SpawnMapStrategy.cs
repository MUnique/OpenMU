// <copyright file="SpawnMapStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

/// <summary>
/// Defines how the spawn map is selected when multiple map IDs are configured.
/// </summary>
public enum SpawnMapStrategy
{
    /// <summary>
    /// A single map is picked at random from the configured list.
    /// </summary>
    RandomMap,

    /// <summary>
    /// The monster spawns on every map in the configured list.
    /// </summary>
    AllMaps,
}