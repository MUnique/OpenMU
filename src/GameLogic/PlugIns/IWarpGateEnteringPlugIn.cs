// <copyright file="IWarpGateEnteringPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a player is about to enter a warp gate.
/// </summary>
[Guid("3F6C2B1E-9A47-4D8B-B5E2-7C0D1A9E4F36")]
[PlugInPoint("Warp gate entering", "Plugins which are called when a player is about to enter a warp gate. They can deny the entrance, e.g. while an event map is closed.")]
public interface IWarpGateEnteringPlugIn
{
    /// <summary>
    /// Is called when a player is about to enter a warp gate.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetGate">The target gate.</param>
    /// <param name="eventArgs">The <see cref="CancelEventArgs"/> instance containing the event data. The entrance can be denied by setting <see cref="CancelEventArgs.Cancel"/> to <c>true</c>.
    /// The plugin should then show the reason to the player. The player stays at its current position.</param>
    ValueTask WarpGateEnteringAsync(Player player, ExitGate targetGate, CancelEventArgs eventArgs);
}
