// <copyright file="CastleSiegeNpcInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Information about a Castle Siege gate or Guardian Statue.
/// </summary>
/// <param name="NpcNumber">The NPC number.</param>
/// <param name="NpcIndex">The NPC instance identifier.</param>
/// <param name="DefenseLevel">The defense upgrade level.</param>
/// <param name="RegenerationLevel">The regeneration upgrade level.</param>
/// <param name="MaximumHealth">The maximum health.</param>
/// <param name="CurrentHealth">The current health.</param>
/// <param name="PositionX">The X coordinate.</param>
/// <param name="PositionY">The Y coordinate.</param>
/// <param name="IsAlive">Whether the NPC is alive.</param>
public sealed record CastleSiegeNpcInfo(
    uint NpcNumber,
    uint NpcIndex,
    byte DefenseLevel,
    byte RegenerationLevel,
    int MaximumHealth,
    int CurrentHealth,
    byte PositionX,
    byte PositionY,
    bool IsAlive);
