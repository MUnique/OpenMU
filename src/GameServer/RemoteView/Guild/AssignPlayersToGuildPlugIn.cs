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
    public void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew)
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
        using var writer = connection.StartSafeWrite(0xC2, AssignCharacterToGuild.GetRequiredSize(guildPlayers.Count));
        var packet = new AssignCharacterToGuild(writer.Span)
        {
            PlayerCount = (byte)guildPlayers.Count,
        };

        int i = 0;
        foreach (var guildPlayer in guildPlayers)
        {
            this.SetGuildPlayerBlock(packet[i], guildPlayer, appearsNew);
            i++;
        }

        writer.Commit();
    }

    /// <inheritdoc />
    public void AssignPlayerToGuild(Player guildPlayer, bool appearsNew)
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
        using var writer = connection.StartSafeWrite(AssignCharacterToGuild.HeaderType, AssignCharacterToGuild.GetRequiredSize(1));
        var packet = new AssignCharacterToGuild(writer.Span)
        {
            PlayerCount = 1,
        };

        this.SetGuildPlayerBlock(packet[0], guildPlayer, appearsNew);

        writer.Commit();
    }

    private void SetGuildPlayerBlock(AssignCharacterToGuild.GuildMemberRelation playerBlock, Player guildPlayer, bool appearsNew)
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