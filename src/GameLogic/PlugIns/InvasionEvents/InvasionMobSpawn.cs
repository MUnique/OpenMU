// <copyright file="InvasionMobSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

/// <summary>
/// Represents a mob spawn configuration for invasions.
/// </summary>
/// <param name="MonsterId">The monster ID to spawn.</param>
/// <param name="Count">The number of monsters to spawn.</param>
/// <param name="MapId">The map ID. If null, spawns on the selected map.</param>
/// <param name="X1">The minimum X coordinate. If null, uses default (10).</param>
/// <param name="X2">The maximum X coordinate. If null, uses default (240).</param>
/// <param name="Y1">The minimum Y coordinate. If null, uses default (10).</param>
/// <param name="Y2">The maximum Y coordinate. If null, uses default (240).</param>
public record InvasionMobSpawn(
    ushort MonsterId,
    ushort Count,
    ushort? MapId = null,
    byte? X1 = null,
    byte? Y1 = null,
    byte? X2 = null,
    byte? Y2 = null);

