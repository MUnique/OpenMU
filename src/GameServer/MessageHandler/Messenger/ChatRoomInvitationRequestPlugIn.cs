// <copyright file="ChatRoomInvitationRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler which handles chat room invitation requests,
/// where a player invites additional players to an existing chat room.
/// </summary>
[PlugIn("ChatRoomInvitationRequestPlugIn", "Packet handler which handles chat room invitation requests, where a player invites additional players to an existing chat room.")]
[Guid("fc779867-7d2d-4409-83b4-b6616bb9234e")]
public class ChatRoomInvitationRequestPlugIn : IPacketHandlerPlugIn
{
    private readonly ChatRequestAction _chatRequestAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ChatRoomInvitationRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ChatRoomInvitationRequest message = packet;
        await this._chatRequestAction.InviteFriendToChatAsync(player, message.FriendName, message.RoomId, message.RequestId).ConfigureAwait(false);
    }
}