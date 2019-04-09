// <copyright file="ChatRequestHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for chat request packets.
    /// </summary>
    [PlugIn("ChatRequestHandlerPlugIn", "Handler for chat request packets.")]
    [Guid("acf9263f-ba71-4d84-b8f8-84e494eb4462")]
    internal class ChatRequestHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ChatRequestAction chatRequestAction = new ChatRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.ChatRoomCreate;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
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
