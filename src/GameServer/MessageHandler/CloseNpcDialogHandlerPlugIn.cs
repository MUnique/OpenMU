// <copyright file="CloseNpcDialogHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for close npc packets.
/// </summary>
[PlugIn("CloseNpcDialogHandlerPlugIn", "Packet handler for close npc packets.")]
[Guid("ecb920e3-eca7-4f40-a453-bdee67e1dabf")]
internal class CloseNpcDialogHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly CloseNpcDialogAction _closeNpcDialogAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CloseNpcRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._closeNpcDialogAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
    }
}