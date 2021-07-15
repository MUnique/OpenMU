// <copyright file="JewelMixHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for jewel mix packets.
    /// </summary>
    [PlugIn("JewelMixHandlerPlugIn", "Handler for jewel mix packets.")]
    [Guid("d6067475-a910-488d-8450-9310ae394c47")]
    internal class JewelMixHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly ItemStackAction mixAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => LahapJewelMixRequest.HeaderType >= 0xC3;

        /// <inheritdoc/>
        public byte Key => LahapJewelMixRequest.Code;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            LahapJewelMixRequest message = packet;
            if (packet.Length < 6)
            {
                return;
            }

            switch (message.Operation)
            {
                case LahapJewelMixRequest.MixType.Mix:
                    this.mixAction.StackItems(player, (byte)message.Item, GetStackSize(message.MixingStackSize));
                    break;
                case LahapJewelMixRequest.MixType.Unmix:
                    this.mixAction.UnstackItems(player, (byte)message.Item, message.UnmixingSourceSlot);
                    break;
                default:
                    throw new ArgumentException($"The mix operation {message.Operation} is unknown.");
            }
        }

        private static byte GetStackSize(LahapJewelMixRequest.StackSize stackSize)
        {
            return stackSize switch
            {
                LahapJewelMixRequest.StackSize.Ten => 10,
                LahapJewelMixRequest.StackSize.Twenty => 20,
                LahapJewelMixRequest.StackSize.Thirty => 30,
                _ => throw new InvalidEnumArgumentException(nameof(stackSize), (int)stackSize, typeof(LahapJewelMixRequest.StackSize))
            };
        }
    }
}
