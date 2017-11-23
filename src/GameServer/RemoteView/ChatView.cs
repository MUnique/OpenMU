// <copyright file="ChatView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class ChatView : IChatView
    {
        private readonly IConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public ChatView(IConnection connection)
        {
            this.connection = connection;
        }

        /// <inheritdoc/>
        public void ChatMessage(string message, string sender, ChatMessageType type)
        {
            var packet = new byte[Encoding.UTF8.GetByteCount(message) + 14];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = this.GetChatMessageTypeByte(type);
            Encoding.UTF8.GetBytes(sender, 0, sender.Length, packet, 3);
            Encoding.UTF8.GetBytes(message, 0, message.Length, packet, 13);
            this.connection.Send(packet);
        }

        private byte GetChatMessageTypeByte(ChatMessageType type)
        {
            if (type == ChatMessageType.Whisper)
            {
                return 0x02;
            }

            return 0x00;
        }
    }
}
