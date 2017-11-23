// <copyright file="ChatRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for chat request packets.
    /// </summary>
    internal class ChatRequestHandler : IPacketHandler
    {
        private readonly ChatRequestAction chatRequestAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRequestHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChatRequestHandler(IGameServerContext gameContext)
        {
            this.chatRequestAction = new ChatRequestAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            // C1 0D CA //10 bytes following
            if (packet.Length < 0x0D)
            {
                // Log?
                return;
            }

            var friendName = packet.ExtractString(3, 10, Encoding.UTF8);
            this.chatRequestAction.RequestChat(player, friendName);
        }
    }
}
