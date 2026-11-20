// <copyright file="QuestCompletionResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Quest;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Properties;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;
using PlugInResources = MUnique.OpenMU.GameServer.Properties.PlugInResources;

/// <summary>
/// The default implementation of the <seealso cref="IQuestCompletionResponsePlugIn" /> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.QuestCompletionResponsePlugIn_Name), Description = nameof(PlugInResources.QuestCompletionResponsePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("4BCA9C67-A695-4244-9F5A-3B7CAC049DB4")]
public class QuestCompletionResponsePlugIn : IQuestCompletionResponsePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestCompletionResponsePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public QuestCompletionResponsePlugIn(RemotePlayer player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask QuestCompletedAsync(QuestDefinition quest)
    {
        if (quest.Group == QuestConstants.LegacyQuestGroup)
        {
            await this._player.Connection.SendLegacySetQuestStateResponseAsync((byte)quest.Number, 0, this._player.GetLegacyQuestStateByte()).ConfigureAwait(false);
        }
        else
        {
            await this._player.Connection.SendQuestCompletionResponseAsync((ushort)quest.Number, (ushort)quest.Group, QuestCompletionResponse.QuestCompletionResult.Completed).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask QuestCompletionRefusedAsync(QuestDefinition quest, QuestCompletionRefusal refusal)
    {
        if (quest.Group == QuestConstants.LegacyQuestGroup)
        {
            // The legacy quest state response has no result for a refusal which the client shows, so a message tells why.
            var message = refusal == QuestCompletionRefusal.ZenLimitReached ? nameof(PlayerMessage.QuestZenLimitReached) : nameof(PlayerMessage.InventoryFull);
            await this._player.ShowLocalizedBlueMessageAsync(message).ConfigureAwait(false);
            return;
        }

        var result = refusal switch
        {
            QuestCompletionRefusal.InventoryFull => QuestCompletionResponse.QuestCompletionResult.InventoryFull,
            QuestCompletionRefusal.ZenLimitReached => QuestCompletionResponse.QuestCompletionResult.ZenLimitReached,
            _ => QuestCompletionResponse.QuestCompletionResult.NotCompleted,
        };
        await this._player.Connection.SendQuestCompletionResponseAsync((ushort)quest.Number, (ushort)quest.Group, result).ConfigureAwait(false);
    }
}