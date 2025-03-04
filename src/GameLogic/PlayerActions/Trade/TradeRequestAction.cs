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

        if (!await partner.PlayerState.TryAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            return false;
        }

        if (!await player.PlayerState.TryAdvanceToAsync(PlayerState.TradeRequested).ConfigureAwait(false))
        {
            await partner.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
            return false;
        }

        player.TradingPartner = partner;
        partner.TradingPartner = player;
        await partner.InvokeViewPlugInAsync<IShowTradeRequestPlugIn>(p => p.ShowTradeRequestAsync(player)).ConfigureAwait(false);
        return true;
    }
}