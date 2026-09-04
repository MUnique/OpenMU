// <copyright file="CastleSiegeMachine.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.NPC;

using MUnique.OpenMU.DataModel.Configuration;
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
    /// <param name="machineType">The machine type.</param>
    public CastleSiegeMachine(
        MonsterSpawnArea spawnInfo,
        MonsterDefinition stats,
        GameMap map,
        CastleSiegeNpcRuntime runtime,
        CastleSiegeMachineIntelligence intelligence,
        CastleSiegeMachineType machineType)
        : base(spawnInfo, stats, map, runtime, intelligence)
    {
        this.MachineType = machineType;
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
    /// Gets the maximum distance from which a player can operate the machine.
    /// </summary>
    public static int OperationRange { get; } = 3;

    /// <summary>
    /// Gets the machine type.
    /// </summary>
    public CastleSiegeMachineType MachineType { get; }

    /// <summary>
    /// Gets or sets the current operator.
    /// </summary>
    public Player? Operator { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this machine is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Determines whether the machine can be operated by the specified side.
    /// </summary>
    /// <param name="side">The player's Castle Siege side.</param>
    /// <returns><see langword="true"/> when the side matches this machine.</returns>
    public bool CanBeUsedBy(CastleSiegeJoinSide side) => this.IsSameSide(side);

    /// <summary>
    /// Determines whether the specified side matches this machine's side.
    /// </summary>
    /// <param name="side">The Castle Siege side to check.</param>
    /// <returns><see langword="true"/> when the side matches this machine.</returns>
    public bool IsSameSide(CastleSiegeJoinSide side)
    {
        return this.MachineType switch
        {
            CastleSiegeMachineType.Attack => side is
                CastleSiegeJoinSide.Attack1
                or CastleSiegeJoinSide.Attack2
                or CastleSiegeJoinSide.Attack3,
            CastleSiegeMachineType.Defense => side == CastleSiegeJoinSide.Defense,
            _ => false,
        };
    }

    /// <inheritdoc />
    protected override void Dispose(bool managed)
    {
        if (managed)
        {
            this.Operator = null;
            this.IsActive = false;
        }

        base.Dispose(managed);
    }
}
