// <copyright file="GuildSoccerTimeUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGuildSoccerTimeUpdatePlugIn"/>, which sends the
/// <see cref="GuildSoccerTimeUpdate"/> packet (F3 22) to the game client.
/// </summary>
[PlugIn]
[Display(Name = "Guild Soccer Time Update", Description = "Shows the remaining time of a battle soccer match (F3 22) to the players on the soccer map.")]
[Guid("7B1C2D3E-4F50-4A61-9B72-8C93D0A1B2C5")]
public class GuildSoccerTimeUpdatePlugIn : IGuildSoccerTimeUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildSoccerTimeUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GuildSoccerTimeUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateSoccerTimeAsync(ushort remainingSeconds)
    {
        await this._player.Connection.SendGuildSoccerTimeUpdateAsync(remainingSeconds).ConfigureAwait(false);
    }
}
