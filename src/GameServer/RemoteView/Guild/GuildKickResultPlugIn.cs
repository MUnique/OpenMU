// <copyright file="GuildKickResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGuildKickResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("GuildKickResultPlugIn", "The default implementation of the IGuildKickResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("0e91e131-12c4-4add-9439-febb7d444083")]
public class GuildKickResultPlugIn : IGuildKickResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildKickResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GuildKickResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask GuildKickResultAsync(GuildKickSuccess successCode)
    {
        await this._player.Connection.SendGuildKickResponseAsync(successCode.Convert()).ConfigureAwait(false);
    }
}