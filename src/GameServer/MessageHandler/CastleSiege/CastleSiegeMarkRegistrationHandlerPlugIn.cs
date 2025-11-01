// <copyright file="CastleSiegeMarkRegistrationHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CastleSiege;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for castle siege mark (guild mark item) registration packets.
/// </summary>
[PlugIn(nameof(CastleSiegeMarkRegistrationHandlerPlugIn), "Handler for castle siege mark registration packets.")]
[Guid("C3D4E5F6-7890-12AB-CDEF-345678901ABC")]
internal class CastleSiegeMarkRegistrationHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CastleSiegeRegistrationAction _registrationAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CastleSiegeMarkRegistration.Code;

    /// <inheritdoc />
    public byte SubKey => CastleSiegeMarkRegistration.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CastleSiegeMarkRegistration request = packet;
        await this._registrationAction.SubmitGuildMarkAsync(player, request.ItemIndex).ConfigureAwait(false);
    }
}
