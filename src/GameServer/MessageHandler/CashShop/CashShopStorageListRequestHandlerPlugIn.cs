// <copyright file="CashShopStorageListRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop storage list request packets.
/// </summary>
[PlugIn(nameof(CashShopStorageListRequestHandlerPlugIn), "Handler for cash shop storage list request packets.")]
[Guid("5E6F7A8B-9C0D-1E2F-3A4B-5C6D7E8F9A0B")]
internal class CashShopStorageListRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopStorageListRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await player.InvokeViewPlugInAsync<IShowCashShopStorageListPlugIn>(p => p.ShowCashShopStorageListAsync()).ConfigureAwait(false);
    }
}
