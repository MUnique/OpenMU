// <copyright file="RepairHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Handles auto-repair of equipped items during offline leveling.
/// </summary>
internal sealed class RepairHandler
{
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

            await this._repairAction.RepairItemAsync(this._player, i).ConfigureAwait(false);
        }
    }
}
