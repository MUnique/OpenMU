// <copyright file="LetterSendHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Messenger
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Messenger;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for letter send packets.
    /// </summary>
    [PlugIn("LetterSendHandlerPlugIn", "Handler for letter send packets.")]
    [Guid("6d10d34d-bd20-4dcf-99eb-569d38ef1c1b")]
    internal class LetterSendHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly LetterSendAction sendAction = new LetterSendAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.FriendMemoSend;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            var letterId = packet.MakeDwordBigEndian(4);
            if (packet.Length < 83)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Letter invalid.", MessageType.BlueNormal);
                player.ViewPlugIns.GetPlugIn<ILetterSendResultPlugIn>()?.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
                return;
            }

            var receiverName = packet.ExtractString(8, 10, Encoding.UTF8);
            var title = packet.ExtractString(18, 60, Encoding.UTF8);
            var message = packet.ExtractString(0x52, packet.Length - 0x52, Encoding.UTF8);
            var rotation = packet[0x4E];
            var animation = packet[0x4F];
            this.sendAction.SendLetter(player, receiverName, message, title, rotation, animation, letterId);
        }
    }
}
