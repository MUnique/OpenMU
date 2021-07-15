// <copyright file="PlayerShopClosePacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler which opens the player shop (3F 03).
    /// </summary>
    [PlugIn("Player Shop - Close", "Packet handler which opens the player shop (3F 03).")]
    [Guid("3DD40F3A-37FA-4694-80FB-BF71B454D98E")]
    [BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
    internal class PlayerShopClosePacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly CloseStoreAction closeStoreAction = new ();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => PlayerShopClose.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            this.closeStoreAction.CloseStore(player);
        }
    }
}