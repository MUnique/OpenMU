// <copyright file="StoreHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler
{
    using System;
    using System.Text;
    using log4net;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// Handler for player store related packets.
    /// </summary>
    internal class StoreHandler : BasePacketHandler, IPacketHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(StoreHandler));

        private readonly SetItemPriceAction setPriceAction;

        private readonly OpenStoreAction openStoreAction;

        private readonly CloseStoreAction closeStoreAction;

        private readonly StoreItemListRequestAction requestListAction;

        private readonly BuyRequestAction buyAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreHandler"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public StoreHandler(IGameContext gameContext)
            : base(gameContext)
        {
            this.setPriceAction = new SetItemPriceAction();
            this.openStoreAction = new OpenStoreAction();
            this.closeStoreAction = new CloseStoreAction();
            this.requestListAction = new StoreItemListRequestAction();
            this.buyAction = new BuyRequestAction();
        }

        /// <inheritdoc/>
        public override void HandlePacket(Player player, Span<byte> packet)
        {
            switch (packet[3])
            {
                case 0x01:
                    this.ReadItemPrice(player, packet);
                    break;
                case 0x02:
                    this.OpenStore(player, packet);
                    break;
                case 0x03:
                    this.closeStoreAction.CloseStore(player);
                    break;
                case 0x05:
                    this.ReadShopItemListRequest(player, packet);
                    break;
                case 0x06:
                    this.ReadBuyRequest(player, packet);
                    break;
                default:
                    throw new NotImplementedException($"Store Packet {packet[3] : X} isn't implemented.");
            }
        }

        private void ReadBuyRequest(Player player, Span<byte> buffer)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("BuyRequest, Player=[{0}], Packet=[{1}]", player.SelectedCharacter.Name, buffer.AsString());
            }

            ushort reqPid = NumberConversionExtensions.MakeWord(buffer[5], buffer[4]);
            var requestedPlayer = player.CurrentMap.GetObject(reqPid) as Player;

            if (requestedPlayer == null)
            {
                Logger.DebugFormat("Player not found: {0}", reqPid);
                player.PlayerView.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            string pname = buffer.ExtractString(5, 10, Encoding.UTF8);
            if (pname != requestedPlayer.SelectedCharacter.Name)
            {
                Logger.DebugFormat("Player Names dont match: {0} != {1}", pname, requestedPlayer.SelectedCharacter.Name);
                player.PlayerView.ShowMessage("Player Names don't match. " + pname + "<>" + requestedPlayer.SelectedCharacter.Name, MessageType.BlueNormal);
                return;
            }

            byte slot = buffer[16];

            this.buyAction.BuyItem(player, requestedPlayer, slot);
        }

        private void ReadShopItemListRequest(Player player, Span<byte> buffer)
        {
            ushort reqPid = NumberConversionExtensions.MakeWord(buffer[5], buffer[4]);
            var requestedPlayer = player.CurrentMap.GetObject(reqPid) as Player;
            if (requestedPlayer == null)
            {
                player.PlayerView.ShowMessage("Open Store: Player not found.", MessageType.BlueNormal);
                return;
            }

            string pname = buffer.ExtractString(6, 10, Encoding.UTF8);

            if (pname != requestedPlayer.SelectedCharacter.Name)
            {
                player.PlayerView.ShowMessage("Player Names don't match." + pname + "<>" + requestedPlayer.SelectedCharacter.Name, MessageType.BlueNormal);
                return;
            }

            this.requestListAction.RequestStoreItemList(player, requestedPlayer);
        }

        private void OpenStore(Player player, Span<byte> buffer)
        {
            ////storename length 26
            var storeName = buffer.ExtractString(4, 26, Encoding.UTF8);
            this.openStoreAction.OpenStore(player, storeName);
        }

        private void ReadItemPrice(Player player, Span<byte> buffer)
        {
            var itemSlot = buffer[4];
            var price = (int)buffer.MakeDwordBigEndian(5);
            Logger.DebugFormat("Player [{0}] sets price of slot {1} to {2}", player.SelectedCharacter.Name, itemSlot, price);
            this.setPriceAction.SetPrice(player, itemSlot, price);
        }
    }
}
