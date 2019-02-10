// <copyright file="FriendAddResponseHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using System;
    using System.Text;
    using GameLogic.Interfaces;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for friend add response packets.
    /// </summary>
    internal class FriendAddResponseHandler : IPacketHandler
    {
        private readonly AddResponseAction responseAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="FriendAddResponseHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public FriendAddResponseHandler(IGameServerContext gameContext)
        {
            this.responseAction = new AddResponseAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 0x0E)
            {
                // log
                return;
            }

            var requesterName = packet.ExtractString(4, 10, Encoding.UTF8);
            var accepted = packet[3] == 1;
            this.responseAction.ProceedReponse(player, requesterName, accepted);
        }
    }
}
