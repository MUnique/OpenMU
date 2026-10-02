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
public class BloodCastleArchangelTalkPlugIn : IPlayerTalkToNpcPlugIn, ISupportCustomConfiguration<BloodCastleArchangelTalkPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc />
    public BloodCastleArchangelTalkPlugInConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        var configuration = this.Configuration ??= new BloodCastleArchangelTalkPlugInConfiguration();
        if (player.CurrentMiniGame is not BloodCastleContext bloodCastle
            || npc.Definition.Number != configuration.ArchangelNumber)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        await bloodCastle.TalkToNpcArchangelAsync(player).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public object CreateDefaultConfig() => new BloodCastleArchangelTalkPlugInConfiguration();
}
