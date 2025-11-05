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
[PlugIn(nameof(ShowGuildListPlugIn), "The default implementation of the IShowGuildListPlugIn which is forwarding everything to the game client with specific data packets.")]
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
        
        // Get guild war information
        string rivalGuildName = string.Empty;
        byte currentScore = 0;
        byte totalScore = 0;

        if (this._player.GuildWarContext is { } warContext)
        {
            // Active guild war
            rivalGuildName = warContext.EnemyTeamName;
            currentScore = warContext.ThisScore;
            totalScore = warContext.Score.MaximumScore;
        }
        else if (this._player.GuildStatus is { } guildStatus)
        {
            // Check for hostility relationship
            var guildServer = this._player.GameServerContext.GuildServer;
            var guild = await guildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false);
            if (guild?.Hostility is { } hostileGuild)
            {
                rivalGuildName = hostileGuild.Name ?? string.Empty;
                currentScore = (byte)Math.Min(255, Math.Max(0, guild.Score));
                totalScore = (byte)Math.Min(255, Math.Max(0, hostileGuild.Score));
            }
        }

        int Write()
        {
            var size = GuildListRef.GetRequiredSize(playerCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new GuildListRef(span)
            {
                GuildMemberCount = (byte)playerCount,
                IsInGuild = playerCount > 0,
                RivalGuildName = rivalGuildName,
                CurrentScore = currentScore,
                TotalScore = totalScore,
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