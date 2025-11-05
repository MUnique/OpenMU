// <copyright file="ShowCastleSiegeRegisteredGuildsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The default implementation of the <see cref="IShowCastleSiegeRegisteredGuildsPlugIn"/> which is forwarding everything to the game client with specific data packets.
/// </summary>
[PlugIn(nameof(ShowCastleSiegeRegisteredGuildsPlugIn), "The default implementation of the IShowCastleSiegeRegisteredGuildsPlugIn which is forwarding everything to the game client with specific data packets.")]
[Guid("B8C9D0E1-4567-67BC-CDEF-890123456CDE")]
public class ShowCastleSiegeRegisteredGuildsPlugIn : IShowCastleSiegeRegisteredGuildsPlugIn
{
    private readonly RemotePlayer _player;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShowCastleSiegeRegisteredGuildsPlugIn"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public ShowCastleSiegeRegisteredGuildsPlugIn(RemotePlayer player) => this._player = player;

    /// <inheritdoc/>
    public async ValueTask ShowRegisteredGuildsAsync(IEnumerable<(Guild Guild, int MarksSubmitted)> registeredGuilds)
    {
        if (this._player.Connection is not { } connection)
        {
            return;
        }

        var guildList = registeredGuilds.ToList();
        var guildCount = (byte)guildList.Count;

        int Write()
        {
            var size = CastleSiegeRegisteredGuildsRef.GetRequiredSize(guildCount);
            var span = connection.Output.GetSpan(size)[..size];
            var packet = new CastleSiegeRegisteredGuildsRef(span)
            {
                GuildCount = guildCount,
            };

            for (int i = 0; i < guildList.Count; i++)
            {
                var guild = guildList[i];
                var guildBlock = packet[i];
                guildBlock.GuildName = guild.Guild.Name ?? string.Empty;
                guildBlock.MarksSubmitted = (uint)guild.MarksSubmitted;
                guildBlock.IsAllianceMaster = 1; // All registered guilds are alliance masters
            }

            return size;
        }

        await connection.SendAsync(Write).ConfigureAwait(false);
    }
}
