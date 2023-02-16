// <copyright file="TradeButtonAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Action to change the trade button state.
/// </summary>
public class TradeButtonAction : BaseTradeAction
{
    /// <summary>
    /// Tries to change the trade button change to the new <paramref name="tradeButtonState"/>.
    /// </summary>
    /// <param name="trader">The trader.</param>
    /// <param name="tradeButtonState">The new state of the trade button.</param>
    public async ValueTask TradeButtonChangedAsync(ITrader trader, TradeButtonState tradeButtonState)
    {
        using var loggerScope = (trader as Player)?.Logger.BeginScope(this.GetType());
        var success = (tradeButtonState == TradeButtonState.Checked && await trader.PlayerState.TryAdvanceToAsync(PlayerState.TradeButtonPressed).ConfigureAwait(false))
                      || (tradeButtonState == TradeButtonState.Unchecked && await trader.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false));
        if (!success)
        {
            return;
        }

        var tradingPartner = trader.TradingPartner;

        if (trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed
            && tradingPartner is not null
            && tradingPartner.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
        {
            TradeResult result = await this.InternalFinishTradeAsync(trader, tradingPartner).ConfigureAwait(false);
            if (result != TradeResult.Success)
            {
                await this.CancelTradeAsync(tradingPartner, false).ConfigureAwait(false);
                await this.CancelTradeAsync(trader, false).ConfigureAwait(false);
                (trader as Player)?.Logger.LogDebug($"Cancelled the trade because of unfinished state. trader: {trader.Name}, partner:{tradingPartner.Name}");
            }

            await trader.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(result)).ConfigureAwait(false);
            await tradingPartner.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(result)).ConfigureAwait(false);
        }
        else if (tradingPartner is not null)
        {
            await tradingPartner.InvokeViewPlugInAsync<IChangeTradeButtonStatePlugIn>(p => p.ChangeTradeButtonStateAsync(TradeButtonState.Checked)).ConfigureAwait(false);
        }
        else
        {
            // nothing to do.
        }
    }

    private static async ValueTask<bool> TryAddItemsOfTradingPartnerAsync(ITrader trader)
    {
        if (trader.TradingPartner?.TemporaryStorage?.Items.Any() ?? false)
        {
            return await trader.Inventory!.TryTakeAllAsync(trader.TradingPartner.TemporaryStorage!).ConfigureAwait(false);
        }

        return true;
    }

    private async ValueTask<TradeResult> InternalFinishTradeAsync(ITrader trader, ITrader tradingPartner)
    {
        await using var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await using var partnerContext = await tradingPartner.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        if (!context.Allowed || !partnerContext.Allowed)
        {
            context.Allowed = false;
            partnerContext.Allowed = false;
            (trader as Player)?.Logger.LogDebug($"Unexpected player states. {trader.Name}:{trader.PlayerState}, {tradingPartner.Name}:{tradingPartner.PlayerState}");
            return TradeResult.Cancelled;
        }

        using var itemContext = trader.GameContext.PersistenceContextProvider.CreateNewTradeContext();
        var traderItems = trader.TemporaryStorage!.Items.ToList();
        var tradePartnerItems = tradingPartner.TemporaryStorage!.Items.ToList();
        this.AttachItemsToPersistenceContext(traderItems, itemContext);
        this.AttachItemsToPersistenceContext(tradePartnerItems, itemContext);

        if (!await TryAddItemsOfTradingPartnerAsync(trader).ConfigureAwait(false) || !await TryAddItemsOfTradingPartnerAsync(tradingPartner).ConfigureAwait(false))
        {
            await this.SendMessageAsync(trader, "Inventory is full.").ConfigureAwait(false);
            await this.SendMessageAsync(tradingPartner, "Inventory is full.").ConfigureAwait(false);
            return TradeResult.FailedByFullInventory;
        }

        try
        {
            this.DetachItemsFromPersistenceContext(traderItems, trader.PersistenceContext);
            this.DetachItemsFromPersistenceContext(tradePartnerItems, trader.TradingPartner!.PersistenceContext);
            await itemContext.SaveChangesAsync().ConfigureAwait(false);
            this.AttachItemsToPersistenceContext(traderItems, trader.TradingPartner.PersistenceContext);
            this.AttachItemsToPersistenceContext(tradePartnerItems, trader.PersistenceContext);
            trader.Money += trader.TradingPartner.TradingMoney;
            trader.TradingPartner.Money += trader.TradingMoney;
            await trader.TradingPartner.InvokeViewPlugInAsync<IChangeTradeButtonStatePlugIn>(p => p.ChangeTradeButtonStateAsync(TradeButtonState.Checked)).ConfigureAwait(false);
            this.ResetTradeState(trader.TradingPartner);
            this.ResetTradeState(trader);
            this.CallPlugIn(traderItems, trader, tradingPartner);
            this.CallPlugIn(tradePartnerItems, tradingPartner, trader);
            FinishedTrades.Add(1);
            return TradeResult.Success;
        }
        catch (Exception exception)
        {
            await this.SendMessageAsync(trader, "An unexpected error occured during closing the trade.").ConfigureAwait(false);
            await this.SendMessageAsync(tradingPartner, "An unexpected error occured during closing the trade.").ConfigureAwait(false);
            context.Allowed = false;
            partnerContext.Allowed = false;
            (trader as Player)?.Logger.LogError(exception, $"An unexpected error occured during closing the trade. trader: {trader.Name}, partner:{tradingPartner.Name}");
            return TradeResult.Cancelled;
        }
    }

    private void CallPlugIn(IEnumerable<Item> items, ITrader source, ITrader target)
    {
        var point = target.GameContext.PlugInManager.GetPlugInPoint<IItemTradedToOtherPlayerPlugIn>();
        if (point is null)
        {
            return;
        }

        foreach (var item in items)
        {
            point.ItemTraded(source, target, item);
        }
    }

    private void AttachItemsToPersistenceContext(IEnumerable<Item> items, IContext itemContext)
    {
        foreach (var item in items)
        {
            itemContext.Attach(item);
        }
    }

    private void DetachItemsFromPersistenceContext(IEnumerable<Item> items, IContext itemContext)
    {
        foreach (var item in items)
        {
            itemContext.Detach(item);
        }
    }
}