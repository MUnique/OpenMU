// <copyright file="CastleSiegeLever.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

/// <summary>
/// An interactive Castle Siege gate lever.
/// </summary>
public sealed class CastleSiegeLever : CastleSiegeNpcBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeLever"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the lever is spawned.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The lever intelligence.</param>
    public CastleSiegeLever(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeContext context,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeLeverIntelligence intelligence)
        : base(spawnInfo, stats, map, runtime, intelligence)
    {
        this.Context = context;
    }

    /// <summary>
    /// Gets the monster number of gate levers.
    /// </summary>
    public static short MonsterNumber { get; } = 219;

    /// <summary>
    /// Gets or sets the gate controlled by this lever.
    /// </summary>
    public CastleSiegeGate? Gate { get; set; }

    /// <summary>
    /// Gets the owning Castle Siege context.
    /// </summary>
    internal CastleSiegeContext Context { get; }
}
