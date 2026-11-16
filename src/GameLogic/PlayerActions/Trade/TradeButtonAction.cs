// <copyright file="TradeButtonAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using System.Runtime.ExceptionServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Action to change the trade button state.
/// </summary>
public class TradeButtonAction : BaseTradeAction
{
    private enum FinishOutcome
    {
        /// <summary>The trade ended with the returned result.</summary>
        Completed,

        /// <summary>The partner left this trade; it wasn't touched.</summary>
        PartnerLeft,

        /// <summary>The trade isn't confirmed by both traders anymore and stays open.</summary>
        Unconfirmed,
    }

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
            var (outcome, result) = await this.InternalFinishTradeAsync(trader, tradingPartner).ConfigureAwait(false);
            if (outcome == FinishOutcome.PartnerLeft)
            {
                // The partner left this trade (e.g. by a logout) and trades with another player now. Only this
                // remaining side ends, with its own items; the other trade isn't touched.
                if (await this.CancelTradeAsync(trader, expectedPartner: tradingPartner).ConfigureAwait(false))
                {
                    await trader.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(TradeResult.Cancelled)).ConfigureAwait(false);
                }

                return;
            }

            if (outcome == FinishOutcome.Unconfirmed)
            {
                // Not both confirmed anymore: the trade stays open, as if the confirmation had arrived after the
                // withdrawal.
                if (trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
                {
                    await tradingPartner.InvokeViewPlugInAsync<IChangeTradeButtonStatePlugIn>(p => p.ChangeTradeButtonStateAsync(TradeButtonState.Checked)).ConfigureAwait(false);
                }

                return;
            }

            if (result != TradeResult.Success)
            {
                return;
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

    private static bool IsInOpenTrade(ITrader trader) => trader.PlayerState.CurrentState == PlayerState.TradeOpened
        || trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed;

    private static bool IsAvailable(ITrader trader) => trader is not Player player || player.IsAvailableForTrade;

    private async ValueTask<(FinishOutcome Outcome, TradeResult Result)> InternalFinishTradeAsync(ITrader trader, ITrader tradingPartner)
    {
        var traderState = trader.PlayerState;
        var partnerState = tradingPartner.PlayerState;
        var traderFirst = traderState.TransitionLockId < partnerState.TransitionLockId;
        var firstState = traderFirst ? traderState : partnerState;
        var secondState = traderFirst ? partnerState : traderState;
        var traderNotification = ValueTask.CompletedTask;
        var partnerNotification = ValueTask.CompletedTask;
        Exception? firstError = null;
        TradeResult result;
        await using (var firstContext = await firstState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        await using (var secondContext = await secondState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            var context = traderFirst ? firstContext : secondContext;
            var partnerContext = traderFirst ? secondContext : firstContext;
            if (!ReferenceEquals(trader.TradingPartner, tradingPartner) || !ReferenceEquals(tradingPartner.TradingPartner, trader))
            {
                context.Allowed = false;
                partnerContext.Allowed = false;
                return (FinishOutcome.PartnerLeft, TradeResult.Cancelled);
            }

            var traderAllowed = context.Allowed;
            var partnerAllowed = partnerContext.Allowed;
            context.Allowed = false;
            partnerContext.Allowed = false;
            if (traderAllowed && partnerAllowed && IsInOpenTrade(trader) && IsInOpenTrade(tradingPartner)
                && (traderState.CurrentState != PlayerState.TradeButtonPressed
                    || partnerState.CurrentState != PlayerState.TradeButtonPressed))
            {
                // A confirmation was withdrawn after the unlocked check. Nothing is exchanged; the trade stays as it is.
                return (FinishOutcome.Unconfirmed, TradeResult.Cancelled);
            }

            // A trader whose death or forced placement is already reserved at this point doesn't exchange; the trade is
            // rolled back. A death reserved later is applied after this finish, which owns both state exclusions.
            var traderAvailable = IsAvailable(trader);
            var partnerAvailable = IsAvailable(tradingPartner);
            if (!traderAllowed || !partnerAllowed
                || traderState.CurrentState != PlayerState.TradeButtonPressed
                || partnerState.CurrentState != PlayerState.TradeButtonPressed
                || !traderAvailable
                || !partnerAvailable)
            {
                trader.Logger.LogDebug(
                    "Unexpected player states. {Trader}:{TraderState} (allowed: {TraderAllowed}, available: {TraderAvailable}), {Partner}:{PartnerState} (allowed: {PartnerAllowed}, available: {PartnerAvailable})",
                    trader.Name,
                    traderState.CurrentState,
                    traderAllowed,
                    traderAvailable,
                    tradingPartner.Name,
                    partnerState.CurrentState,
                    partnerAllowed,
                    partnerAvailable);
                result = TradeResult.Cancelled;
            }
            else
            {
                try
                {
                    result = await this.TransferTradeAsync(trader, tradingPartner, context, partnerContext).ConfigureAwait(false);
                }
                catch (Exception error)
                {
                    // Nothing was committed (a committed transfer always returns its success), but a part of the
                    // exchange may already be applied. Roll both halves back below, then rethrow.
                    firstError = error;
                    result = TradeResult.Cancelled;
                }
            }

            if (result != TradeResult.Success)
            {
                // Both exclusions still own the partially applied inventory until both rollback attempts finish.
                await ObserveAsync(async () =>
                {
                    await RollbackAsync(tradingPartner, trader, partnerContext, partnerAllowed, () => partnerNotification = StartFailureResultAsync(tradingPartner)).ConfigureAwait(false);
                }).ConfigureAwait(false);
                await ObserveAsync(async () =>
                {
                    await RollbackAsync(trader, tradingPartner, context, traderAllowed, () => traderNotification = StartFailureResultAsync(trader)).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }

        if (result != TradeResult.Success)
        {
            await ObserveAsync(() => traderNotification).ConfigureAwait(false);
            await ObserveAsync(() => partnerNotification).ConfigureAwait(false);
            if (firstError is not null)
            {
                ExceptionDispatchInfo.Capture(firstError).Throw();
            }

            (trader as Player)?.Logger.LogDebug($"Cancelled the trade because of unfinished state. trader: {trader.Name}, partner:{tradingPartner.Name}");
        }

        return (FinishOutcome.Completed, result);

        async ValueTask RollbackAsync(ITrader participant, ITrader expectedPartner, StateMachine.StateChangeContext stateContext, bool allowed, Action startResult)
        {
            CancelledTrades.Add(1);
            if (!allowed || !ReferenceEquals(participant.TradingPartner, expectedPartner)
                || (participant.PlayerState.CurrentState != PlayerState.TradeRequested
                    && participant.PlayerState.CurrentState != PlayerState.TradeOpened
                    && participant.PlayerState.CurrentState != PlayerState.TradeButtonPressed))
            {
                return;
            }

            await this.RestoreTradeAsync(participant).ConfigureAwait(false);
            startResult();
            stateContext.Allowed = true;
        }

        ValueTask StartFailureResultAsync(ITrader participant)
        {
            try
            {
                return participant.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(result));
            }
            catch (Exception error)
            {
                return ValueTask.FromException(error);
            }
        }

        async ValueTask ObserveAsync(Func<ValueTask> operation)
        {
            try
            {
                await operation().ConfigureAwait(false);
            }
            catch (Exception error)
            {
                if (firstError is null)
                {
                    firstError = error;
                }
                else
                {
                    (trader as Player)?.Logger.LogError(error, "Additional error while rolling back a trade between {Trader} and {Partner}.", trader, tradingPartner);
                }
            }
        }
    }

    private async ValueTask<TradeResult> TransferTradeAsync(ITrader trader, ITrader tradingPartner, StateMachine.StateChangeContext context, StateMachine.StateChangeContext partnerContext)
    {
        using var itemContext = trader.GameContext.PersistenceContextProvider.CreateNewTradeContext();
        var traderItems = trader.TemporaryStorage!.Items.ToList();
        var tradePartnerItems = tradingPartner.TemporaryStorage!.Items.ToList();

        // Checked here, and not only when an item is moved into the trade: the temporary storage
        // is shared with other windows (e.g. the chaos machine), so it may already contain items.
        // Both are checked, so that each trader gets the message about their own items.
        var traderItemsAreTradable = await this.CheckItemsAreTradableAsync(trader, traderItems).ConfigureAwait(false);
        var partnerItemsAreTradable = await this.CheckItemsAreTradableAsync(tradingPartner, tradePartnerItems).ConfigureAwait(false);
        if (!traderItemsAreTradable || !partnerItemsAreTradable)
        {
            return TradeResult.Cancelled;
        }

        this.AttachItemsToPersistenceContext(traderItems, itemContext);
        this.AttachItemsToPersistenceContext(tradePartnerItems, itemContext);

        if (!await TryAddItemsOfTradingPartnerAsync(trader).ConfigureAwait(false) || !await TryAddItemsOfTradingPartnerAsync(tradingPartner).ConfigureAwait(false))
        {
            // TODO: Maybe notify which player has the full inventory.
            await this.SendMessageAsync(trader, nameof(PlayerMessage.InventoryFull)).ConfigureAwait(false);
            await this.SendMessageAsync(tradingPartner, nameof(PlayerMessage.InventoryFull)).ConfigureAwait(false);
            return TradeResult.FailedByFullInventory;
        }

        var itemsCommitted = false;
        try
        {
            this.DetachItemsFromPersistenceContext(traderItems, trader.PersistenceContext);
            this.DetachItemsFromPersistenceContext(tradePartnerItems, trader.TradingPartner!.PersistenceContext);
            if (!await itemContext.SaveChangesAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Trade item changes were not saved.");
            }

            itemsCommitted = true;
            context.Allowed = true;
            partnerContext.Allowed = true;

            // Item ownership is now committed. Move the escrowed money before any
            // post-commit attachment or client notification can fail.
            trader.Money += trader.TradingPartner.TradingMoney;
            trader.TradingPartner.Money += trader.TradingMoney;

            // Attaching a transferred item's entire graph would walk the receiving
            // inventory and can collide with items that context already tracks.
            this.AttachTransferredItemsToPersistenceContext(traderItems, trader.TradingPartner.PersistenceContext);
            this.AttachTransferredItemsToPersistenceContext(tradePartnerItems, trader.PersistenceContext);
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
            if (itemsCommitted)
            {
                // SaveChanges already committed item ownership. Cancelling the trade
                // here would replay both inventory backups over the committed state.
                // A remaining attachment failure needs operator investigation, but it
                // must never turn a persisted trade into a reported cancellation.
                (trader as Player)?.Logger.LogCritical(
                    exception,
                    "Trade was committed but post-commit completion failed. trader: {Trader}, partner: {Partner}.",
                    trader.Name,
                    tradingPartner.Name);
                this.ResetTradeState(tradingPartner);
                this.ResetTradeState(trader);
                FinishedTrades.Add(1);
                return TradeResult.Success;
            }

            await this.SendMessageAsync(trader, nameof(PlayerMessage.UnexpectedErrorDuringClosingTrade)).ConfigureAwait(false);
            await this.SendMessageAsync(tradingPartner, nameof(PlayerMessage.UnexpectedErrorDuringClosingTrade)).ConfigureAwait(false);
            context.Allowed = false;
            partnerContext.Allowed = false;
            (trader as Player)?.Logger.LogError(exception, $"An unexpected error occured during closing the trade. trader: {trader.Name}, partner:{tradingPartner.Name}");
            return TradeResult.Cancelled;
        }
    }

    private async ValueTask<bool> CheckItemsAreTradableAsync(ITrader trader, IEnumerable<Item> items)
    {
        var notTradable = items.FirstOrDefault(item => item.Definition is { IsTradable: false });
        if (notTradable is null)
        {
            return true;
        }

        (trader as Player)?.Logger.LogWarning(
            "Trader {0} offered {1}, which its item definition doesn't allow to trade. The trade is cancelled.",
            trader.Name,
            notTradable);
        await this.SendMessageAsync(trader, nameof(PlayerMessage.ItemCannotBeTraded)).ConfigureAwait(false);
        return false;
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

    private void AttachTransferredItemsToPersistenceContext(IEnumerable<Item> items, IContext itemContext)
    {
        foreach (var item in items)
        {
            itemContext.AttachWithoutParent(item);
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
