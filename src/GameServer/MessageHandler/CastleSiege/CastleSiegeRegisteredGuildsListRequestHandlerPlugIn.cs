// <copyright file="CastleSiegeRegisteredGuildsListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for castle siege registered guilds list request packets.
/// </summary>
[PlugIn(nameof(CastleSiegeRegisteredGuildsListRequestHandlerPlugIn), "Handler for castle siege registered guilds list request packets.")]
[Guid("C9D0E1F2-5678-78CD-CDEF-901234567DEF")]
internal class CastleSiegeRegisteredGuildsListRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeRegisteredGuildsListRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var registeredGuilds = new List<(Guild Guild, int MarksSubmitted)>();

        if (player.CurrentMap?.CastleSiegeContext is { } castleSiegeContext
            && player.GameContext is IGameServerContext serverContext)
        {
            foreach (var registration in castleSiegeContext.RegisteredAlliances)
            {
                var guild = await serverContext.GuildServer.GetGuildAsync(registration.AllianceMasterGuildId).ConfigureAwait(false);
                if (guild is not null)
                {
                    registeredGuilds.Add((guild, registration.GuildMarksSubmitted));
                }
            }
        }

        await player.InvokeViewPlugInAsync<IShowCastleSiegeRegisteredGuildsPlugIn>(
            p => p.ShowRegisteredGuildsAsync(registeredGuilds)).ConfigureAwait(false);
    }
}
