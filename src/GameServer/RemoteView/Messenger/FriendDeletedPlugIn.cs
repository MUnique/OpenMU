// <copyright file="FriendDeletedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Messenger;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IFriendDeletedPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("FriendDeletedPlugIn", "The default implementation of the IFriendDeletedPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("3f6f86ec-ffe8-4fa7-82c3-4a743fab7157")]
public class FriendDeletedPlugIn : IFriendDeletedPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendDeletedPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public FriendDeletedPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask FriendDeletedAsync(string deletedFriend)
    {
        await this._player.Connection.SendFriendDeletedAsync(deletedFriend).ConfigureAwait(false);
    }
}