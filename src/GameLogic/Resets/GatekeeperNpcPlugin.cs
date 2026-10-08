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
[PlugIn]
[Display(Name = nameof(PlugInResources.GatekeeperNpcPlugin_Name), Description = nameof(PlugInResources.GatekeeperNpcPlugin_Description), ResourceType = typeof(PlugInResources))]
public class GatekeeperNpcPlugin : NpcTalkPlugInBase
{
    /// <summary>
    /// The number of the 'Gatekeeper' in the Barracks of Balgass.
    /// </summary>
    internal const short GatekeeperNumber = 408;

    /// <inheritdoc />
    public override short DefaultNpcNumber => GatekeeperNumber;

    /// <inheritdoc />
    protected override ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        // The client opens the dialog itself, so we don't need to do anything here.
        eventArgs.HasBeenHandled = true;
        eventArgs.LeavesDialogOpen = true;
        return ValueTask.CompletedTask;
    }
}