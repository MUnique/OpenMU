// <copyright file="BloodCastleArchangelTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles talking to the Archangel in the blood castle event, where players hand in the quest item to win the event.
/// </summary>
[Guid("93AC4541-4BB7-4833-A7B0-902AB9D87089")]
[PlugIn]
[Display(Name = nameof(PlugInResources.BloodCastleArchangelTalkPlugIn_Name), Description = nameof(PlugInResources.BloodCastleArchangelTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class BloodCastleArchangelTalkPlugIn : NpcTalkPlugInBase
{
    /// <summary>
    /// The number of the Archangel NPC.
    /// </summary>
    internal const short ArchangelNumber = 232;

    /// <inheritdoc />
    public override short DefaultNpcNumber => ArchangelNumber;

    /// <inheritdoc />
    protected override async ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (player.CurrentMiniGame is not BloodCastleContext bloodCastle)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        await bloodCastle.TalkToNpcArchangelAsync(player).ConfigureAwait(false);
    }
}
