// <copyright file="CastleSiegeStatusRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for castle siege status request packets.
/// </summary>
[PlugIn(nameof(CastleSiegeStatusRequestHandlerPlugIn), "Handler for castle siege status request packets.")]
[Guid("90A1B2C3-4567-78AB-CDEF-012345678ABC")]
internal class CastleSiegeStatusRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeStatusRequest.Code;

    /// <inheritdoc />
    public byte SubKey => CastleSiegeStatusRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var castleSiegeContext = player.CurrentMap?.CastleSiegeContext;
        var ownerGuildName = "None";
        var siegeStatus = castleSiegeContext?.State.ToString() ?? "Inactive";

        if (castleSiegeContext?.OwnerAlliance is { } ownerAlliance
            && player.GameContext is IGameServerContext serverContext)
        {
            var ownerGuild = await serverContext.GuildServer.GetGuildAsync(ownerAlliance.AllianceMasterGuildId).ConfigureAwait(false);
            ownerGuildName = ownerGuild?.Name ?? "None";
        }

        await player.InvokeViewPlugInAsync<IShowCastleSiegeStatusPlugIn>(
            p => p.ShowStatusAsync(ownerGuildName, siegeStatus)).ConfigureAwait(false);
    }
}
