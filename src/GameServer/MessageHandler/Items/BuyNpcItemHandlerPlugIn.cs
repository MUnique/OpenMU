// <copyright file="BuyNpcItemHandlerPlugIn.cs" company="MUnique">
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
    /// Handler for npc item buy requests.
    /// </summary>
    [PlugIn("BuyNpcItemHandlerPlugIn", "Handler for npc item buy requests.")]
    [Guid("7c7a0944-341b-4cdf-a9b2-010c0c95fa41")]
    internal class BuyNpcItemHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly BuyNpcItemAction buyAction = new BuyNpcItemAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => false;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.BuyNPCItem;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            byte slot = packet[3];

            this.buyAction.BuyItem(player, slot);
        }
    }
}
