// <copyright file="MerchantNpc.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

/// <summary>
/// The implemented class of a merchant, which is a non-player-character which
/// can be observed by players and has a storage for items which are offered to sell.
/// </summary>
public class MerchantNpc : NonPlayerCharacter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MerchantNpc"/> class.
    /// </summary>
    /// <param name="spawnInfo">The spawn information.</param>
    /// <param name="stats">The stats.</param>
    /// <param name="map">The map on which this instance will spawn.</param>
    /// <param name="logger">The logger, which logs a warning for each item of the store which finds no place.</param>
    public MerchantNpc(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map, ILogger? logger = null)
        : base(spawnInfo, stats, map)
    {
        ArgumentNullException.ThrowIfNull(stats.MerchantStore, nameof(stats.MerchantStore));

        this.MerchantStorage = new Storage(
            InventoryConstants.RowSize * InventoryConstants.WarehouseRows,
            0,
            0,
            stats.MerchantStore,
            item => LeaveOutItemWithoutPlace(item, stats, logger));
    }

    /// <summary>
    /// Gets the merchant storage.
    /// </summary>
    public Storage MerchantStorage { get; }

    /// <inheritdoc/>
    protected override bool CanSpawnInSafezone => true;

    /// <summary>
    /// Leaves an item of the store which finds no place, e.g. because the size of its definition grew,
    /// out of the <see cref="MerchantStorage"/>, instead of failing the creation of the merchant and its map.
    /// The item stays in the store of the database, so that an administrator can move or remove it.
    /// </summary>
    /// <param name="item">The item which finds no place.</param>
    /// <param name="merchant">The definition of the merchant.</param>
    /// <param name="logger">The logger.</param>
    /// <returns><c>false</c>, so that the item stays in the store of the database.</returns>
    private static bool LeaveOutItemWithoutPlace(Item item, MonsterDefinition merchant, ILogger? logger)
    {
        logger?.LogWarning(
            "The item {item} at slot {slot} of the store of merchant {merchant} finds no place, e.g. because its size grew. Move or remove it in the store of the merchant.",
            item,
            item.ItemSlot,
            merchant);
        return false;
    }
}