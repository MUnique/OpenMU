// <copyright file="IDuelRequestedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called after a player challenged another player to a duel, once the
/// request was handed to the challenged player.
/// </summary>
[Guid("C7A76A9D-1B71-493F-A2C1-C434299E910D")]
[PlugInPoint("Duel requested", "Plugins which are called after a player challenged another player to a duel.")]
public interface IDuelRequestedPlugIn
{
    /// <summary>
    /// Is called after the player challenged the target to a duel.
    /// </summary>
    /// <param name="requester">The challenging player.</param>
    /// <param name="target">The challenged player, who is asked to accept or refuse.</param>
    /// <returns>The task.</returns>
    ValueTask DuelRequestedAsync(Player requester, Player target);
}
