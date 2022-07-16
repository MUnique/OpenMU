// <copyright file="EventQuestStateRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Quest;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for event quest state request packets (0xF6, 0x21 identifier).
/// </summary>
[PlugIn("Quest - Event quest state request", "Packet handler for event quest state request packets (0xF6, 0x21 identifier)")]
[Guid("907FFFF8-0CF2-4DCD-931F-8AA17FE2EF3D")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
public class EventQuestStateRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => EventQuestStateListRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<IQuestEventResponsePlugIn>(p => p.ShowActiveEventQuestsAsync()).ConfigureAwait(false);
    }
}