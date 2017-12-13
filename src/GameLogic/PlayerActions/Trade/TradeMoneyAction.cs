// <copyright file="TradeMoneyAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to set the traded money.
    /// </summary>
    public class TradeMoneyAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeMoneyAction"/> class.
        /// </summary>
        public TradeMoneyAction()
        {
        }

        /// <summary>
        /// Sets the money which should be traded to the other player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="moneyAmount">The money amount.</param>
        public void TradeMoney(Player player, uint moneyAmount)
        {
            // Check if Trade is open
            if (player.PlayerState.CurrentState != PlayerState.TradeOpened)
            {
                player.PlayerView.ShowMessage("Uncheck trade accept button first", MessageType.BlueNormal);
                return;
            }

            // Check if the Player got enough Zen/Money
            if (player.SelectedCharacter.Money < moneyAmount)
            {
                return;
            }

            // Add the Money to the Trade
            player.TryAddMoney(player.TradingMoney);
            player.TryAddMoney((int)(-1 * moneyAmount));
            player.TradingMoney = (int)moneyAmount;
            player.PlayerView.InventoryView.UpdateMoney();
            player.PlayerView.TradeView.RequestedTradeMoneyHasBeenSet();

            // Send the Money Packet to the Trading Partner
            var tradingPartner = player.TradingPartner as Player;
            if (tradingPartner != null)
            {
                tradingPartner.PlayerView.TradeView.SetTradeMoney(moneyAmount);
            }

            player.PlayerView.TradeView.ChangeTradeButtonState(TradeButtonState.Red);
            if (tradingPartner != null)
            {
                tradingPartner.PlayerState.TryAdvanceTo(PlayerState.TradeOpened);
                tradingPartner.PlayerView.TradeView.ChangeTradeButtonState(TradeButtonState.Red);
            }
        }
    }
}
