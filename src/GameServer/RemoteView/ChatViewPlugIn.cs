// <copyright file="ChatViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the chat view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    [PlugIn("Chat View PlugIn", "View Plugin to send chat messages to the player")]
    [Guid("F0B5BAD4-B97C-49F1-84E0-25EDC796B0E4")]
    public class ChatViewPlugIn : IChatViewPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatViewPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ChatViewPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        /// <inheritdoc/>
        public void ChatMessage(string message, string sender, ChatMessageType type)
        {
            this.player.Connection?.SendChatMessage(ConvertChatMessageType(type), sender, message);
        }

        private static ChatMessage.ChatMessageType ConvertChatMessageType(ChatMessageType type)
        {
            if (type == ChatMessageType.Whisper)
            {
                return Network.Packets.ServerToClient.ChatMessage.ChatMessageType.Whisper;
            }

            return Network.Packets.ServerToClient.ChatMessage.ChatMessageType.Normal;
        }
    }
}
