// <copyright file="ChatRoomCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Net;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IChatRoomCreatedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ChatRoomCreatedPlugIn", "The default implementation of the IChatRoomCreatedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("a7c99cb5-94f6-42ea-b6e2-2272a9a81e12")]
public class ChatRoomCreatedPlugIn : IChatRoomCreatedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomCreatedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ChatRoomCreatedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo authenticationInfo, string friendName, bool success)
    {
        var hostAddress = authenticationInfo.HostAddress;
        if (IPAddress.TryParse(authenticationInfo.HostAddress, out var chatServerAddress)
            && chatServerAddress.IsOnSameHost()
            && this._player.Connection?.LocalEndPoint is IPEndPoint localEndPoint)
        {
            hostAddress = localEndPoint.Address.ToString();
        }

        await this._player.Connection.SendChatRoomConnectionInfoAsync(
            hostAddress,
            authenticationInfo.RoomId,
            uint.Parse(authenticationInfo.AuthenticationToken),
            friendName,
            success)
            .ConfigureAwait(false);
    }
}