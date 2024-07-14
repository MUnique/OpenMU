// <copyright file="GatekeeperNpcPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the gatekeeper npc in the Barracks of Balgass.
/// </summary>
[Guid("1B7BCA14-3124-4550-94B4-3FFCEE1FD55A")]
[PlugIn(nameof(GatekeeperNpcPlugin), "Handle Gatekeeper NPC Request")]
public class GatekeeperNpcPlugin : IPlayerTalkToNpcPlugIn
{
    /// <summary>
    /// Gets the NPC number of 'Gatekeeper' in Barracks of Balgass.
    /// </summary>
    public static short GatekeeperNpcNumber => 408;

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != GatekeeperNpcNumber)
        {
            return;
        }

        // The client opens the dialog itself, so we don't need to do anything here.
        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;
    }
}