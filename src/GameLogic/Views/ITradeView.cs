// <copyright file="ITradeView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using Interfaces;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The state of the trade button.
    /// </summary>
    public enum TradeButtonState : byte
    {
        /// <summary>
        /// Trade button is not pressed. It means that the trade is not yet accepted by the trader.
        /// </summary>
        Unchecked = 0,

        /// <summary>
        /// Trade Button is pressed. It means that the trade is accepted by the trader.
        /// </summary>
        Checked = 1,

        /// <summary>
        /// This state is only sent to the client. After some seconds the client is changing back to normal Unchecked.
        /// </summary>
        Red = 2
    }

    /// <summary>
    /// The result of a closed trade (both players pressed 'OK').
    /// </summary>
    public enum TradeResult
    {
        /// <summary>
        /// The trade was cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The trade was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The trade failed because of a full inventory.
        /// </summary>
        FailedByFullInventory,

        /// <summary>
        /// The trade failed because the request timed out.
        /// </summary>
        TimedOut,

        /// <summary>
        /// The trade failed because one or more items were not allowed to trade
        /// </summary>
        FailedByItemsNotAllowedToTrade,
    }

    /// <summary>
    /// Interface for the trade view.
    /// </summary>
    public interface ITradeView
    {
        /// <summary>
        /// An item has been removed from the trade window by the other player.
        /// </summary>
        /// <remarks>
        /// It will be also sent when an item is moved within the trade window.
        /// </remarks>
        /// <param name="fromSlot">The slot from which the item has been removed.</param>
        /// <param name="item">The item which has been removed.</param>
        void TradeItemDisappear(byte fromSlot, Item item);

        /// <summary>
        /// An item has been added to the trade window by the other player.
        /// </summary>
        /// <remarks>
        /// It is also sent when an item gets moved inside the trade window. In this case a TradeItemDisappear message was sent before for the old slot.
        /// </remarks>
        /// <param name="toSlot">The slot at which the item has been added.</param>
        /// <param name="item">The item which has been added.</param>
        void TradeItemAppear(byte toSlot, Item item);

        /// <summary>
        /// Shows the trade request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowTradeRequest(ITrader requester);

        /// <summary>
        /// The requested trade partner has answered the request. Information about the tradingpartner can be found at ITrader.TradingPartner.
        /// </summary>
        /// <param name="tradeAccepted">if set to <c>true</c> the trade has been accepted and will be opened.</param>
        void ShowTradeRequestAnswer(bool tradeAccepted);

        /// <summary>
        /// The trade process has finished with the specified result.
        /// </summary>
        /// <param name="tradeResult">The trade result.</param>
        void TradeFinished(TradeResult tradeResult);

        /// <summary>
        /// Changes the state of the other players trade button.
        /// </summary>
        /// <param name="state">The state.</param>
        void ChangeTradeButtonState(TradeButtonState state);

        /// <summary>
        /// Sets the amount of money which should be traded to the other player.
        /// </summary>
        /// <remarks>
        /// This message is sent when the trading partner put a certain amount of zen (also 0) into the trade.
        /// It overrides all previous sent zen values.
        /// </remarks>
        /// <param name="moneyAmount">The money amount.</param>
        void SetTradeMoney(uint moneyAmount);

        /// <summary>
        /// Notifies the client that the money amount which has been previously requested to be set,
        /// has been successfully placed into the trade.
        /// </summary>
        void RequestedTradeMoneyHasBeenSet();
    }
}
