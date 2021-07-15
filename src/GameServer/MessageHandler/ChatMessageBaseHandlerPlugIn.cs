// <copyright file="ChatMessageBaseHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions;
    using MUnique.OpenMU.Network.Packets.ClientToServer;

    /// <summary>
    /// Base class for a chat message handler.
    /// </summary>
    internal abstract class ChatMessageBaseHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ChatMessageAction messageAction = new ();

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
            WhisperMessage message = packet;
            this.messageAction.ChatMessage(player, message.ReceiverName, message.Message, this.IsWhisper);
        }
    }
}
