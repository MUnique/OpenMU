﻿// <copyright file="QuestStartedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IQuestStartedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("Quest - Started", "The default implementation of the IQuestStartedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("2D813203-8266-4BB7-A267-E476AD11AC4B")]
public class QuestStartedPlugIn : IQuestStartedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestStartedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestStartedPlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc/>
    public void QuestStarted(QuestDefinition quest)
    {
        if (quest.Group == QuestConstants.LegacyQuestGroup)
        {
            this._player.Connection?.SendLegacySetQuestStateResponse((byte)quest.Number, 0, this._player.GetLegacyQuestStateByte());
        }
        else
        {
            var state = this._player.GetQuestState(quest.Group);
            if (state?.ActiveQuest == quest)
            {
                this._player.Connection?.SendQuestStepInfo((ushort)quest.Number, (ushort)quest.Group);
                this._player.ViewPlugIns.GetPlugIn<IQuestProgressPlugIn>()?.ShowQuestProgress(quest, true);
            }
        }
    }
}