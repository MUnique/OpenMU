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
    /// <exception cref="MerchantNpc">MerchantStore</exception>
    public MerchantNpc(MonsterSpawnArea spawnInfo, MonsterDefinition stats, GameMap map)
        : base(spawnInfo, stats, map)
    {
        ArgumentNullException.ThrowIfNull(stats.MerchantStore, nameof(stats.MerchantStore));

        this.MerchantStorage = new Storage(InventoryConstants.RowSize * InventoryConstants.WarehouseRows, stats.MerchantStore);
    }

    /// <summary>
    /// Gets the merchant storage.
    /// </summary>
    public Storage MerchantStorage { get; }

    /// <inheritdoc/>
    protected override bool CanSpawnInSafezone => true;
}