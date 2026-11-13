// <copyright file="TradeRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

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
        if (player.IsTemplatePlayer || partner.IsTemplatePlayer)
        {
            player.Logger.LogWarning("This or the requested player is template player, cannot trade.");
            return false;
        }

        if (player.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>() is null || partner.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>() is null)
        {
            return false;
        }

        // Publish each side under its own state scope; never hold both state locks together.
        await using (var context = await partner.PlayerState.TryBeginAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            context.Allowed &= IsAlive(player) && IsAlive(partner)
                && player.TradingPartner is null && partner.TradingPartner is null;
            if (!context.Allowed)
            {
                return false;
            }

            partner.TradingPartner = player;
        }

        bool published;
        await using (var context = await player.PlayerState.TryBeginAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            context.Allowed &= IsAlive(player) && IsAlive(partner)
                && partner.PlayerState.CurrentState == PlayerState.TradeRequested
                && ReferenceEquals(partner.TradingPartner, player)
                && player.TradingPartner is null;
            if (context.Allowed)
            {
                player.TradingPartner = partner;

                // Death may reserve either participant between validation and publication.
                context.Allowed = IsAlive(player) && IsAlive(partner)
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
            if (!IsCurrentRequest(player, partner))
            {
                await ClearPendingRequestAsync(partner, player).ConfigureAwait(false);
            }

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

    private static bool IsAlive(ITrader trader) => trader is not Player player || player.IsAlive;

    private static bool IsCurrentRequest(ITrader player, ITrader partner) => IsAlive(player) && IsAlive(partner)
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
