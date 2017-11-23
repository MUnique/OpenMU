// <copyright file="AddFriendHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for add friend packets.
    /// </summary>
    internal class AddFriendHandler : IPacketHandler
    {
        private readonly AddFriendAction addAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFriendHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public AddFriendHandler(IGameServerContext gameContext)
        {
            this.addAction = new AddFriendAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            // C1 0D C1 //10 bytes following
            if (packet.Length < 0x0D)
            {
                // Log?
                return;
            }

            var friendname = packet.ExtractString(3, 10, Encoding.UTF8);
            this.addAction.AddFriend(player, friendname);
        }
    }
}
