// <copyright file="ChaosMachineCloseHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using GameLogic;
    using GameLogic.PlayerActions;

    /// <summary>
    /// Handler for chaos mix packets.
    /// </summary>
    internal class ChaosMachineCloseHandler : IPacketHandler
    {
        private readonly CloseNpcDialogAction closeNpcDialogAction = new CloseNpcDialogAction();

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.closeNpcDialogAction.CloseNpcDialog(player);
        }
    }
}
