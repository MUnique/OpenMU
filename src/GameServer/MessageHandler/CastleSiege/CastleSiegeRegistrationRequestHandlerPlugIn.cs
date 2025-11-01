// <copyright file="CastleSiegeRegistrationRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for castle siege registration request packets.
/// </summary>
[PlugIn(nameof(CastleSiegeRegistrationRequestHandlerPlugIn), "Handler for castle siege registration request packets.")]
[Guid("A1B2C3D4-5678-90AB-CDEF-123456789ABC")]
internal class CastleSiegeRegistrationRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeRegistrationAction _registrationAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeRegistrationRequest.Code;

    /// <inheritdoc />
    public byte SubKey => CastleSiegeRegistrationRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._registrationAction.RegisterForSiegeAsync(player).ConfigureAwait(false);
    }
}
