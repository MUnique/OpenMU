// <copyright file="ChatMessageBaseHandlerPlugIn.cs" company="MUnique">
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
    /// Base class for a chat message handler.
    /// </summary>
    internal abstract class ChatMessageBaseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ChatMessageAction messageAction = new ChatMessageAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public abstract byte Key { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is handling whisper messages.
        /// </summary>
        protected abstract bool IsWhisper { get; }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ////byte 3-12 char name
            string characterName = packet.ExtractString(3, 10, Encoding.UTF8);
            ////byte 13-n message
            string message = packet.ExtractString(13, packet.Length - 13, Encoding.UTF8);

            this.messageAction.ChatMessage(player, characterName, message, this.IsWhisper);
        }
    }
}
