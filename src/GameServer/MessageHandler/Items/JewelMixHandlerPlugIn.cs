// <copyright file="JewelMixHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for jewel mix packets.
    /// </summary>
    [PlugIn("JewelMixHandlerPlugIn", "Handler for jewel mix packets.")]
    [Guid("d6067475-a910-488d-8450-9310ae394c47")]
    internal class JewelMixHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ItemStackAction mixAction = new ItemStackAction();

        private enum MixType
        {
            Mix,

            Unmix,
        }

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.JewelMix;

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
