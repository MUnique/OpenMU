// <copyright file="DeleteFriendHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using System;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for delete friend packets.
    /// </summary>
    internal class DeleteFriendHandler : IPacketHandler
    {
        private readonly DeleteFriendAction deleteAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteFriendHandler"/> class.
        /// </summary>
        /// <param name="friendServer">The friend server.</param>
        public DeleteFriendHandler(IFriendServer friendServer)
        {
            this.deleteAction = new DeleteFriendAction(friendServer);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // C1 0D C3 //10 bytes following
            if (packet.Length < 0x0D)
            {
                // Log?
                return;
            }

            var friendName = packet.ExtractString(3, 10, Encoding.UTF8);
            this.deleteAction.DeleteFriend(player, friendName);
        }
    }
}
