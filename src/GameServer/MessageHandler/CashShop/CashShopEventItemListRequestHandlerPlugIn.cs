// <copyright file="CashShopEventItemListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop event item list request packets.
/// </summary>
[PlugIn(nameof(CashShopEventItemListRequestHandlerPlugIn), "Handler for cash shop event item list request packets.")]
[Guid("7F8A9B0C-1D2E-3F4A-5B6C-7D8E9F0A1B2C")]
internal class CashShopEventItemListRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopEventItemListRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<IShowCashShopEventItemListPlugIn>(p => p.ShowCashShopEventItemListAsync()).ConfigureAwait(false);
    }
}
