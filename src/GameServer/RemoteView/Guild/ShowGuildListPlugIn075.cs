// <copyright file="ShowGuildListPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowGuildListPlugIn075), "The default implementation of the IShowGuildListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("1A7148DF-6E1B-47C7-9148-426F5E35F421")]
[MaximumClient(0, 89, ClientLanguage.Invariant)]
public class ShowGuildListPlugIn075 : IShowGuildListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildListPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildListPlugIn075(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowGuildListAsync(IEnumerable<GuildListEntry> players)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var sortedPlayers = players
            .OrderBy(p => p.PlayerPosition != GuildPosition.GuildMaster)
            .ThenBy(p => p.PlayerName)
            .ToList();
        var playerCount = sortedPlayers.Count;
        int Write()
        {
            var size = GuildList075Ref.GetRequiredSize(playerCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new GuildList075Ref(span)
            {
                GuildMemberCount = (byte)playerCount,
                IsInGuild = playerCount > 0,
                CurrentScore = 0, // TODO
                TotalScore = 0, // TODO
            };

            int i = 0;
            foreach (var member in sortedPlayers)
            {
                var memberBlock = packet[i];
                memberBlock.Name = member.PlayerName;
                memberBlock.ServerId = member.ServerId;
                memberBlock.ServerId2 = (byte)(member.ServerId == 0xFF ? 0x7F : 0x80 + member.ServerId);
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}