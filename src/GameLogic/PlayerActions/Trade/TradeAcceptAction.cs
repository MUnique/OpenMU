// <copyright file="TradeAcceptAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// Action to accept the trade.
/// </summary>
public class TradeAcceptAction : BaseTradeAction
{
    /// <summary>
    /// Handles the trade request answer.
    /// </summary>
    /// <param name="tradeAccepter">The trader which answers the trade request.</param>
    /// <param name="accept">Indicates if the trader accepted the trade request.</param>
    public async ValueTask HandleTradeAccept(ITrader tradeAccepter, bool accept)
    {
        var tradePartner = tradeAccepter.TradingPartner;
        if (accept && tradePartner != null)
        {
            if (!tradeAccepter.PlayerState.TryAdvanceTo(PlayerState.TradeOpened) || !tradePartner.PlayerState.TryAdvanceTo(PlayerState.TradeOpened))
            {
                ////Something bad happened here...
                await this.CancelTradeAsync(tradeAccepter);
                await this.CancelTradeAsync(tradePartner);
                await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
                await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
            }
            else
            {
                await this.OpenTradeAsync(tradeAccepter);
                await this.OpenTradeAsync(tradePartner);
            }
        }
        else
        {
            if (tradePartner != null)
            {
                await this.CancelTradeAsync(tradePartner);
                await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
            }

            await this.CancelTradeAsync(tradeAccepter);
            await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Opens the trade.
    /// </summary>
    /// <param name="trader">Trader for which the trade should be opened.</param>
    internal async ValueTask OpenTradeAsync(ITrader trader)
    {
        trader.BackupInventory = new BackupItemStorage(trader.Inventory!.ItemStorage);
        trader.TradingMoney = 0;
        await trader.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(true)).ConfigureAwait(false);
    }
}