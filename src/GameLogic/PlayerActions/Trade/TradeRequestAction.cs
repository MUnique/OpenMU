// <copyright file="TradeRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// Action to request a trade with another player.
/// </summary>
public class TradeRequestAction
{
    /// <summary>
    /// Requests the trade from the other player.
    /// </summary>
    /// <param name="player">The player who requests the trade.</param>
    /// <param name="partner">The partner which will be requested.</param>
    /// <returns>The success of sending the request to the <paramref name="partner"/>.</returns>
    public async ValueTask<bool> RequestTradeAsync(ITrader player, ITrader partner)
    {
        if (ReferenceEquals(player, partner))
        {
            return false;
        }

        if (player.IsTemplatePlayer || partner.IsTemplatePlayer)
        {
            player.Logger.LogWarning("This or the requested player is template player, cannot trade.");
            return false;
        }

        if (player is Player requester && partner is Player requested
            && requester.GameContext.PlugInManager.GetPlugInPoint<ITradeRequestedPlugIn>() is { } plugInPoint)
        {
            await plugInPoint.TradeRequestedAsync(requester, requested).ConfigureAwait(false);
        }

        if (player.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>() is null || partner.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>() is null)
        {
            return false;
        }

        // Publish each side under its own state scope; never hold both state locks together.
        // A trade is only requested from the entered world state, where a remaining partner reference is stale
        // (e.g. a logout left it behind and nothing cleared it): it is replaced, like the reference of any trade.
        await using (var context = await partner.PlayerState.TryBeginAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            context.Allowed &= IsAvailableForTrade(player) && IsAvailableForTrade(partner)
                && player.PlayerState.CurrentState == PlayerState.EnteredWorld;
            if (!context.Allowed)
            {
                return false;
            }

            partner.TradingPartner = player;
        }

        bool published;
        await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            context.Allowed &= IsAvailableForTrade(player) && IsAvailableForTrade(partner)
                && partner.PlayerState.CurrentState == PlayerState.TradeRequested
                && ReferenceEquals(partner.TradingPartner, player);
            if (context.Allowed)
            {
                player.TradingPartner = partner;

                // Death may reserve either participant between validation and publication.
                context.Allowed = IsAvailableForTrade(player) && IsAvailableForTrade(partner)
                    && partner.PlayerState.CurrentState == PlayerState.TradeRequested
                    && ReferenceEquals(partner.TradingPartner, player);
                if (!context.Allowed && ReferenceEquals(player.TradingPartner, partner))
                {
                    player.TradingPartner = null;
                }
            }

            published = context.Allowed;
        }

        if (!published)
        {
            // Refuse incomplete publication rather than leave an unseen reciprocal request reserved.
            // Without invitation identities, a cancel/reopen race can also roll back a newer pending half.
            await ClearPendingRequestAsync(partner, player).ConfigureAwait(false);
            return false;
        }

        if (!IsCurrentRequest(player, partner))
        {
            return false;
        }

        // Once published, normal death/cancel owns cleanup. A delayed view must not roll back
        // a newer interaction which may have replaced this request in the meantime.
        await partner.InvokeViewPlugInAsync<IShowTradeRequestPlugIn>(p => p.ShowTradeRequestAsync(player)).ConfigureAwait(false);
        return IsCurrentRequest(player, partner);
    }

    private static bool IsAvailableForTrade(ITrader trader) => trader is not Player player || player.IsAvailableForTrade;

    private static bool IsCurrentRequest(ITrader player, ITrader partner) => IsAvailableForTrade(player) && IsAvailableForTrade(partner)
        && player.PlayerState.CurrentState == PlayerState.TradeRequested
        && partner.PlayerState.CurrentState == PlayerState.TradeRequested
        && ReferenceEquals(player.TradingPartner, partner) && ReferenceEquals(partner.TradingPartner, player);

    private static async ValueTask ClearPendingRequestAsync(ITrader trader, ITrader expectedPartner)
    {
        await using var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        context.Allowed &= trader.PlayerState.CurrentState == PlayerState.TradeRequested
            && ReferenceEquals(trader.TradingPartner, expectedPartner);
        if (context.Allowed)
        {
            trader.TradingPartner = null;
        }
    }
}
