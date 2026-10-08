// <copyright file="CashShopPointInfoRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for cash shop point information requests (0xD2, 0x01 identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopPointInfoRequestHandlerPlugIn_Name), Description = nameof(PlugInResources.CashShopPointInfoRequestHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B287730A-7743-43EE-82CD-9EC468BF65F2")]
[BelongsToGroup(CashShopGroupHandlerPlugIn.GroupKey)]
internal class CashShopPointInfoRequestHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CashShopActions _cashShopActions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CashShopPointInfoRequest.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        await this._cashShopActions.ShowPointsAsync(player).ConfigureAwait(false);
    }
}
