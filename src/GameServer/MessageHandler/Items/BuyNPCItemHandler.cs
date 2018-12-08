// <copyright file="BuyNPCItemHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for npc item buy requests.
    /// </summary>
    internal class BuyNPCItemHandler : IPacketHandler
    {
        private readonly BuyNpcItemAction buyAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyNPCItemHandler"/> class.
        /// </summary>
        public BuyNPCItemHandler()
        {
            this.buyAction = new BuyNpcItemAction();
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            byte slot = packet[3];

            this.buyAction.BuyItem(player, slot);
        }
    }
}
