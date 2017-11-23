// <copyright file="CloseNPCHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;

    /// <summary>
    /// Packet handler for close npc packets.
    /// </summary>
    internal class CloseNPCHandler : IPacketHandler
    {
        private readonly CloseNpcAction closeNpcAction = new CloseNpcAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            this.closeNpcAction.CloseNpcDialog(player);
        }
    }
}
