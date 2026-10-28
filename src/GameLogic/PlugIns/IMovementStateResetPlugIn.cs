// <copyright file="IMovementStateResetPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin point which is called when the movement state of a player is reset, e.g. after a teleport.
/// </summary>
[Guid("B8ED165D-4A63-4D24-864B-738673D82A95")]
[PlugInPoint("Movement state reset", "Plugins which need to reset their movement-related state, e.g. after a teleport.")]
public interface IMovementStateResetPlugIn
{
    /// <summary>
    /// Resets the movement state for the player.
    /// </summary>
    /// <param name="player">The player.</param>
    ValueTask ResetMovementStateAsync(Player player);
}
