// <copyright file="PickupItemHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for item pickup packets.
    /// </summary>
    internal class PickupItemHandler : IPacketHandler
    {
        private readonly PickupItemAction pickupAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupItemHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public PickupItemHandler(IGameContext gameContext)
        {
            this.pickupAction = new PickupItemAction(gameContext);
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // 0xc3, 5, 0x22, id1, id2
            ushort dropid = NumberConversionExtensions.MakeWord(packet[4], packet[3]);

            this.pickupAction.PickupItem(player, dropid);
        }
    }
}
