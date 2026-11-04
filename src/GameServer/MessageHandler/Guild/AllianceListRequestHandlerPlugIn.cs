// <copyright file="AllianceListRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for alliance list request packets (C1 E9).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.AllianceListRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.AllianceListRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B3C4D5E6-F7A8-4B9C-0D1E-2F3A4B5C6D7E")]
internal class AllianceListRequestHandlerPlugIn : IPacketHandlerPlugIn
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

        var allianceGuilds = await serverContext.GuildServer.GetAllianceGuildsAsync(guildStatus.GuildId).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowAllianceListPlugIn>(p => p.ShowListAsync(allianceGuilds)).ConfigureAwait(false);
    }
}
