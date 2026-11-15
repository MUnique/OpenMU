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
        if (tradePartner is not null && !ReferenceEquals(tradePartner.TradingPartner, tradeAccepter))
        {
            // The requester left this request (e.g. by a logout) and may have another partner now. The answer ends
            // only this remaining half; the requester and its current partner aren't touched.
            await EndStaleRequestAsync(tradeAccepter, tradePartner).ConfigureAwait(false);
            await tradeAccepter.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(false)).ConfigureAwait(false);
            return;
        }

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
                    if (await this.OpenTradeAsync(tradeAccepter, tradePartner).ConfigureAwait(false))
                    {
                        await this.OpenTradeAsync(tradePartner, tradeAccepter).ConfigureAwait(false);
                    }
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
    /// <param name="expectedPartner">The partner of the accepted request.</param>
    /// <returns>Whether the trade is still open after its response.</returns>
    internal async ValueTask<bool> OpenTradeAsync(ITrader trader, ITrader expectedPartner)
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
            // Without a backup, the cancel of the opening error handling keeps these items where they are.
            throw new InvalidOperationException("Could not return temporary storage items to the inventory before opening a trade.");
        }

        // A cancel (e.g. by a death) clears the backup under the same state exclusion. A continuation of a
        // trade which was cancelled meanwhile must not publish a backup for the next interaction.
        await using (var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false))
        {
            context.Allowed = false;
            if (!IsCurrentTrade(trader, expectedPartner))
            {
                return false;
            }

            trader.BackupInventory = new BackupItemStorage(trader.Inventory!.ItemStorage);
            trader.TradingMoney = 0;
        }

        await trader.InvokeViewPlugInAsync<IShowTradeRequestAnswerPlugIn>(p => p.ShowTradeRequestAnswerAsync(true)).ConfigureAwait(false);
        return IsCurrentTrade(trader, expectedPartner);
    }

    private static async ValueTask EndStaleRequestAsync(ITrader trader, ITrader expectedPartner)
    {
        await using var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        context.Allowed &= trader.PlayerState.CurrentState == PlayerState.TradeRequested
            && ReferenceEquals(trader.TradingPartner, expectedPartner)
            && !ReferenceEquals(expectedPartner.TradingPartner, trader);
        if (context.Allowed)
        {
            trader.TradingPartner = null;
        }
    }

    private static bool IsCurrentTrade(ITrader trader, ITrader expectedPartner)
        => trader.PlayerState.CurrentState == PlayerState.TradeOpened
            && expectedPartner.PlayerState.CurrentState == PlayerState.TradeOpened
            && ReferenceEquals(trader.TradingPartner, expectedPartner)
            && ReferenceEquals(expectedPartner.TradingPartner, trader)
            && (trader is not Player player || player.IsAvailableForTrade)
            && (expectedPartner is not Player partner || partner.IsAvailableForTrade);
}
