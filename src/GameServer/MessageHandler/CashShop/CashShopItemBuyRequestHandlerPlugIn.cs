// <copyright file="CashShopItemBuyRequestHandlerPlugIn.cs" company="MUnique">
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
/// Handler for cash shop item buy request packets.
/// </summary>
[PlugIn(nameof(CashShopItemBuyRequestHandlerPlugIn), "Handler for cash shop item buy request packets.")]
[Guid("1A2B3C4D-5E6F-7A8B-9C0D-1E2F3A4B5C6D")]
internal class CashShopItemBuyRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopItemBuyRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopItemBuyRequest request = packet;

        var result = await player.TryBuyCashShopItemAsync(
            (int)request.ProductMainIndex,
            (int)request.CoinIndex).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IShowCashShopItemBuyResultPlugIn>(
            p => p.ShowCashShopItemBuyResultAsync(result, (int)request.ProductMainIndex)).ConfigureAwait(false);
    }
}
