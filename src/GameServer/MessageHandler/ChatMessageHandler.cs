// <copyright file="ChatMessageHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network;

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
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ////byte 3-12 char name
            string characterName = packet.ExtractString(3, 10, Encoding.UTF8);
            ////byte 13-n message
            string message = packet.ExtractString(13, packet.Length - 13, Encoding.UTF8);

            this.messageAction.ChatMessage(player, characterName, message, packet[2] == 0x02);
        }
    }
}
