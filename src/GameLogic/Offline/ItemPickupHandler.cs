// <copyright file="ItemPickupHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Offline;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Handles item and zen pickup for the offline player.
/// </summary>
public sealed class ItemPickupHandler
{
    private const byte MinPickupRange = 1;

    private static readonly PickupItemAction PickupAction = new();

    private static readonly HashSet<ItemIdentifier> Jewels =
    [
        ItemConstants.JewelOfChaos,
        ItemConstants.JewelOfBless,
        ItemConstants.JewelOfSoul,
        ItemConstants.JewelOfLife,
        ItemConstants.JewelOfCreation,
        ItemConstants.JewelOfGuardian,
        ItemConstants.Gemstone,
        ItemConstants.JewelOfHarmony,
        ItemConstants.LowerRefineStone,
        ItemConstants.HigherRefineStone,
    ];

    private readonly OfflinePlayer _player;
    private readonly IMuHelperSettings? _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPickupHandler"/> class.
    /// </summary>
    /// <param name="player">The offline player.</param>
    /// <param name="config">The MU Helper configuration.</param>
    public ItemPickupHandler(OfflinePlayer player, IMuHelperSettings? config)
    {
        this._player = player;
        this._config = config;
    }

    /// <summary>
    /// Scans for and picks up items within a configurable range.
    /// </summary>
    public async ValueTask PickupItemsAsync()
    {
        if (this._config is null || this._player.CurrentMap is not { } map)
        {
            return;
        }

        if (!this._config.PickAllItems && !this._config.PickSelectItems)
        {
            return;
        }

        byte range = (byte)Math.Max(this._config.ObtainRange, MinPickupRange);
        var drops = map.GetDropsInRange(this._player.Position, range);

        foreach (var drop in drops)
        {
            if (this.ShouldPickUpDrop(drop))
            {
                await PickupAction.PickupItemAsync(this._player, drop.Id).ConfigureAwait(false);
            }
        }
    }

    private static bool IsJewel(Item item)
    {
        if (item.Definition is not { } definition)
        {
            return false;
        }

        return Jewels.Contains(new(definition.Number, definition.Group));
    }

    private bool ShouldPickUpDrop(IIdentifiable drop)
    {
        if (this._config!.PickAllItems)
        {
            return true;
        }

        if (!this._config.PickSelectItems)
        {
            return false;
        }

        if (drop is DroppedMoney && this._config.PickZen)
        {
            return true;
        }

        if (drop is DroppedItem droppedItem)
        {
            return this.ShouldPickUp(droppedItem.Item);
        }

        return false;
    }

    private bool ShouldPickUp(Item item)
    {
        if (this._config is null)
        {
            return false;
        }

        if (this._config.PickJewel && IsJewel(item))
        {
            return true;
        }

        var isAncient = item.ItemSetGroups.Any(s => s.AncientSetDiscriminator != 0);
        var isExcellent = item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent);
        if ((this._config.PickAncient && isAncient) || (this._config.PickExcellent && isExcellent))
        {
            // A human's helper hoards every excellent/ancient piece - its owner sorts the treasure
            // out later. A bot has no later: it cannot trade, so it only takes what it can actually
            // wear as an upgrade; everything else would silt up its backpack until the loot pickup
            // stops.
            return this._player.Account?.IsBot != true
                   || Bots.BotEquipmentHandler.IsUpgradeFor(this._player, item);
        }

        if (this._config.PickUpgradeItems && Bots.BotEquipmentHandler.IsUpgradeFor(this._player, item))
        {
            // The item is class-qualified gear which beats what the bot currently wears - worth picking
            // up; the BotEquipmentHandler will equip it on one of its next passes.
            return true;
        }

        if (this._config.PickExtraItems && item.Definition is { } definition)
        {
            return this._config.ExtraItemNames.Any(name => definition.Name.ToString()?.Contains(name, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        return false;
    }
}
