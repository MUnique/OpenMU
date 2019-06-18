// <copyright file="PickupItemHandlerPlugInBase.cs" company="MUnique">
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
    internal abstract class PickupItemHandlerPlugInBase : IPacketHandlerPlugIn
    {
        private readonly PickupItemAction pickupAction = new PickupItemAction();

        /// <inheritdoc/>
        public abstract bool IsEncryptionExpected { get; }

        /// <inheritdoc/>
        public byte Key => (byte)PacketType.PickupItem;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            // 0xc3, 5, 0x22, id1, id2
            this.pickupAction.PickupItem(player, packet.MakeWordSmallEndian(3));
        }
    }
}