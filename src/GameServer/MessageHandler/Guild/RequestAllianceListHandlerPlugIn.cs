// <copyright file="RequestAllianceListHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for alliance list request packets.
/// </summary>
[PlugIn(nameof(RequestAllianceListHandlerPlugIn), "Handler for alliance list request packets.")]
[Guid("5E6F7890-9012-1234-3456-567890ABCDEF")]
internal class RequestAllianceListHandlerPlugIn : IPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RequestAllianceList.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            return;
        }

        var allianceGuildIds = await serverContext.GuildServer.GetAllianceMemberGuildIdsAsync(guildStatus.GuildId).ConfigureAwait(false);
        var allianceGuilds = new List<Guild>();

        foreach (var guildId in allianceGuildIds)
        {
            var guild = await serverContext.GuildServer.GetGuildAsync(guildId).ConfigureAwait(false);
            if (guild is not null)
            {
                allianceGuilds.Add(guild);
            }
        }

        await player.InvokeViewPlugInAsync<IShowAllianceListPlugIn>(p => p.ShowAllianceListAsync(allianceGuilds)).ConfigureAwait(false);
    }
}
