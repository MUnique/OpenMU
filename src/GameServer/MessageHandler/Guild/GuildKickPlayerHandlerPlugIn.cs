// <copyright file="GuildKickPlayerHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Guild;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for guild player kick packets.
/// </summary>
[PlugIn("GuildKickPlayerHandlerPlugIn", "Handler for guild player kick packets.")]
[Guid("ddc7e221-c3a9-47c3-881e-dc59beecc03e")]
internal class GuildKickPlayerHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly GuildKickPlayerAction _kickAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => GuildKickPlayerRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player guildMaster, Memory<byte> packet)
    {
        GuildKickPlayerRequest request = packet;
        await this._kickAction.KickPlayerAsync(guildMaster, request.PlayerName, request.SecurityCode).ConfigureAwait(false);
    }
}