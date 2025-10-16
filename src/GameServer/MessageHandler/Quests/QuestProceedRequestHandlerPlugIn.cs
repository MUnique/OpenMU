﻿// <copyright file="QuestProceedRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for quest start proceed packets (0xF6, 0x0B identifier).
/// </summary>
[PlugIn("Quest - Proceed Request", "Packet handler for quest proceed request packets (0xF6, 0x0B identifier)")]
[Guid("D3773016-F156-4481-B080-A8C087444B78")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
public class QuestProceedRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly QuestStartAction _questStartAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => QuestProceedRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        QuestProceedRequest request = packet;
        var questGroup = (short)request.QuestGroup;
        var questNumber = (short)request.QuestNumber;
        var questState = player.GetQuestState(questGroup, questNumber);

        if (request.ProceedAction == QuestProceedRequest.QuestProceedAction.AcceptQuest)
        {
            // Enforce additional preconditions for Zyro quests: configurable resets requirement.
            var zyroFeature = player.GameContext.FeaturePlugIns.GetPlugIn<MUnique.OpenMU.GameLogic.Features.ZyroExpansionFeaturePlugIn>();
            if (zyroFeature?.Configuration is { } zyroCfg && questGroup == zyroCfg.QuestGroup)
            {
                var resets = (int)player.Attributes![MUnique.OpenMU.GameLogic.Attributes.Stats.Resets];
                var requiredResets = questNumber switch
                {
                    10 => zyroCfg.ResetsRequiredForVault,
                    20 => zyroCfg.ResetsRequiredForInventory1,
                    30 => zyroCfg.ResetsRequiredForInventory2,
                    _ => 0,
                };

                if (resets < requiredResets)
                {
                    var message = player.GetLocalizedMessage(
                        "Quest_Message_NotEnoughResets",
                        "You need at least {0} resets to start this quest.",
                        requiredResets);
                    await player.InvokeViewPlugInAsync<MUnique.OpenMU.GameLogic.Views.IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MUnique.OpenMU.Interfaces.MessageType.BlueNormal)).ConfigureAwait(false);
                    return;
                }
            }

            if (questState?.ActiveQuest != null)
            {
                // keep it running and confirm that it started
                await player.InvokeViewPlugInAsync<IQuestStartedPlugIn>(p => p.QuestStartedAsync(questState.ActiveQuest)).ConfigureAwait(false);
            }
            else
            {
                await this._questStartAction.StartQuestAsync(player, (short)request.QuestGroup, (short)request.QuestNumber).ConfigureAwait(false);
            }
        }
        else
        {
            // Refused
            if (player.OpenedNpc?.Definition.Quests
                    .FirstOrDefault(q => q.Group == questGroup && q.StartingNumber == questNumber)
                is { } quest)
            {
                await player.InvokeViewPlugInAsync<IQuestStepInfoPlugIn>(p => p.ShowQuestStepInfoAsync(quest.Group, quest.RefuseNumber)).ConfigureAwait(false);
            }
        }
    }
}
