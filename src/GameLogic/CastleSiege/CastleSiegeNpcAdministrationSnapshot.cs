// <copyright file="CastleSiegeNpcAdministrationSnapshot.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// An immutable snapshot of one configured Castle Siege NPC.
/// </summary>
/// <param name="MonsterNumber">The monster definition number.</param>
/// <param name="InstanceId">The configured NPC instance identifier.</param>
/// <param name="DefenseLevel">The defense upgrade level.</param>
/// <param name="RegenerationLevel">The health-regeneration upgrade level.</param>
/// <param name="LifeLevel">The maximum-health upgrade level.</param>
/// <param name="CurrentHealth">The current health.</param>
/// <param name="MaximumHealth">The maximum health.</param>
/// <param name="IsAlive">Whether the NPC is currently alive.</param>
/// <param name="IsPersisted">Whether the NPC state is persisted between siege cycles.</param>
public sealed record CastleSiegeNpcAdministrationSnapshot(
    short MonsterNumber,
    byte InstanceId,
    byte DefenseLevel,
    byte RegenerationLevel,
    byte LifeLevel,
    int CurrentHealth,
    int MaximumHealth,
    bool IsAlive,
    bool IsPersisted);
