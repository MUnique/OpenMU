// <copyright file="ShowGuildWarDeclaredPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildWarDeclaredPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowGuildWarDeclaredPlugIn), "The default implementation of the IShowGuildWarDeclaredPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("50393D5D-01F2-43F8-B5D7-243D91B905BC")]
public class ShowGuildWarDeclaredPlugIn : IShowGuildWarDeclaredPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildWarDeclaredPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildWarDeclaredPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowDeclaredAsync()
    {
        if (this._player.GuildWarContext is { } guildWarContext)
        {
            await this._player.Connection.SendGuildWarDeclaredAsync(guildWarContext.EnemyTeamName, guildWarContext.WarType.Convert(), (byte)guildWarContext.Team).ConfigureAwait(false);
        }
    }
}