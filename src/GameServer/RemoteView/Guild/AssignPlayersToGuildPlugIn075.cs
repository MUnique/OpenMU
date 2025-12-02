// <copyright file="AssignPlayersToGuildPlugIn075.cs" company="MUnique">
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
[PlugIn(nameof(AssignPlayersToGuildPlugIn075), "The default implementation of the IAssignPlayersToGuildPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("ABFA2CBD-1AB0-4F56-97A7-FCF458865ACF")]
[MaximumClient(0, 89, ClientLanguage.Invariant)]
public class AssignPlayersToGuildPlugIn075 : BaseGuildInfoPlugIn<AssignPlayersToGuildPlugIn075>, IAssignPlayersToGuildPlugIn
{
    private readonly RemotePlayer _player;
    private readonly HashSet<uint> _transmittedGuilds = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignPlayersToGuildPlugIn075"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public AssignPlayersToGuildPlugIn075(RemotePlayer player)
        : base(player)
    {
        this._player = player;
    }

    /// <inheritdoc />
    public async ValueTask AssignPlayersToGuildAsync(ICollection<Player> guildPlayers, bool appearsNew)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        foreach (var guildPlayer in guildPlayers)
        {
            await this.SendGuildInfoIfRequiredAsync(guildPlayer).ConfigureAwait(false);
        }

        int Write()
        {
            var size = AssignCharacterToGuild075Ref.GetRequiredSize(guildPlayers.Count);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AssignCharacterToGuild075Ref(span)
            {
                PlayerCount = (byte)guildPlayers.Count,
            };

            int i = 0;
            foreach (var guildPlayer in guildPlayers)
            {
                this.SetGuildPlayerBlock(packet[i], guildPlayer);
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

        await this.SendGuildInfoIfRequiredAsync(guildPlayer).ConfigureAwait(false);

        // C2 00 11
        // 65
        // 01
        // 34 4B 00 00 80 00 00
        // A4 F2 00 00 00
        int Write()
        {
            var size = AssignCharacterToGuild075Ref.GetRequiredSize(1);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AssignCharacterToGuild075Ref(span)
            {
                PlayerCount = 1,
            };

            this.SetGuildPlayerBlock(packet[0], guildPlayer);
            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override Memory<byte> Serialize(Interfaces.Guild guild, uint guildId)
    {
        var array = new byte[GuildInformations075.GetRequiredSize(1)];
        var result = new GuildInformations075(array) { GuildCount = 1 };

        var guildInfo = result[0];
        guildInfo.GuildId = (ushort)guildId;
        guildInfo.GuildName = guild.Name ?? string.Empty;
        guild.Logo.CopyTo(guildInfo.Logo);
        return array.AsMemory();
    }

    private async ValueTask SendGuildInfoIfRequiredAsync(Player guildPlayer)
    {
        if (guildPlayer.GuildStatus is not { } guildStatus)
        {
            return;
        }

        if (this._transmittedGuilds.Contains(guildStatus.GuildId))
        {
            return;
        }

        var data = await this.GetGuildDataAsync(guildStatus.GuildId).ConfigureAwait(false);
        if (data.Length == 0)
        {
            return;
        }

        var connection = this.Player.Connection;
        if (connection is null)
        {
            return;
        }

        this._transmittedGuilds.Add(guildStatus.GuildId);

        // guildInfo is the cached, serialized result of the GuildInformation-Class.
        int Write()
        {
            var target = connection.Output.GetSpan(data.Length);
            data.Span.CopyTo(target);
            return data.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    private void SetGuildPlayerBlock(AssignCharacterToGuild075Ref.GuildMemberRelationRef playerBlock, Player guildPlayer)
    {
        if (guildPlayer.GuildStatus is null)
        {
            return;
        }

        playerBlock.GuildId = (ushort)guildPlayer.GuildStatus.GuildId;
        playerBlock.PlayerId = guildPlayer.GetId(this._player);
    }
}