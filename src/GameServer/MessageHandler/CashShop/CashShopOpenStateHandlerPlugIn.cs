// <copyright file="CashShopOpenStateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop open/close state packets.
/// </summary>
[PlugIn(nameof(CashShopOpenStateHandlerPlugIn), "Handler for cash shop open/close state packets.")]
[Guid("3D4E5F6A-7B8C-9D0E-1F2A-3B4C5D6E7F8A")]
internal class CashShopOpenStateHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopOpenState.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopOpenState request = packet;
        bool isOpen = !request.IsClosed;

        await player.InvokeViewPlugInAsync<IShowCashShopOpenStatePlugIn>(p => p.ShowCashShopOpenStateAsync(isOpen)).ConfigureAwait(false);
    }
}
