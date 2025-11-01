// <copyright file="CastleSiegeRegistrationStateRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for castle siege registration state request packets.
/// </summary>
[PlugIn(nameof(CastleSiegeRegistrationStateRequestHandlerPlugIn), "Handler for castle siege registration state request packets.")]
[Guid("A0B1C2D3-5678-89AB-CDEF-123456789ABC")]
internal class CastleSiegeRegistrationStateRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeRegistrationStateRequest.Code;

    /// <inheritdoc />
    public byte SubKey => CastleSiegeRegistrationStateRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        var isRegistered = false;
        var totalMarksSubmitted = 0;

        if (player.GuildStatus is { } guildStatus
            && player.GameContext is IGameServerContext serverContext)
        {
            var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);

            if (allianceMasterGuildId != 0)
            {
                var castleSiegeContext = player.CurrentMap?.CastleSiegeContext;
                var registration = castleSiegeContext?.GetRegistration(allianceMasterGuildId);
                isRegistered = registration != null;
                totalMarksSubmitted = registration?.GuildMarksSubmitted ?? 0;
            }
        }

        await player.InvokeViewPlugInAsync<IShowCastleSiegeRegistrationStatePlugIn>(
            p => p.ShowRegistrationStateAsync(isRegistered, totalMarksSubmitted)).ConfigureAwait(false);
    }
}
