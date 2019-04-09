// <copyright file="PlayerShopRequestItemListPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler which handles requests for the item list of another player (3F 05).
    /// </summary>
    [PlugIn("Player Shop - Request Item List", "Packet handler which handles requests for the item list of another player (3F 05).")]
    [Guid("5A87AD36-1778-4CA4-AE08-B7BC12135C1B")]
    [BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
    internal class PlayerShopRequestItemListPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private readonly StoreItemListRequestAction requestListAction = new StoreItemListRequestAction();

        /// <inheritdoc/>
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => 0x05;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            ushort requestedId = packet.MakeWordSmallEndian(4);
            var requestedPlayer = player.CurrentMap.GetObject(requestedId) as Player;
            if (requestedPlayer == null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            string requestedPlayerName = packet.ExtractString(6, 10, Encoding.UTF8);

            if (requestedPlayerName != requestedPlayer.SelectedCharacter.Name)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Player Names don't match." + requestedPlayerName + "<>" + requestedPlayer.SelectedCharacter.Name, MessageType.BlueNormal);
                return;
            }

            this.requestListAction.RequestStoreItemList(player, requestedPlayer);
        }
    }
}