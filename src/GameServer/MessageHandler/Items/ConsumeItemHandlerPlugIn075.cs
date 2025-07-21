// <copyright file="ConsumeItemHandlerPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for item consume packets.
/// </summary>
[PlugIn(nameof(ConsumeItemHandlerPlugIn075), "Handler for item consume packets of version 0.75")]
[Guid("04F05526-C88A-4E3A-A872-B9103524AD38")]
internal class ConsumeItemHandlerPlugIn075 : IPacketHandlerPlugIn
{
    private readonly ItemConsumeAction _consumeAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => ConsumeItemRequest075.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        ConsumeItemRequest075 message = packet;
        await this._consumeAction.HandleConsumeRequestAsync(player, message.ItemSlot, message.TargetSlot, FruitUsage.Undefined).ConfigureAwait(false);
    }
}