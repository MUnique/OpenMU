// <copyright file="CastleSiegeCrown.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

/// <summary>
/// The interactive Castle Siege Crown.
/// </summary>
public sealed class CastleSiegeCrown : CastleSiegeNpcBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeCrown"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the Crown is spawned.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The Crown intelligence.</param>
    public CastleSiegeCrown(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeCrownIntelligence intelligence)
        : base(spawnInfo, stats, map, runtime, intelligence)
    {
    }

    /// <summary>
    /// Gets the monster number of the Crown.
    /// </summary>
    public static short MonsterNumber { get; } = 216;

    /// <summary>
    /// Gets or sets the Crown state.
    /// </summary>
    public CastleSiegeCrownState State { get; set; } = CastleSiegeCrownState.Locked;
}
