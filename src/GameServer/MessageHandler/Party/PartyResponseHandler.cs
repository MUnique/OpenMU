// <copyright file="PartyResponseHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Party
{
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;

    /// <summary>
    /// Handler for party response packets.
    /// </summary>
    internal class PartyResponseHandler : IPacketHandler
    {
        private readonly PartyResponseAction action;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyResponseHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public PartyResponseHandler(IGameContext gameContext)
        {
            this.action = new PartyResponseAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, byte[] packet)
        {
            this.action.HandleResponse(player, packet[3] != 0);
        }
    }
}
