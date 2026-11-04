// <copyright file="CashShopOpenStateHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.CashShop;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.CashShop;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Packet handler for opening and closing the cash shop (0xD2, 0x02 identifier).
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CashShopOpenStateHandlerPlugIn_Name), Description = nameof(PlugInResources.CashShopOpenStateHandlerPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("A2F0468D-EE44-4EE4-AD47-19A4792F65DB")]
[BelongsToGroup(CashShopGroupHandlerPlugIn.GroupKey)]
internal class CashShopOpenStateHandlerPlugIn : ISubPacketHandlerPlugIn
{
    private readonly CashShopActions _cashShopActions = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => CashShopOpenState.SubCode;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        CashShopOpenState message = packet;
        if (message.IsClosed)
        {
            this._cashShopActions.Close(player);
        }
        else
        {
            await this._cashShopActions.OpenAsync(player).ConfigureAwait(false);
        }
    }
}
