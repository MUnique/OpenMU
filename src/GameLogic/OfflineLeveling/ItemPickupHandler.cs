// <copyright file="ItemPickupHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.OfflineLeveling;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Handles item and zen pickup for the offline leveling player.
/// </summary>
public sealed class ItemPickupHandler
{
    private const byte MinPickupRange = 1;
    private const byte JewelItemGroup = 14;

    private static readonly PickupItemAction PickupAction = new();

    private readonly OfflineLevelingPlayer _player;
    private readonly MuHelperPlayerConfiguration? _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPickupHandler"/> class.
    /// </summary>
    /// <param name="player">The offline leveling player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public ItemPickupHandler(OfflineLevelingPlayer player, MuHelperPlayerConfiguration? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Scans for and picks up items within configurable range.
    /// </summary>
    public async ValueTask PickupItemsAsync()
    {
        if (this._config is null || this._player.CurrentMap is not { } map)
        {
            return;
        }

        if (!this._config.PickAllItems
            && !this._config.PickJewel
            && !this._config.PickAncient
            && !this._config.PickZen
            && !this._config.PickExcellent)
        {
            return;
        }

        byte range = (byte)Math.Max(this._config.ObtainRange, MinPickupRange);
        var drops = map.GetDropsInRange(this._player.Position, range);

        foreach (var drop in drops)
        {
            if (drop is DroppedMoney && this._config.PickZen)
            {
                await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
            }
            else if (drop is DroppedItem droppedItem && this.ShouldPickUp(droppedItem.Item))
            {
                await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
            }
            else
            {
                // Other drops are ignored by configuration.
            }
        }
    }

    private bool ShouldPickUp(Item item)
    {
        if (this._config is null)
        {
            return false;
        }

        if (this._config.PickAllItems)
        {
            return true;
        }

        if (this._config.PickJewel && item.Definition?.Group == JewelItemGroup)
        {
            return true;
        }

        if (this._config.PickAncient && item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0))
        {
            return true;
        }

        if (this._config.PickExcellent && item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            return true;
        }

        return false;
    }
}
