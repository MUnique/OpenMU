// <copyright file="RepairHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Handles auto-repair of equipped items during offline leveling.
/// </summary>
internal sealed class RepairHandler
{
    private readonly Player _player;
    private readonly IMuHelperSettings? _config;
    private readonly ItemPriceCalculator _priceCalculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepairHandler"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public RepairHandler(Player player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
        this._priceCalculator = new ItemPriceCalculator();
    }

    /// <summary>
    /// Performs repairs on equipped items if the configuration allows it.
    /// </summary>
    public async ValueTask PerformRepairsAsync()
    {
        if (this._config is not { RepairItem: true })
        {
            this._player.Logger.LogDebug("Auto-repair is disabled by MU Helper configuration for character {CharacterName}.", this._player.Name);
            return;
        }

        for (byte i = InventoryConstants.FirstEquippableItemSlotIndex;
             i <= InventoryConstants.LastEquippableItemSlotIndex;
             i++)
        {
            if (i == InventoryConstants.PetSlot)
            {
                continue;
            }

            await this.RepairItemInSlotAsync(i).ConfigureAwait(false);
        }
    }

    private async ValueTask RepairItemInSlotAsync(byte slot)
    {
        var item = this._player.Inventory?.GetItem(slot);
        if (item is null)
        {
            return;
        }

        var maxDurability = item.GetMaximumDurabilityOfOnePiece();
        if (maxDurability == 0 || item.Durability >= maxDurability)
        {
            return;
        }

        // NPC discount is false because we repair "in the field"
        var price = this._priceCalculator.CalculateRepairPrice(item, false);
        this._player.Logger.LogDebug(
            "Item {ItemName} in slot {Slot} needs repair. Durability: {Durability}/{MaxDurability}. Cost: {RepairCost}. Player Zen: {PlayerZen}.",
            item.Definition?.Name,
            slot,
            item.Durability,
            maxDurability,
            price,
            this._player.Money);

        if (price > 0 && this._player.TryRemoveMoney((int)price))
        {
            item.Durability = maxDurability;
            await this._player
                .InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(item, false))
                .ConfigureAwait(false);

            this._player.Logger.LogDebug(
                "Successfully auto-repaired item {ItemName} in slot {Slot} for {RepairCost} Zen by character {CharacterName}.",
                item.Definition?.Name,
                slot,
                price,
                this._player.Name);
        }
        else if (price > 0)
        {
            this._player.Logger.LogDebug(
                "Insufficient Zen to repair item {ItemName} in slot {Slot} (Cost: {RepairCost}, Zen: {PlayerZen}) for character {CharacterName}.",
                item.Definition?.Name,
                slot,
                price,
                this._player.Money,
                this._player.Name);
        }
        else
        {
            // Price is 0 or less, no action required.
        }
    }
}
