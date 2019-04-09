// <copyright file="AddFriendHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for add friend packets.
    /// </summary>
    [PlugIn("AddFriendHandlerPlugIn", "Handler for add friend packets.")]
    [Guid("302870db-59cc-4cf8-b5ed-b0efa9f6ccbc")]
    internal class AddFriendHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly AddFriendAction addAction = new AddFriendAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendAdd;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // C1 0D C1 //10 bytes following
            if (packet.Length < 0x0D)
            {
                // Log?
                return;
            }

            var friendName = packet.ExtractString(3, 10, Encoding.UTF8);
            this.addAction.AddFriend(player, friendName);
        }
    }
}
