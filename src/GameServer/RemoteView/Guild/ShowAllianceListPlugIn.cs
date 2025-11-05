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
[PlugIn(nameof(ShowAllianceListPlugIn), "The default implementation of the IShowAllianceListPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("C3D4E5F6-7890-1234-5678-90ABCDEF0123")]
public class ShowAllianceListPlugIn : IShowAllianceListPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowAllianceListPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowAllianceListPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowAllianceListAsync(IEnumerable<Guild> allianceGuilds)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var guildList = allianceGuilds.ToList();
        var guildCount = (byte)guildList.Count;

        int Write()
        {
            var size = AllianceListRef.GetRequiredSize(guildCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new AllianceListRef(span)
            {
                GuildCount = guildCount,
            };

            for (int i = 0; i < guildList.Count; i++)
            {
                var guild = guildList[i];
                var guildEntry = packet[i];
                guildEntry.GuildName = guild.Name ?? string.Empty;

                // A guild is the alliance master if it has no AllianceGuild (i.e., it IS the alliance master)
                guildEntry.IsMasterGuild = (byte)(guild.AllianceGuild == null ? 1 : 0);
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
