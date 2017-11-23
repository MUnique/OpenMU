// <copyright file="ChatMessageHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;

    /// <summary>
    /// Packet handler for chat messages.
    /// </summary>
    internal class ChatMessageHandler : IPacketHandler
    {
        private readonly ChatMessageAction messageAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessageHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChatMessageHandler(IGameContext gameContext)
        {
            this.messageAction = new ChatMessageAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            ////byte 3-12 char name
            string characterName = Encoding.UTF8.GetString(packet, 3, packet.Skip(3).TakeWhile(b => b != 0).Count());
            ////byte 13-n message
            string message = Encoding.UTF8.GetString(packet, 13, packet.Skip(13).TakeWhile(b => b != 0).Count());

            this.messageAction.ChatMessage(player, characterName, message, packet[2] == 0x02);
        }
    }
}
