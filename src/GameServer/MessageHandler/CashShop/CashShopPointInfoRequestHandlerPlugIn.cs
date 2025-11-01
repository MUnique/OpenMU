// <copyright file="CashShopPointInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop point information request packets.
/// </summary>
[PlugIn(nameof(CashShopPointInfoRequestHandlerPlugIn), "Handler for cash shop point information request packets.")]
[Guid("9B1E2A3F-4C5D-6E7F-8A9B-0C1D2E3F4A5B")]
internal class CashShopPointInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopPointInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<IShowCashShopPointsPlugIn>(p => p.ShowCashShopPointsAsync()).ConfigureAwait(false);
    }
}
