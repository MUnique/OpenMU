// <copyright file="PartyResponseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for party response packets.
/// </summary>
[PlugIn("PartyResponseHandlerPlugIn", "Handler for party response packets.")]
[Guid("bd1e7c33-a80e-439f-b8e2-b2c22a68126b")]
internal class PartyResponseHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly PartyResponseAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => PartyInviteResponse.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        PartyInviteResponse message = packet;
        await this._action.HandleResponseAsync(player, message.Accepted).ConfigureAwait(false);
    }
}