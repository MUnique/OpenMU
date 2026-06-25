// <copyright file="RepairHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Handles auto-repair of equipped items for the offline player.
/// </summary>
internal sealed class RepairHandler
{
    /// <summary>
    /// The durability health threshold (inclusive, in percent) below which a repair is triggered.
    /// Mirrors the client's <c>DEFAULT_DURABILITY_THRESHOLD</c> constant in MuHelper.cpp.
    /// </summary>
    private const int DurabilityRepairThresholdPercent = 50;

    private readonly Player _player;
    private readonly IMuHelperSettings? _config;
    private readonly ItemRepairAction _repairAction = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RepairHandler"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public RepairHandler(Player player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Performs repairs on equipped items if the configuration allows it
    /// and the item's durability is at or below <see cref="DurabilityRepairThresholdPercent"/>%.
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

            var item = this._player.Inventory?.GetItem(i);
            if (item is null)
            {
                continue;
            }

            if (!NeedsDurabilityRepair(item))
            {
                continue;
            }

            await this._repairAction.RepairItemAsync(this._player, i).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Returns <see langword="true"/> when the item's durability health is at or below
    /// <see cref="DurabilityRepairThresholdPercent"/>%, using ceiling-integer arithmetic
    /// to match the client formula: <c>iHealth = (durability * 100 + max - 1) / max</c>.
    /// </summary>
    private static bool NeedsDurabilityRepair(Item item)
    {
        var max = item.GetMaximumDurabilityOfOnePiece();
        if (max == 0)
        {
            return false;
        }

        var durabilityHealthPercent = ((int)item.Durability * 100 + max - 1) / max;
        return durabilityHealthPercent <= DurabilityRepairThresholdPercent;
    }
}
