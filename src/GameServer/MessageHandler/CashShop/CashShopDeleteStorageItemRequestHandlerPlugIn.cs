// <copyright file="CashShopDeleteStorageItemRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for cash shop delete storage item request packets.
/// </summary>
[PlugIn(nameof(CashShopDeleteStorageItemRequestHandlerPlugIn), "Handler for cash shop delete storage item request packets.")]
[Guid("3C4D5E6F-7A8B-9C0D-1E2F-3A4B5C6D7E8F")]
internal class CashShopDeleteStorageItemRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    /// <inheritdoc/>
    public bool IsEncryptionExpected => true;

    /// <inheritdoc/>
    public byte Key => CashShopDeleteStorageItemRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopDeleteStorageItemRequest request = packet;

        // Find the item in storage by matching BaseItemCode (Group) and MainItemCode (Number)
        var item = player.SelectedCharacter?.CashShopStorage?.Items
            .FirstOrDefault(i => i.Definition?.Group == (byte)request.BaseItemCode
                              && i.Definition?.Number == (short)request.MainItemCode);

        if (item is null)
        {
            await player.InvokeViewPlugInAsync<IShowCashShopItemDeleteResultPlugIn>(
                p => p.ShowCashShopItemDeleteResultAsync(false, 0)).ConfigureAwait(false);
            return;
        }

        var success = await player.TryDeleteCashShopStorageItemAsync(item.ItemSlot).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IShowCashShopItemDeleteResultPlugIn>(
            p => p.ShowCashShopItemDeleteResultAsync(success, item.ItemSlot)).ConfigureAwait(false);
    }
}
