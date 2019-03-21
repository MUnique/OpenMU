// <copyright file="TradeMoneyAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to set the traded money.
    /// </summary>
    public class TradeMoneyAction
    {
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
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Uncheck trade accept button first", MessageType.BlueNormal);
                return;
            }

            // Check if the Player got enough Zen/Money
            if (player.Money < moneyAmount)
            {
                return;
            }

            // Add the Money to the Trade
            player.TryAddMoney(player.TradingMoney);
            player.TryAddMoney((int)(-1 * moneyAmount));
            player.TradingMoney = (int)moneyAmount;
            player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
            player.ViewPlugIns.GetPlugIn<IRequestedTradeMoneyHasBeenSetPlugIn>()?.RequestedTradeMoneyHasBeenSet();

            // Send the Money Packet to the Trading Partner
            player.TradingPartner?.ViewPlugIns.GetPlugIn<ISetTradeMoneyPlugIn>()?.SetTradeMoney(moneyAmount);

            player.ViewPlugIns.GetPlugIn<IChangeTradeButtonStatePlugIn>()?.ChangeTradeButtonState(TradeButtonState.Red);
            if (player.TradingPartner != null)
            {
                player.TradingPartner.PlayerState.TryAdvanceTo(PlayerState.TradeOpened);
                player.TradingPartner.ViewPlugIns.GetPlugIn<IChangeTradeButtonStatePlugIn>()?.ChangeTradeButtonState(TradeButtonState.Red);
            }
        }
    }
}
