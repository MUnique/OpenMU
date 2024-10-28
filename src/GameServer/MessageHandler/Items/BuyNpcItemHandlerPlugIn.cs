// <copyright file="BuyNpcItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for npc item buy requests.
/// </summary>
[PlugIn("BuyNpcItemHandlerPlugIn", "Handler for npc item buy requests.")]
[Guid("7c7a0944-341b-4cdf-a9b2-010c0c95fa41")]
internal class BuyNpcItemHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly BuyNpcItemAction _buyAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => BuyItemFromNpcRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        BuyItemFromNpcRequest message = packet;
        await this._buyAction.BuyItemAsync(player, message.ItemSlot).ConfigureAwait(false);
    }
}