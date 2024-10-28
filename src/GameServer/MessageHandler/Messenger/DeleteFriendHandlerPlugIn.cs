// <copyright file="DeleteFriendHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for delete friend packets.
/// </summary>
[PlugIn("DeleteFriendHandlerPlugIn", "Handler for delete friend packets.")]
[Guid("82d21573-64bd-439e-9368-8fc227475942")]
internal class DeleteFriendHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly DeleteFriendAction _deleteAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => FriendDelete.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        FriendDelete message = packet;
        await this._deleteAction.DeleteFriendAsync(player, message.FriendName).ConfigureAwait(false);
    }
}