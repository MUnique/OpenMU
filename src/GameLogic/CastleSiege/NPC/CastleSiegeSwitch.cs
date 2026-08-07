// <copyright file="CastleSiegeSwitch.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

/// <summary>
/// One of the two Castle Siege Crown switches.
/// </summary>
public sealed class CastleSiegeSwitch : CastleSiegeNpcBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeSwitch"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the switch is spawned.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The switch intelligence.</param>
    /// <param name="switchIndex">The zero-based switch index.</param>
    public CastleSiegeSwitch(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeSwitchIntelligence intelligence,
        int switchIndex)
        : base(spawnInfo, stats, map, runtime, intelligence)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(switchIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(switchIndex, 1);
        this.SwitchIndex = switchIndex;
    }

    /// <summary>
    /// Gets the first Crown switch monster number.
    /// </summary>
    public static short FirstMonsterNumber { get; } = 217;

    /// <summary>
    /// Gets the second Crown switch monster number.
    /// </summary>
    public static short SecondMonsterNumber { get; } = 218;

    /// <summary>
    /// Gets the zero-based switch index.
    /// </summary>
    public int SwitchIndex { get; }

    /// <summary>
    /// Gets or sets the player currently occupying the switch.
    /// </summary>
    public Player? Occupant { get; set; }
}
