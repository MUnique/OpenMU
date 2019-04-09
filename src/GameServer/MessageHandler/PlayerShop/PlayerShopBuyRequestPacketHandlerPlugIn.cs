// <copyright file="PlayerShopBuyRequestPacketHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.PlayerShop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
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
        public byte Key => 0x06;

        /// <inheritdoc/>
        public void HandlePacket(Player player, Span<byte> packet)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("BuyRequest, Player=[{0}], Packet=[{1}]", player.SelectedCharacter.Name, packet.AsString());
            }

            ushort requestedId = packet.MakeWordSmallEndian(4);
            var requestedPlayer = player.CurrentMap.GetObject(requestedId) as Player;
            if (requestedPlayer == null)
            {
                Logger.DebugFormat("Player not found: {0}", requestedId);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            string playerName = packet.ExtractString(5, 10, Encoding.UTF8);
            if (playerName != requestedPlayer.SelectedCharacter.Name)
            {
                Logger.DebugFormat("Player Names dont match: {0} != {1}", playerName, requestedPlayer.SelectedCharacter.Name);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage($"Player Names don't match. {playerName} <> {requestedPlayer.SelectedCharacter.Name}", MessageType.BlueNormal);
                return;
            }

            byte slot = packet[16];

            this.buyAction.BuyItem(player, requestedPlayer, slot);
        }
    }
}