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
            try
            {
                // The backups of both traders have to exist before the state allows to put zen or items
                // into the trade. Otherwise, zen or items which are put into the trade in between would
                // already be missing in the backup, and wouldn't come back when the trade is cancelled.
                await this.PrepareTradeAsync(tradeAccepter).ConfigureAwait(false);
                await this.PrepareTradeAsync(tradePartner).ConfigureAwait(false);

                if (!await tradeAccepter.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false) || !await tradePartner.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false))
                {
                    // Failed to open the trade because a player could not enter the TradeOpened state.
                    await this.CancelTradeAsync(tradeAccepter).ConfigureAwait(false);
                    await this.CancelTradeAsync(tradePartner).ConfigureAwait(false);
                    await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
                    await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
                    return;
                }

                await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(true)).ConfigureAwait(false);
                await tradePartner.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(true)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                tradeAccepter.Logger.LogError(ex, "Error while opening a trade between {tradeAccepter} and {tradePartner}", tradeAccepter, tradePartner);
                await this.CancelTradeAsync(tradeAccepter).ConfigureAwait(false);
                await this.CancelTradeAsync(tradePartner).ConfigureAwait(false);
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
    /// Prepares the trade of a trader: saves the progress and creates the backup of the inventory,
    /// which is restored when the trade is cancelled. It has to be done before the trader is allowed
    /// to add zen or items to the trade.
    /// </summary>
    /// <param name="trader">Trader for which the trade should be prepared.</param>
    internal async ValueTask PrepareTradeAsync(ITrader trader)
    {
        // first make sure that all items which could be transferred are already present in the database
        await trader.SaveProgressAsync().ConfigureAwait(false);

        // Items which another window (e.g. the chaos machine) left in the temporary storage go back to
        // the inventory first. The trade uses the same storage, so they would be offered in the trade,
        // and lost when it is cancelled, because a cancel restores the inventory backup made below.
        if (trader.TemporaryStorage is { } temporaryStorage
            && temporaryStorage.Items.Any()
            && !await trader.Inventory!.TryTakeAllAsync(temporaryStorage).ConfigureAwait(false))
        {
            trader.Logger.LogWarning("Could not return the items of the temporary storage of {trader} to the inventory before opening a trade.", trader);
        }

        trader.BackupInventory = new BackupItemStorage(trader.Inventory!.ItemStorage);
        trader.TradingMoney = 0;
    }
}