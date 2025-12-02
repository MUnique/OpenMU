// <copyright file="UnlockVaultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Vault;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Vault;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for vault unlock packets (0x83, 0x00 identifier).
/// </summary>
[PlugIn("Vault Lock - Unlock", "Packet handler for vault unlock packets (0x83, 0x00 identifier).")]
[Guid("B035454B-8858-4F30-94CC-EFAAF868FEFE")]
[BelongsToGroup(VaultLockGroupPlugIn.GroupKey)]
internal class UnlockVaultPlugIn : ISubPacketHandlerPlugIn
{
    private readonly UnlockVaultAction _unlockVaultAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => UnlockVault.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        string pin;

        void ExtractValues()
        {
            UnlockVaultRef message = packet.Span;
            pin = message.Pin.ToString();
        }

        ExtractValues();
        await this._unlockVaultAction.UnlockVaultAsync(player, pin).ConfigureAwait(false);
    }
}