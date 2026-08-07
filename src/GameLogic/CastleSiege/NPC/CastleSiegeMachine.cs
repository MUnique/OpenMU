// <copyright file="CastleSiegeMachine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.GameLogic.CastleSiege.Intelligence;

/// <summary>
/// An interactive Castle Siege attack or defense machine.
/// </summary>
public sealed class CastleSiegeMachine : CastleSiegeNpcBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachine"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The NPC definition.</param>
    /// <param name="map">The map on which the machine is spawned.</param>
    /// <param name="runtime">The Castle Siege runtime entry.</param>
    /// <param name="intelligence">The machine intelligence.</param>
    public CastleSiegeMachine(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeMachineIntelligence intelligence)
        : base(spawnInfo, stats, map, runtime, intelligence)
    {
    }

    /// <summary>
    /// Gets the attacking machine monster number.
    /// </summary>
    public static short AttackMonsterNumber { get; } = 221;

    /// <summary>
    /// Gets the defending machine monster number.
    /// </summary>
    public static short DefenseMonsterNumber { get; } = 222;

    /// <summary>
    /// Gets or sets the current operator.
    /// </summary>
    public Player? Operator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this machine is active.
    /// </summary>
    public bool IsActive { get; set; }
}
