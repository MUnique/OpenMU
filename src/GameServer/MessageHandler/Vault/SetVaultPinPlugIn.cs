// <copyright file="SetVaultPinPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Vault;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Vault;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for vault pin set packets (0x83, 0x01 identifier).
/// </summary>
[PlugIn("Vault Lock - Set Pin", "Packet handler for vault pin set packets (0x83, 0x01 identifier).")]
[Guid("A4C4CD69-6E28-4088-B533-CD63589D3CCA")]
[BelongsToGroup(VaultLockGroupPlugIn.GroupKey)]
internal class SetVaultPinPlugIn : ISubPacketHandlerPlugIn
{
    private readonly SetVaultPinAction _setVaultPinAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => SetVaultPin.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        SetVaultPin message = packet;
        await this._setVaultPinAction.SetPinAsync(player, message.Pin.ToString(), message.Password).ConfigureAwait(false);
    }
}