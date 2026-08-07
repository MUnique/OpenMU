// <copyright file="CastleSiegeLeverTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CastleSiege.Actions;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Opens the operation interface of the gate associated with a Castle Siege lever.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeLeverTalkPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeLeverTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("92DC9090-A4B1-4431-BF12-280DCDB8877B")]
public sealed class CastleSiegeLeverTalkPlugIn : IPlayerTalkToNpcPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(
        Player player,
        NonPlayerCharacter npc,
        NpcTalkEventArgs eventArgs)
    {
        if (npc is not CastleSiegeLever lever || lever.Gate is not { } gate)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;
        await CastleSiegeGateOperateAction.ShowInterfaceAsync(player, lever.Context, gate.Id).ConfigureAwait(false);
    }
}
