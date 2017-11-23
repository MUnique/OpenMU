// <copyright file="SellItemToNPCHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for item sale to npc packets.
    /// </summary>
    internal class SellItemToNPCHandler : IPacketHandler
    {
        private SellItemToNpcAction sellAction = new SellItemToNpcAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            byte slot = packet[3];
            this.sellAction.SellItem(player, slot);
        }
    }
}
