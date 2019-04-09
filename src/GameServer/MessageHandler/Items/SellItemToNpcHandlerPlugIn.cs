// <copyright file="SellItemToNpcHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for item sale to npc packets.
    /// </summary>
    [PlugIn("SellItemToNpcHandlerPlugIn", "Handler for item sale to npc packets.")]
    [Guid("8bbf8737-8731-4975-baa8-e14f77451b85")]
    internal class SellItemToNpcHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly SellItemToNpcAction sellAction = new SellItemToNpcAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.SellNPCItem;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            byte slot = packet[3];
            this.sellAction.SellItem(player, slot);
        }
    }
}
