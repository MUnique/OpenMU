// <copyright file="ShowAllianceListPlugIn.cs" company="MUnique">
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
/// The default implementation of the <see cref="IShowAllianceListPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.ShowAllianceListPlugIn_Name), Description = nameof(PlugInResources.ShowAllianceListPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A8B9C0D1-E2F3-4A4B-5C6D-7E8F9A0B1C2D")]
public class ShowAllianceListPlugIn : IShowAllianceListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowListAsync(IEnumerable<AllianceGuildEntry> guilds)
    {
        var connection = this._player.Connection;
        if (connection is null)
        {
            return;
        }

        var guildList = guilds.ToList();
        int guildCount = guildList.Count;

        int Write()
        {
            var size = AllianceList.GetRequiredSize(guildCount);
            var packet = new AllianceListRef(connection.Output.GetSpan(size)[..size])
            {
                GuildCount = (byte)guildCount,
            };

            for (int i = 0; i < guildCount; i++)
            {
                var entry = packet[i];
                entry.GuildId = guildList[i].GuildId;
                entry.GuildName = guildList[i].GuildName;
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
