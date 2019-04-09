// <copyright file="PickupItemHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Items
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Handler for item pickup packets.
    /// </summary>
    [PlugIn("PickupItemHandlerPlugIn", "Handler for item pickup packets.")]
    [Guid("8bcb9d85-95ae-4611-ae64-e9cc801ec647")]
    internal class PickupItemHandlerPlugIn : IPacketHandlerPlugIn
    {
        private readonly PickupItemAction pickupAction = new PickupItemAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.PickupItem;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // 0xc3, 5, 0x22, id1, id2
            ushort dropid = NumberConversionExtensions.MakeWord(packet[4], packet[3]);

            this.pickupAction.PickupItem(player, dropid);
        }
    }
}
