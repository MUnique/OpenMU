// <copyright file="WarehouseCloseHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;

    /// <summary>
    /// Handler for warehouse close packets.
    /// </summary>
    internal class WarehouseCloseHandler : IPacketHandler
    {
        private readonly CloseNpcAction closeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarehouseCloseHandler"/> class.
        /// </summary>
        public WarehouseCloseHandler()
        {
            this.closeAction = new CloseNpcAction();
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            this.closeAction.CloseNpcDialog(player);
        }
    }
}
