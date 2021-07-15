// <copyright file="PlayerShopBuyRequestPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
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
        private readonly BuyRequestAction buyAction = new ();

        /// <inheritdoc />
        public bool IsEncryptionExpected => true;

        /// <inheritdoc/>
        public byte Key => PlayerShopItemBuyRequest.SubCode;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            PlayerShopItemBuyRequest message = packet;

            if (player.CurrentMap?.GetObject(message.PlayerId) is not Player requestedPlayer)
            {
                player.Logger.LogDebug("Player not found: {0}", message.PlayerId);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            if (message.PlayerName != requestedPlayer.SelectedCharacter?.Name)
            {
                player.Logger.LogDebug("Player Names dont match: {0} != {1}", message.PlayerName, requestedPlayer.SelectedCharacter?.Name);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Player Names don't match. {message.PlayerName} <> {requestedPlayer.SelectedCharacter?.Name}", MessageType.BlueNormal);
                return;
            }

            this.buyAction.BuyItem(player, requestedPlayer, message.ItemSlot);
        }
    }
}