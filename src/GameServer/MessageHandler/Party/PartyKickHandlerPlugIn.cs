// <copyright file="PartyKickHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for party kick packets.
/// </summary>
[PlugIn("PartyKickHandlerPlugIn", "Handler for party kick packets.")]
[Guid("26d0fef9-8171-4098-87ae-030054163509")]
internal class PartyKickHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly PartyKickAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => (byte)PacketType.PartyKick;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PartyPlayerKickRequest message = packet;
        await this._action.KickPlayerAsync(player, message.PlayerIndex).ConfigureAwait(false);
    }
}