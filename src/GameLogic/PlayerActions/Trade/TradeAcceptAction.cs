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
    public async ValueTask HandleTradeAcceptAsync(ITrader tradeAccepter, bool accept)
    {
        var tradePartner = tradeAccepter.TradingPartner;
        if (accept && tradePartner != null)
        {
            if (!await tradeAccepter.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false) || !await tradePartner.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false))
            {
                ////Something bad happened here...
                await this.CancelTradeAsync(tradeAccepter).ConfigureAwait(false);
                await this.CancelTradeAsync(tradePartner).ConfigureAwait(false);
                await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
                await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    await this.OpenTradeAsync(tradeAccepter).ConfigureAwait(false);
                    await this.OpenTradeAsync(tradePartner).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    tradeAccepter.Logger.LogError(ex, "Error while opening a trade between {tradeAccepter} and {tradePartner}", tradeAccepter, tradePartner);
                    await this.CancelTradeAsync(tradeAccepter).ConfigureAwait(false);
                    await this.CancelTradeAsync(tradePartner).ConfigureAwait(false);
                }
            }
        }
        else
        {
            if (tradePartner != null)
            {
                await this.CancelTradeAsync(tradePartner).ConfigureAwait(false);
                await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
            }

            await this.CancelTradeAsync(tradeAccepter).ConfigureAwait(false);
            await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Opens the trade.
    /// </summary>
    /// <param name="trader">Trader for which the trade should be opened.</param>
    internal async ValueTask OpenTradeAsync(ITrader trader)
    {
        // first make sure that all items which could be transferred are already present in the database
        await trader.SaveProgressAsync().ConfigureAwait(false);

        trader.BackupInventory = new BackupItemStorage(trader.Inventory!.ItemStorage);
        trader.TradingMoney = 0;
        await trader.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(true)).ConfigureAwait(false);
    }
}