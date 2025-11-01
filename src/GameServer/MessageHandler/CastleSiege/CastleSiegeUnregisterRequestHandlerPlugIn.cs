// <copyright file="CastleSiegeUnregisterRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for castle siege unregistration request packets.
/// </summary>
[PlugIn(nameof(CastleSiegeUnregisterRequestHandlerPlugIn), "Handler for castle siege unregistration request packets.")]
[Guid("B2C3D4E5-6789-01AB-CDEF-234567890ABC")]
internal class CastleSiegeUnregisterRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeRegistrationAction _registrationAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeUnregisterRequest.Code;

    /// <inheritdoc />
    public byte SubKey => CastleSiegeUnregisterRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._registrationAction.UnregisterFromSiegeAsync(player).ConfigureAwait(false);
    }
}
