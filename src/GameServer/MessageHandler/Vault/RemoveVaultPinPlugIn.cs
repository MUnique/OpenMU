// <copyright file="RemoveVaultPinPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Vault;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Vault;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for vault pin remove packets (0x83, 0x02 identifier).
/// </summary>
[PlugIn("Vault Lock - Remove Pin", "Packet handler for vault pin remove packets (0x83, 0x02 identifier).")]
[Guid("55ED4BC4-516F-490E-A065-B0150F2BD939")]
[BelongsToGroup(VaultLockGroupPlugIn.GroupKey)]
internal class RemoveVaultPinPlugIn : ISubPacketHandlerPlugIn
{
    private readonly RemoveVaultPinAction _unlockVaultAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => RemoveVaultPin.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        RemoveVaultPin message = packet;
        await this._unlockVaultAction.RemovePinAsync(player, message.Password).ConfigureAwait(false);
    }
}