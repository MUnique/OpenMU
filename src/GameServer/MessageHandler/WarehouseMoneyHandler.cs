// <copyright file="WarehouseCloseHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for warehouse money packets.
    /// </summary>
    internal class WarehouseMoneyHandler : IPacketHandler
    {
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

            player.PlayerView.UpdateVaultMoney();
        }
    }
}
