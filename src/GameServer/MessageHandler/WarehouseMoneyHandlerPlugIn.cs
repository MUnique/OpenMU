// <copyright file="WarehouseMoneyHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for warehouse money packets.
    /// </summary>
    [PlugIn("WarehouseMoneyHandlerPlugIn", "Handler for warehouse money packets.")]
    [Guid("e365f3f2-55c8-4890-9f6b-26fd39822b71")]
    internal class WarehouseMoneyHandlerPlugIn : IPacketHandlerPlugIn
    {
        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => VaultMoveMoneyRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            VaultMoveMoneyRequest request = packet;
            switch (request.Direction)
            {
                case VaultMoveMoneyRequest.VaultMoneyMoveDirection.InventoryToVault:
                    player.TryDepositVaultMoney((int)request.Amount);
                    break;
                case VaultMoveMoneyRequest.VaultMoneyMoveDirection.VaultToInventory:
                    player.TryTakeVaultMoney((int)request.Amount);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"The direction {request.Direction} is not a valid value.");
            }

            player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()?.UpdateVaultMoney(true);
        }
    }
}
