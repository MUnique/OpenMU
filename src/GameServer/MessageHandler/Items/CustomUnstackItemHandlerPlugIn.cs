// <copyright file="CustomUnstackItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.ComponentModel;
using System.Buffers.Binary;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles custom stack-split requests from the client.
/// </summary>
[PlugIn]
[Display(Name = "CustomUnstack", Description = "Splits a stack into a separate inventory slot.")]
[Guid("e5f6a7b8-c9d0-1e2f-3a4b-5c6d7e8f9a0b")]
internal sealed class CustomUnstackItemHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ItemStackAction _unstackAction = new();

    /// <inheritdoc/>
    public bool IsEncryptionExpected => false;

    /// <inheritdoc/>
    public byte Key => 0x3E;

    /// <summary>
    /// Handles the custom unstack item request.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="packet">The packet.</param>
    /// <returns>The task to run in parallel.</returns>
    public async ValueTask HandlePacketAsync(Player player, Memory<byte> packet)
    {
        if (packet.Length != 7)
        {
            return;
        }

        var sourceStorage = (ItemStorageKind)packet.Span[3];
        var sourceSlot = packet.Span[4];
        var splitCount = BinaryPrimitives.ReadUInt16LittleEndian(packet.Span[5..7]);
        if (sourceStorage != ItemStorageKind.Inventory || splitCount == 0)
        {
            return;
        }

        await this._unstackAction
            .SplitStackAsync(player, sourceSlot, splitCount)
            .ConfigureAwait(false);
    }
}
