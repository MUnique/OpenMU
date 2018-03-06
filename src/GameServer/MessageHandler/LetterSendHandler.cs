// <copyright file="LetterSendHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System.Linq;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for letter send packets.
    /// </summary>
    internal class LetterSendHandler : IPacketHandler
    {
        private readonly LetterSendAction sendAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LetterSendHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        /// <param name="friendServer">The friend server.</param>
        public LetterSendHandler(IGameContext gameContext, IFriendServer friendServer)
        {
            this.sendAction = new LetterSendAction(gameContext, friendServer);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            var letterId = packet.MakeDwordBigEndian(4);
            if (packet.Length < 83)
            {
                player.PlayerView.ShowMessage("Letter invalid.", MessageType.BlueNormal);
                player.PlayerView.MessengerView.LetterSendResult(LetterSendSuccess.TryAgain, letterId);
                return;
            }

            var receiverName = Encoding.UTF8.GetString(packet, 8, packet.Skip(8).Take(10).TakeWhile(b => b != 0).Count());
            var title = Encoding.UTF8.GetString(packet, 18, packet.Skip(18).Take(60).TakeWhile(b => b != 0).Count());
            var message = Encoding.UTF8.GetString(packet, 0x52, packet.Skip(0x52).TakeWhile(b => b != 0).Count());
            var rotation = packet[0x4E];
            var animation = packet[0x4F];
            this.sendAction.SendLetter(player, receiverName, message, title, rotation, animation, letterId);
        }
    }
}
