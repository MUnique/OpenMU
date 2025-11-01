// <copyright file="CashShopItemGiftRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop item gift request packets.
/// </summary>
[PlugIn(nameof(CashShopItemGiftRequestHandlerPlugIn), "Handler for cash shop item gift request packets.")]
[Guid("2B3C4D5E-6F7A-8B9C-0D1E-2F3A4B5C6D7E")]
internal class CashShopItemGiftRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopItemGiftRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopItemGiftRequest request = packet;

        var result = await player.TrySendCashShopGiftAsync(
            (int)request.ProductMainIndex,
            request.GiftReceiverName,
            request.GiftText,
            (int)request.CoinIndex).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IShowCashShopItemGiftResultPlugIn>(
            p => p.ShowCashShopItemGiftResultAsync(result, request.GiftReceiverName)).ConfigureAwait(false);
    }
}
