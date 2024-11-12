// <copyright file="VaultCloseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Vault;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for warehouse close packets.
/// </summary>
[PlugIn(nameof(VaultCloseHandlerPlugIn), "Handler for warehouse close packets.")]
[Guid("7859931f-3341-4bd7-91ad-1b0b03f11198")]
internal class VaultCloseHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly CloseNpcDialogAction _closeDialogAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => VaultClosed.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._closeDialogAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
    }
}