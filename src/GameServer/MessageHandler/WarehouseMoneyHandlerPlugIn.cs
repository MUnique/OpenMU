// <copyright file="WarehouseMoneyHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Network;
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
        public byte Key => (byte)PacketType.VaultMoneyInOut;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            int zen = (int)packet.MakeDwordBigEndian(4);

            if (packet[3] == 0)
            {
                player.TryDepositVaultMoney(zen);
            }
            else
            {
                player.TryTakeVaultMoney(zen);
            }

            player.ViewPlugIns.GetPlugIn<IUpdateVaultMoneyPlugIn>()?.UpdateVaultMoney();
        }
    }
}
