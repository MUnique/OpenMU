// <copyright file="ShowGuildInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowGuildInfoPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn("ShowGuildInfoPlugIn", "The default implementation of the IShowGuildInfoPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("65f9d310-4adf-48b4-ace2-16779b254ecf")]
public class ShowGuildInfoPlugIn : BaseGuildInfoPlugIn<ShowGuildInfoPlugIn>, IShowGuildInfoPlugIn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShowGuildInfoPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowGuildInfoPlugIn(RemotePlayer player)
        : base(player)
    {
    }

    /// <inheritdoc/>
    public async ValueTask ShowGuildInfoAsync(uint guildId)
    {
        var data = await this.GetGuildDataAsync(guildId).ConfigureAwait(false);
        if (data.Length == 0)
        {
            return;
        }

        var connection = this.Player.Connection;
        if (connection is null)
        {
            return;
        }

        // guildInfo is the cached, serialized result of the GuildInformation-Class.
        int Write()
        {
            var target = connection.Output.GetSpan(data.Length);
            data.Span.CopyTo(target);
            return data.Length;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override Memory<byte> Serialize(Guild guild, uint guildId)
    {
        /*
     *  C1 3C 66 00
        87 38 00 00 // guild number
        00  // guild type
        54 68 65 4F 6E 65 00 00 //TheOne - Maintain
        41 76 61 6C 6F 6E 00 2B //Avalon - Assistant
        18 88 88 81 18 66 66 81 18 61 16 81 18 61 16 81 18 66 66 81 18 61 16 81 18 61 16 81 18 61 16 81 //Guild Logo
        F9 96 7C //?
     */
        var array = new byte[GuildInformation.Length];
        var result = new GuildInformation(array)
        {
            GuildId = guildId,
            GuildName = guild.Name ?? string.Empty,
        };
        if (guild.AllianceGuild != null)
        {
            result.AllianceGuildName = guild.AllianceGuild.Name ?? string.Empty;
        }

        guild.Logo.CopyTo(result.Logo);
        return array.AsMemory();
    }
}