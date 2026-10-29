// <copyright file="IPingCheckPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point interface for observing client pings.
/// </summary>
[Guid("B3A4C5D6-E7F8-4A9B-8C0D-1E2F3A4B5C6D")]
[PlugInPoint("Ping check", "Is called to observe client ping consistency.")]
public interface IPingCheckPlugIn
{
    /// <summary>
    /// Observes a client ping for clock and attack speed consistency.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="tickCount">The client tick count reported by the ping.</param>
    /// <param name="attackSpeed">The attack speed reported by the ping.</param>
    ValueTask PingCheatCheckAsync(Player player, uint tickCount, ushort attackSpeed);
}
