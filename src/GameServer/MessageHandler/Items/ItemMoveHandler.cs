// <copyright file="ItemMoveHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for item move packets.
    /// </summary>
    internal class ItemMoveHandler : IPacketHandler
    {
        private readonly MoveItemAction moveAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMoveHandler"/> class.
        /// </summary>
        public ItemMoveHandler()
        {
            this.moveAction = new MoveItemAction();
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            byte fromID = packet[3];
            byte fromSlot = packet[4];
            byte toID = packet[17];
            byte toSlot = packet[18];

            this.moveAction.MoveItem(player, fromSlot, (Storages)fromID, toSlot, (Storages)toID);
        }
    }
}
