// <copyright file="IPlayerTeleportRestrictionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which can prevent a player from being teleported by a skill to a target on the same map,
/// e.g. over a closed Castle Siege gate.
/// </summary>
[Guid("54B6C467-417E-48A4-A3F2-C2345E8BCA29")]
[PlugInPoint("Player teleport restriction", "Plugins which can prevent a player from being teleported by a skill to a target on the same map.")]
public interface IPlayerTeleportRestrictionPlugIn
{
    /// <summary>
    /// Checks if the player may be teleported from its current position to the target.
    /// </summary>
    /// <param name="player">The player who would be teleported.</param>
    /// <param name="target">The target coordinates on the current map of the player.</param>
    /// <param name="eventArgs">The event args. A plugin sets <see cref="CancelEventArgs.Cancel"/> to prevent the teleport.</param>
    void CheckTeleport(Player player, Point target, CancelEventArgs eventArgs);
}
