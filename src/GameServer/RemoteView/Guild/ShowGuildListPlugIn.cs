// <copyright file="ShowGuildListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(ShowGuildListPlugIn), Description = "The default implementation of the IShowGuildListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("f72a9968-100b-481f-aba1-1dd597fdad47")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class ShowGuildListPlugIn : IShowGuildListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowGuildListAsync(IEnumerable<OpenMU.Interfaces.GuildListEntry> players)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var playerCount = players.Count();
        int Write()
        {
            var size = GuildListRef.GetRequiredSize(playerCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new GuildListRef(span)
            {
                GuildMemberCount = (byte)playerCount,
                IsInGuild = playerCount > 0,
                RivalGuildName = string.Empty, // TODO
                CurrentScore = 0, // TODO
                TotalScore = 0, // TODO
            };

            int i = 0;
            foreach (var member in players)
            {
                var memberBlock = packet[i];
                memberBlock.Name = member.PlayerName;
                memberBlock.Role = member.PlayerPosition.Convert();
                memberBlock.ServerId = member.ServerId;
                memberBlock.ServerId2 = (byte)(member.ServerId == 0xFF ? 0x7F : 0x80 + member.ServerId);
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}