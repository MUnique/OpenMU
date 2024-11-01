// <copyright file="DropItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for drop item packets.
/// </summary>
[PlugIn("DropItemHandlerPlugIn", "Handler for drop item packets.")]
[Guid("b79bc453-74a0-4eea-8bc3-014d737aaa88")]
internal class DropItemHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly DropItemAction _dropAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => DropItemRequest.Code;

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        DropItemRequest message = packet;
        await this._dropAction.DropItemAsync(player, message.ItemSlot, new Point(message.TargetX, message.TargetY)).ConfigureAwait(false);
    }
}