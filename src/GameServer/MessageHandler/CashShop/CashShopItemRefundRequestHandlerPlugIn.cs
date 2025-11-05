// <copyright file="CashShopItemRefundRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop item refund request packets.
/// </summary>
[PlugIn(nameof(CashShopItemRefundRequestHandlerPlugIn), "Handler for cash shop item refund request packets.")]
[Guid("B2C3D4E5-F6A7-8901-2B3C-4D5E6F7A8901")]
internal class CashShopItemRefundRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopItemRefundRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopItemRefundRequest request = packet;

        var result = await player.TryRefundCashShopItemAsync(request.ItemSlot).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IShowCashShopItemRefundResultPlugIn>(
            p => p.ShowCashShopItemRefundResultAsync(result)).ConfigureAwait(false);
    }
}
