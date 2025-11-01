// <copyright file="RemoveAllianceGuildHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for remove alliance guild request packets.
/// </summary>
[PlugIn(nameof(RemoveAllianceGuildHandlerPlugIn), "Handler for remove alliance guild request packets.")]
[Guid("1A2B3C4D-5E6F-7890-9012-1234567890AB")]
internal class RemoveAllianceGuildHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly AllianceKickAction _allianceKickAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RemoveAllianceGuildRequest.Code;

    /// <inheritdoc />
    public byte SubKey => RemoveAllianceGuildRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        RemoveAllianceGuildRequest request = packet;
        await this._allianceKickAction.KickGuildFromAllianceAsync(player, request.GuildName).ConfigureAwait(false);
    }
}
