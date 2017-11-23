// <copyright file="ChangeOnlineStateHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Friends
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Messenger;

    /// <summary>
    /// Handler for online state change packets.
    /// </summary>
    internal class ChangeOnlineStateHandler : IPacketHandler
    {
        private readonly ChangeOnlineStateAction changeAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeOnlineStateHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChangeOnlineStateHandler(IGameServerContext gameContext)
        {
            this.changeAction = new ChangeOnlineStateAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            if (packet.Length < 4)
            {
                // log
                return;
            }

            this.changeAction.SetOnlineState(player, packet[3] == 1);
        }
    }
}
