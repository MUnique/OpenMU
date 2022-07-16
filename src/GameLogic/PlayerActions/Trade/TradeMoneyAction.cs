// <copyright file="TradeMoneyAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to set the traded money.
/// </summary>
public class TradeMoneyAction
{
    /// <summary>
    /// Sets the money which should be traded to the other player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="moneyAmount">The money amount.</param>
    public async ValueTask TradeMoneyAsync(Player player, uint moneyAmount)
    {
        // Check if Trade is open
        if (player.PlayerState.CurrentState != PlayerState.TradeOpened)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Uncheck trade accept button first", MessageType.BlueNormal)).ConfigureAwait(false);
            return;
        }

        // Check if the Player got enough Zen/Money
        if (player.Money < moneyAmount)
        {
            return;
        }

        // Add the Money to the Trade
        player.TryAddMoney(player.TradingMoney);
        player.TryAddMoney((int)(-1 * moneyAmount));
        player.TradingMoney = (int)moneyAmount;
        await player.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync()).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IRequestedTradeMoneyHasBeenSetPlugIn>(p => p.RequestedTradeMoneyHasBeenSetAsync()).ConfigureAwait(false);

        await player.InvokeViewPlugInAsync<IChangeTradeButtonStatePlugIn>(p => p.ChangeTradeButtonStateAsync(TradeButtonState.Red)).ConfigureAwait(false);
        if (player.TradingPartner is { } tradingPartner)
        {
            // Send the Money Packet to the Trading Partner
            await tradingPartner.InvokeViewPlugInAsync<ISetTradeMoneyPlugIn>(p => p.SetTradeMoneyAsync(moneyAmount)).ConfigureAwait(false);
            await tradingPartner.PlayerState.TryAdvanceToAsync(PlayerState.TradeOpened).ConfigureAwait(false);
            await tradingPartner.InvokeViewPlugInAsync<IChangeTradeButtonStatePlugIn>(p => p.ChangeTradeButtonStateAsync(TradeButtonState.Red)).ConfigureAwait(false);
        }
    }
}