// <copyright file="JewelMixHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;

    /// <summary>
    /// Handler for jewel mix packets.
    /// </summary>
    internal class JewelMixHandler : IPacketHandler
    {
        private readonly ItemStackAction mixAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="JewelMixHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public JewelMixHandler(IGameContext gameContext)
        {
            this.mixAction = new ItemStackAction(gameContext);
        }

        private enum MixType
        {
            Mix,

            Unmix
        }

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (packet.Length < 6)
            {
                return;
            }

            /*C1 06 BC 00 00 01 [slot]
//               ^ Size, 0=10, 1=20, 2=30
//            ^0=Bless, 1=Soul ...
//         ^ 0=mix  1=unmix
*/
            byte stackSize = (byte)((packet[5] + 1) * 10);
            byte mixId = packet[4];
            var mixType = (MixType)packet[3];
            switch (mixType)
            {
                case MixType.Mix:
                    this.mixAction.StackItems(player, mixId, stackSize);
                    break;
                case MixType.Unmix:
                    byte slot = packet.Length > 6 ? packet[6] : (byte)0;
                    this.mixAction.UnstackItems(player, mixId, slot);
                    break;
                default:
                    throw new ArgumentException($"The mix type {mixType} is unknown.");
            }
        }
    }
}
