// <copyright file="GuildSoccerScoreUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGuildSoccerScoreUpdatePlugIn"/>, which sends the
/// <see cref="GuildSoccerScoreUpdate"/> packet (F3 23) to the game client.
/// </summary>
[PlugIn]
[Display(Name = "Guild Soccer Score Update", Description = "Shows the battle soccer score board (F3 23) to the players on the soccer map.")]
[Guid("7B1C2D3E-4F50-4A61-9B72-8C93D0A1B2C4")]
public class GuildSoccerScoreUpdatePlugIn : IGuildSoccerScoreUpdatePlugIn
{
    /// <summary>
    /// The value of the red team goals which tells the client to hide the score board.
    /// </summary>
    private const byte HideBoardValue = byte.MaxValue;

    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildSoccerScoreUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GuildSoccerScoreUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateSoccerScoreAsync(string redTeamName, byte redTeamGoals, string blueTeamName, byte blueTeamGoals)
    {
        await this._player.Connection.SendGuildSoccerScoreUpdateAsync(
            redTeamName,
            Math.Min(redTeamGoals, (byte)(HideBoardValue - 1)),
            blueTeamName,
            blueTeamGoals).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask HideSoccerScoreAsync()
    {
        await this._player.Connection.SendGuildSoccerScoreUpdateAsync(string.Empty, HideBoardValue, string.Empty, 0).ConfigureAwait(false);
    }
}
