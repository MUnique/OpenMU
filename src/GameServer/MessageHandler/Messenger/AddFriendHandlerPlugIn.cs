// <copyright file="AddFriendHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for add friend packets.
/// </summary>
[PlugIn("AddFriendHandlerPlugIn", "Handler for add friend packets.")]
[Guid("302870db-59cc-4cf8-b5ed-b0efa9f6ccbc")]
internal class AddFriendHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AddFriendAction _addAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => FriendAddRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        FriendAddRequest message = packet;
        await this._addAction.AddFriendAsync(player, message.FriendName).ConfigureAwait(false);
    }
}