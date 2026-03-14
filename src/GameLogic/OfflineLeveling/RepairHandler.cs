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
    private readonly MuHelperPlayerConfiguration? _config;
    private readonly ItemPriceCalculator _priceCalculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepairHandler"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public RepairHandler(Player player, MuHelperPlayerConfiguration? config)
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
            this._player.Logger.LogDebug("Auto-repair is disabled by MU Helper configuration for character {0}.", this._player.Name);
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

            var item = this._player.Inventory?.GetItem(i);
            if (item is null)
            {
                continue;
            }

            var maxDurability = item.GetMaximumDurabilityOfOnePiece();
            if (maxDurability == 0 || item.Durability >= maxDurability)
            {
                continue;
            }

            // NPC discount is false because we repair "in the field"
            var price = this._priceCalculator.CalculateRepairPrice(item, false);
            this._player.Logger.LogDebug(
                "Item {0} in slot {1} needs repair. Durability: {2}/{3}. Cost: {4}. Player Zen: {5}.",
                item.Definition?.Name,
                i,
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
                    "Successfully auto-repaired item {0} in slot {1} for {2} Zen by character {3}.",
                    item.Definition?.Name,
                    i,
                    price,
                    this._player.Name);
            }
            else if (price > 0)
            {
                this._player.Logger.LogDebug(
                    "Insufficient Zen to repair item {0} in slot {1} (Cost: {2}, Zen: {3}) for character {4}.",
                    item.Definition?.Name,
                    i,
                    price,
                    this._player.Money,
                    this._player.Name);
            }
        }
    }
}
