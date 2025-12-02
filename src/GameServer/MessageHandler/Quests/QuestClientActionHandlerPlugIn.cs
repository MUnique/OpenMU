// <copyright file="QuestClientActionHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Quests;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for quest client action packets (0xF6, 0x10 identifier).
/// </summary>
[PlugIn("Quest - Client Action", "Packet handler for quest client action packets (0xF6, 0x10 identifier)")]
[Guid("02F632AB-17E4-4B73-90DA-92FD5310B3CF")]
[BelongsToGroup(QuestGroupHandlerPlugIn.GroupKey)]
public class QuestClientActionHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly QuestClientAction _questClientAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => QuestClientActionRequest.SubCode;

    /// <inheritdoc />
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        QuestClientActionRequest request = packet;
        this._questClientAction.ClientAction(player, (short)request.QuestGroup, (short)request.QuestNumber);
    }
}