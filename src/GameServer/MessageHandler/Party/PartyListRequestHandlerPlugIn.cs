// <copyright file="PartyListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for party list request packets.
/// </summary>
[PlugIn("PartyListRequestHandlerPlugIn", "Handler for party list request packets.")]
[Guid("2650e346-69ef-4a9e-82ba-5f0b9591a548")]
internal class PartyListRequestHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly PartyListRequestAction _action = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => PartyListRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._action.RequestPartyListAsync(player).ConfigureAwait(false);
    }
}