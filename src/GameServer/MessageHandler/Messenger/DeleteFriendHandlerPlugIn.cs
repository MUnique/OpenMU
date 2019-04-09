// <copyright file="DeleteFriendHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for delete friend packets.
    /// </summary>
    [PlugIn("DeleteFriendHandlerPlugIn", "Handler for delete friend packets.")]
    [Guid("82d21573-64bd-439e-9368-8fc227475942")]
    internal class DeleteFriendHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly DeleteFriendAction deleteAction = new DeleteFriendAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendDelete;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // C1 0D C3 //10 bytes following
            if (packet.Length < 0x0D)
            {
                // Log?
                return;
            }

            var friendName = packet.ExtractString(3, 10, Encoding.UTF8);
            this.deleteAction.DeleteFriend(player, friendName);
        }
    }
}
