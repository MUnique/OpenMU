// <copyright file="IllusionTempleTeamStorageTalkPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// Base class for the plugins which handle talking to one of the two team storages of the illusion
/// temple event. Carrying the sacred relic to the own team's storage is how a team scores, so each
/// team has its own NPC and both are handled the same way.
/// </summary>
public abstract class IllusionTempleTeamStorageTalkPlugInBase : NpcTalkPlugInBase
{
    /// <inheritdoc />
    protected override async ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (player.CurrentMiniGame is not IllusionTempleContext illusionTemple)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        await illusionTemple.TalkToNpcTeamStorageAsync(npc.Definition.Number, player).ConfigureAwait(false);
    }
}
