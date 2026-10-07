// <copyright file="CashShopItemGiftRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for cash shop gift requests (0xD2, 0x04 identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopItemGiftRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.CashShopItemGiftRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("7C363101-6708-4787-9762-9FECF78D7710")]
[BelongsToGroup(CashShopGroupHandlerPlugIn.GroupKey)]
internal class CashShopItemGiftRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CashShopActions _cashShopActions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CashShopItemGiftRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopItemGiftRequest message = packet;
        await this._cashShopActions.GiftAsync(
            player,
            (int)message.PackageMainIndex,
            (int)message.ProductMainIndex,
            message.CoinIndex.ToCashShopCoinType(),
            message.GiftReceiverName,
            message.GiftText).ConfigureAwait(false);
    }
}
