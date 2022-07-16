// <copyright file="ShowShowGuildWarRequestResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowShowGuildWarRequestResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowShowGuildWarRequestResultPlugIn), "The default implementation of the IShowShowGuildWarRequestResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("7DDF834C-218B-4F35-B66C-54579BE485D5")]
public class ShowShowGuildWarRequestResultPlugIn : IShowShowGuildWarRequestResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowShowGuildWarRequestResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowShowGuildWarRequestResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResultAsync(GameLogic.Views.Guild.GuildWarRequestResult result)
    {
        await this._player.Connection.SendGuildWarRequestResultAsync(result.Convert()).ConfigureAwait(false);
    }
}