// <copyright file="DoppelgangerRewardChestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Opens the reward chests of the doppelganger event, when a player talks to them.
/// The client handles the chests as NPCs, so they're opened by clicking on them instead of attacking them.
/// </summary>
[Guid("3E9A6C51-7B28-4F0D-A1C4-58D2E96B7F30")]
[PlugIn]
[Display(Name = nameof(PlugInResources.DoppelgangerRewardChestPlugIn_Name), Description = nameof(PlugInResources.DoppelgangerRewardChestPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class DoppelgangerRewardChestPlugIn : IPlayerTalkToNpcPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (player.CurrentMiniGame is not DoppelgangerContext context
            || npc is not Destructible chest
            || !context.IsRewardChest(chest))
        {
            return;
        }

        // Mark as handled before any await, so that the TalkNpcAction sees it synchronously.
        eventArgs.HasBeenHandled = true;
        await context.OpenRewardChestAsync(player, chest).ConfigureAwait(false);
    }
}
