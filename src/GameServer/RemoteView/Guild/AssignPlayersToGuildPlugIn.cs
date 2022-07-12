// <copyright file="AssignPlayersToGuildPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IAssignPlayersToGuildPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(AssignPlayersToGuildPlugIn), "The default implementation of the IAssignPlayersToGuildPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("f42f571e-0cd1-4c22-ba53-8344848ba998")]
[MinimumClient(0, 90, ClientLanguage.Invariant)]
public class AssignPlayersToGuildPlugIn : IAssignPlayersToGuildPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignPlayersToGuildPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AssignPlayersToGuildPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc />
    public async ValueTask AssignPlayersToGuildAsync(ICollection<Player> guildPlayers, bool appearsNew)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        // C2 00 11
        // 65
        // 01
        // 34 4B 00 00 80 00 00
        // A4 F2 00 00 00
        int Write()
        {
            var size = AssignCharacterToGuildRef.GetRequiredSize(guildPlayers.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AssignCharacterToGuildRef(span)
            {
                PlayerCount = (byte)guildPlayers.Count,
            };

            int i = 0;
            foreach (var guildPlayer in guildPlayers)
            {
                this.SetGuildPlayerBlock(packet[i], guildPlayer, appearsNew);
                i++;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask AssignPlayerToGuildAsync(Player guildPlayer, bool appearsNew)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        // C2 00 11
        // 65
        // 01
        // 34 4B 00 00 80 00 00
        // A4 F2 00 00 00
        int Write()
        {
            var size = AssignCharacterToGuildRef.GetRequiredSize(1);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AssignCharacterToGuildRef(span)
            {
                PlayerCount = 1,
            };

            this.SetGuildPlayerBlock(packet[0], guildPlayer, appearsNew);
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private void SetGuildPlayerBlock(AssignCharacterToGuildRef.GuildMemberRelationRef playerBlock, Player guildPlayer, bool appearsNew)
    {
        if (guildPlayer.GuildStatus is null)
        {
            return;
        }

        playerBlock.GuildId = guildPlayer.GuildStatus.GuildId;
        playerBlock.Role = guildPlayer.GuildStatus.Position.Convert();
        playerBlock.PlayerId = guildPlayer.GetId(this._player);
        playerBlock.IsPlayerAppearingNew = appearsNew;
    }
}