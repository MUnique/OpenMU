// <copyright file="ChatRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for chat request packets.
/// </summary>
[PlugIn("ChatRequestHandlerPlugIn", "Handler for chat request packets.")]
[Guid("acf9263f-ba71-4d84-b8f8-84e494eb4462")]
internal class ChatRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ChatRequestAction _chatRequestAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ChatRoomCreateRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ChatRoomCreateRequest message = packet;
        await this._chatRequestAction.RequestChatAsync(player, message.FriendName).ConfigureAwait(false);
    }
}