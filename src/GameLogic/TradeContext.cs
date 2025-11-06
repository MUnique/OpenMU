// <copyright file="TradeContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Context object that encapsulates trading state and logic for a player.
/// </summary>
public class TradeContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TradeContext"/> class.
    /// </summary>
    /// <param name="trader">The trader that owns this context.</param>
    public TradeContext(ITrader trader)
    {
        this.Trader = trader ?? throw new ArgumentNullException(nameof(trader));
    }

    /// <summary>
    /// Gets the trader that owns this context.
    /// </summary>
    public ITrader Trader { get; }

    /// <summary>
    /// Gets or sets the current trading partner.
    /// </summary>
    public ITrader? TradingPartner
    {
        get => this.Trader.TradingPartner;
        set => this.Trader.TradingPartner = value;
    }

    /// <summary>
    /// Gets or sets the money which is currently in the trade.
    /// </summary>
    public int TradingMoney
    {
        get => this.Trader.TradingMoney;
        set => this.Trader.TradingMoney = value;
    }

    /// <summary>
    /// Gets a value indicating whether the trader is currently in a trade.
    /// </summary>
    public bool IsTrading => this.TradingPartner != null;

    /// <summary>
    /// Gets a value indicating whether the trader's state allows trading.
    /// </summary>
    public bool CanTrade => this.Trader.PlayerState.CurrentState == PlayerState.EnteredWorld;

    /// <summary>
    /// Gets a value indicating whether the trade is in button pressed state.
    /// </summary>
    public bool IsTradeButtonPressed => this.Trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed;

    /// <summary>
    /// Gets a value indicating whether the trade is in opened state.
    /// </summary>
    public bool IsTradeOpened => this.Trader.PlayerState.CurrentState == PlayerState.TradeOpened;

    /// <summary>
    /// Gets a value indicating whether the trader is in any trade-related state.
    /// </summary>
    public bool IsInTradeState => this.IsTradeButtonPressed || this.IsTradeOpened;

    /// <summary>
    /// Cancels the current trade if any is in progress.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask CancelTradeIfNeededAsync()
    {
        if (this.IsInTradeState)
        {
            var cancelAction = new TradeCancelAction();
            await cancelAction.CancelTradeAsync(this.Trader).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Validates that the trader can start a new trade.
    /// </summary>
    /// <param name="targetTrader">The target trading partner.</param>
    /// <returns>True if trade can be started; false otherwise.</returns>
    public bool CanStartTrade(ITrader targetTrader)
    {
        if (targetTrader == null)
        {
            return false;
        }

        if (this.IsTrading)
        {
            return false;
        }

        if (!this.CanTrade)
        {
            return false;
        }

        if (this.Trader.IsTemplatePlayer || targetTrader.IsTemplatePlayer)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Resets the trade state to default values.
    /// </summary>
    public void ResetTradeState()
    {
        this.TradingPartner = null;
        this.TradingMoney = 0;
        this.Trader.BackupInventory = null;
        this.Trader.TemporaryStorage?.Clear();
    }

    /// <summary>
    /// Creates a backup of the current inventory for rollback purposes.
    /// </summary>
    public void CreateInventoryBackup()
    {
        if (this.Trader.Inventory != null)
        {
            this.Trader.BackupInventory = new BackupItemStorage(this.Trader.Inventory.ItemStorage);
        }
    }

    /// <summary>
    /// Restores the inventory from backup if available.
    /// </summary>
    /// <returns>The task.</returns>
    public async ValueTask RestoreInventoryFromBackupAsync()
    {
        if (this.Trader.BackupInventory != null && this.Trader.Inventory != null)
        {
            this.Trader.Inventory.Clear();
            this.Trader.BackupInventory.RestoreItemStates();

            foreach (var item in this.Trader.BackupInventory.Items)
            {
                await this.Trader.Inventory.AddItemAsync(item.ItemSlot, item).ConfigureAwait(false);
            }

            this.Trader.Inventory.ItemStorage.Money = this.Trader.BackupInventory.Money;
            this.Trader.BackupInventory = null;
        }
    }

    /// <summary>
    /// Gets the total value of items in the temporary storage (trade window).
    /// </summary>
    /// <returns>The total value of items being traded.</returns>
    public long GetTradedItemsValue()
    {
        if (this.Trader.TemporaryStorage == null)
        {
            return 0;
        }

        long totalValue = 0;
        foreach (var item in this.Trader.TemporaryStorage.Items)
        {
            if (item.Definition != null)
            {
                // Basic item value calculation - could be enhanced with item calculator
                totalValue += item.Definition.Value * (item.Level + 1);
            }
        }

        return totalValue;
    }

    /// <summary>
    /// Validates that both traders have sufficient space and money for the trade.
    /// </summary>
    /// <returns>True if the trade is valid; false otherwise.</returns>
    public bool ValidateTradeItems()
    {
        if (this.TradingPartner?.TemporaryStorage == null || this.Trader.TemporaryStorage == null)
        {
            return false;
        }

        // Check if trader has enough inventory space for partner's items
        var partnerItemCount = this.TradingPartner.TemporaryStorage.Items.Count();
        var availableSlots = this.Trader.Inventory?.FreeSlots?.Count() ?? 0;

        if (partnerItemCount > availableSlots)
        {
            return false;
        }

        // Check if trader has enough money
        var totalMoneyNeeded = this.TradingPartner.TradingMoney;
        if (this.Trader.Money < totalMoneyNeeded)
        {
            return false;
        }

        return true;
    }
}