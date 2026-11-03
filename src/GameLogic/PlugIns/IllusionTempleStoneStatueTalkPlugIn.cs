// <copyright file="IllusionTempleStoneStatueTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles talking to the Stone Statue in the illusion temple event, where players pick up the sacred relic.
/// </summary>
[Guid("3A1D6E82-5C47-4F0B-9A36-D81E4B27C590")]
[PlugIn]
[Display(Name = nameof(PlugInResources.IllusionTempleStoneStatueTalkPlugIn_Name), Description = nameof(PlugInResources.IllusionTempleStoneStatueTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
public class IllusionTempleStoneStatueTalkPlugIn : NpcTalkPlugInBase
{
    /// <summary>
    /// The number of the Stone Statue NPC, which holds the sacred relic during a match.
    /// </summary>
    internal const short StoneStatueNumber = 380;

    /// <inheritdoc />
    public override short DefaultNpcNumber => StoneStatueNumber;

    /// <inheritdoc />
    protected override async ValueTask PlayerTalksToConfiguredNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (player.CurrentMiniGame is not IllusionTempleContext illusionTemple)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        await illusionTemple.TalkToNpcStoneStatueAsync(player).ConfigureAwait(false);
    }
}
