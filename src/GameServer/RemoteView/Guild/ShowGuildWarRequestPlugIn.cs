// <copyright file="ShowGuildWarRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildWarRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowGuildWarRequestPlugIn), "The default implementation of the IShowGuildWarRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("128250B3-6CFC-4C89-BF8A-B50C892A78D3")]
public class ShowGuildWarRequestPlugIn : IShowGuildWarRequestPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildWarRequestPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildWarRequestPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRequestAsync(string requestingGuildName, GameLogic.GuildWar.GuildWarType warType)
    {
        await this._player.Connection.SendGuildWarRequestAsync(requestingGuildName, warType.Convert()).ConfigureAwait(false);
    }
}