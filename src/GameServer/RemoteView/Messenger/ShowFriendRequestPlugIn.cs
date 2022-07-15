// <copyright file="ShowFriendRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowFriendRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowFriendRequestPlugIn", "The default implementation of the IShowFriendRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("efb8b51b-181a-4a7d-b5a4-34c0dbd41748")]
public class ShowFriendRequestPlugIn : IShowFriendRequestPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowFriendRequestPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowFriendRequestPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowFriendRequestAsync(string requester)
    {
        await this._player.Connection.SendFriendRequestAsync(requester).ConfigureAwait(false);
    }
}