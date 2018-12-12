// <copyright file="CloseNpcDialogHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;

    /// <summary>
    /// Packet handler for close npc packets.
    /// </summary>
    internal class CloseNpcDialogHandler : IPacketHandler
    {
        private readonly CloseNpcDialogAction closeNpcDialogAction = new CloseNpcDialogAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.closeNpcDialogAction.CloseNpcDialog(player);
        }
    }
}
