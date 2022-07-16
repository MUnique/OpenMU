// <copyright file="GuildWarScoreUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IGuildWarScoreUpdatePlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(IGuildWarScoreUpdatePlugIn), "The default implementation of the IGuildWarScoreUpdatePlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("24E5E0C1-DAE7-4E34-810D-C622F8F9B70F")]
public class GuildWarScoreUpdatePlugIn : IGuildWarScoreUpdatePlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildWarScoreUpdatePlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public GuildWarScoreUpdatePlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask UpdateScoreAsync()
    {
        if (this._player.GuildWarContext is { } guildWarContext)
        {
            await this._player.Connection.SendGuildWarScoreUpdateAsync(guildWarContext.ThisScore, guildWarContext.EnemyScore).ConfigureAwait(false);
        }
    }
}