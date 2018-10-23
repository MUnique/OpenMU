// <copyright file="ChaosMixHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for chaos mix packets.
    /// </summary>
    internal class ChaosMixHandler : IPacketHandler
    {
        private readonly ItemCraftAction mixAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaosMixHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public ChaosMixHandler(IGameContext gameContext)
        {
            this.mixAction = new ItemCraftAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 4)
            {
                return;
            }

            if (packet[2] != 0x86)
            {
                return;
            }

            byte mixId = packet[2];
            this.mixAction.MixItems(player, mixId);
        }
    }
}
