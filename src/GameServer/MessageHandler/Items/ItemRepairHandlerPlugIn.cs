﻿// <copyright file="ItemRepairHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handler for item repair packets.
/// </summary>
[PlugIn("ItemRepairHandlerPlugIn", "Handler for item repair packets.")]
[Guid("85b4a195-c90c-47f8-bae2-833b5d2ef398")]
internal class ItemRepairHandlerPlugIn : IPacketHandlerPlugIn
{
    private readonly ItemRepairAction _repairAction = new ();

    /// <inheritdoc/>
    public bool IsEncryptionExpected { get; } = RepairItemRequest.HeaderType >= 0xC3;

    /// <inheritdoc/>
    public byte Key => RepairItemRequest.Code;

    /// <inheritdoc/>
    public void HandlePacket(Player player, Span<byte> packet)
    {
        RepairItemRequest message = packet;
        if (message.ItemSlot == 0xFF)
        {
            this._repairAction.RepairAllItems(player);
        }
        else
        {
            this._repairAction.RepairItem(player, message.ItemSlot);
        }
    }
}