// <copyright file="ISpeedHackCheatCheckPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point interface for speed hack checking.
/// </summary>
[Guid("F01BEF4A-32EE-4D56-BEB0-BE503B748DC7")]
[PlugInPoint("Speedhack Cheat Check", "Is called for walk/attack speedhack checks and movement state resets.")]
public interface ISpeedHackCheatCheckPlugIn
{
    /// <summary>
    /// Checks if the walk speed is within limits.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="steps">The steps requested by the player.</param>
    /// <param name="eventArgs">The event args to mark a violation.</param>
    ValueTask WalkCheatCheckAsync(Player player, Memory<WalkingStep> steps, SpeedHackCheckEventArgs eventArgs);

    /// <summary>
    /// Checks if the attack speed is within limits.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="eventArgs">The event args to mark a violation.</param>
    ValueTask AttackCheatCheckAsync(Player player, SpeedHackCheckEventArgs eventArgs);

    /// <summary>
    /// Resets the movement speed check state for the player.
    /// </summary>
    /// <param name="player">The player.</param>
    ValueTask ResetMovementStateAsync(Player player);
}
