// <copyright file="PlayerShopBuyRequestPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.Network.Packets.ClientToServer;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Packet handler which handles buy requests to a player shop (3F 06).
    /// </summary>
    [PlugIn("Player Shop - Buy request", "Packet handler which handles buy requests to a player shop (3F 06).")]
    [Guid("F5B72F91-9651-433D-AC23-5898B950A09B")]
    [BelongsToGroup(StoreHandlerGroupPlugIn.GroupKey)]
    internal class PlayerShopBuyRequestPacketHandlerPlugIn : ISubPacketHandlerPlugIn
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PlayerShopBuyRequestPacketHandlerPlugIn));

        private readonly BuyRequestAction buyAction = new BuyRequestAction();

        /// <inheritdoc />
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => PlayerShopItemBuyRequest.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            PlayerShopItemBuyRequest message = packet;
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("BuyRequest, Player=[{0}], Packet=[{1}]", player.SelectedCharacter.Name, packet.AsString());
            }

            var requestedPlayer = player.CurrentMap.GetObject(message.PlayerId) as Player;
            if (requestedPlayer == null)
            {
                Logger.DebugFormat("Player not found: {0}", message.PlayerId);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            if (message.PlayerName != requestedPlayer.SelectedCharacter.Name)
            {
                Logger.DebugFormat("Player Names dont match: {0} != {1}", message.PlayerName, requestedPlayer.SelectedCharacter.Name);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Player Names don't match. {message.PlayerName} <> {requestedPlayer.SelectedCharacter.Name}", MessageType.BlueNormal);
                return;
            }

            this.buyAction.BuyItem(player, requestedPlayer, message.ItemSlot);
        }
    }
}