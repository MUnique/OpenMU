// <copyright file="ShowGuildWarResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildWarResultPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowGuildWarResultPlugIn), "The default implementation of the IShowGuildWarResultPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("991247B9-D4A3-466D-A7CE-2621843CA94F")]
public class ShowGuildWarResultPlugIn : IShowGuildWarResultPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildWarResultPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildWarResultPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowResultAsync(string hostileGuildName, GuildWarResult result)
    {
        await this._player.Connection.SendGuildWarEndedAsync(result.Convert(), hostileGuildName).ConfigureAwait(false);
    }
}