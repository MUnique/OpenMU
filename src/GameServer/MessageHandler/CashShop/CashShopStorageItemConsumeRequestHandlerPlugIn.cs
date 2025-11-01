// <copyright file="CashShopStorageItemConsumeRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop storage item consume request packets.
/// </summary>
[PlugIn(nameof(CashShopStorageItemConsumeRequestHandlerPlugIn), "Handler for cash shop storage item consume request packets.")]
[Guid("4D5E6F7A-8B9C-0D1E-2F3A-4B5C6D7E8F9A")]
internal class CashShopStorageItemConsumeRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopStorageItemConsumeRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopStorageItemConsumeRequest request = packet;

        var success = await player.TryConsumeCashShopStorageItemAsync((byte)request.ItemIndex).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IShowCashShopItemConsumeResultPlugIn>(
            p => p.ShowCashShopItemConsumeResultAsync(success, (byte)request.ItemIndex)).ConfigureAwait(false);
    }
}
