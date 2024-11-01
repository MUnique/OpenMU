// <copyright file="FriendAddResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for friend add response packets.
/// </summary>
[PlugIn("FriendAddResponseHandlerPlugIn", "Handler for friend add response packets.")]
[Guid("171b8f75-3927-4325-b694-54130365e4a2")]
internal class FriendAddResponseHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly AddResponseAction _responseAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => FriendAddResponse.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        FriendAddResponse message = packet;
        await this._responseAction.ProceedResponseAsync(player, message.FriendRequesterName, message.Accepted).ConfigureAwait(false);
    }
}