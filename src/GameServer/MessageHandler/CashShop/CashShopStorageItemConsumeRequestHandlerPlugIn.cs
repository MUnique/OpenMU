// <copyright file="CashShopStorageItemConsumeRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for requests to use an item of the cash shop storage (0xD2, 0x0B identifier).
/// </summary>
/// <remarks>
/// The client sends back the storage index as base item code and the item sequence, which is
/// the price sequence number, as main item code. See the cash shop view plugin.
/// </remarks>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopStorageItemConsumeRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.CashShopStorageItemConsumeRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("2D189ACC-EA7D-4816-83CD-9B5C04E45D67")]
[BelongsToGroup(CashShopGroupHandlerPlugIn.GroupKey)]
internal class CashShopStorageItemConsumeRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CashShopActions _cashShopActions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CashShopStorageItemConsumeRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopStorageItemConsumeRequest message = packet;
        await this._cashShopActions.UseStorageItemAsync(player, (int)message.BaseItemCode, (int)message.MainItemCode).ConfigureAwait(false);
    }
}
