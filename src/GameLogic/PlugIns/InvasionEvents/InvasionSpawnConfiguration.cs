// <copyright file="InvasionSpawnConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a spawn configuration for an invasion event monster.
/// </summary>
public class InvasionSpawnConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvasionSpawnConfiguration"/> class.
    /// Required for serialization and UI binding.
    /// </summary>
    public InvasionSpawnConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvasionSpawnConfiguration"/> class.
    /// </summary>
    /// <param name="monsterId">The monster ID to spawn.</param>
    /// <param name="count">The number of monsters to spawn (1-254).</param>
    /// <param name="mapIds">The list of map IDs where the monster can spawn.</param>
    /// <param name="mapStrategy">Controls whether to spawn on a random map or all maps.</param>
    /// <param name="x">The optional fixed X coordinate.</param>
    /// <param name="y">The optional fixed Y coordinate.</param>
    public InvasionSpawnConfiguration(
        ushort monsterId,
        ushort count,
        IList<ushort> mapIds,
        SpawnMapStrategy mapStrategy,
        byte? x = null,
        byte? y = null)
    {
        this.MonsterId = monsterId;
        this.Count = count;
        this.MapIds = mapIds;
        this.MapStrategy = mapStrategy;
        this.X = x;
        this.Y = y;
    }

    /// <summary>
    /// Gets or sets the monster ID to spawn.
    /// </summary>
    [Required]
    public ushort MonsterId { get; set; }

    /// <summary>
    /// Gets or sets the number of monsters to spawn (1-254).
    /// </summary>
    [Range(1, 254)]
    public ushort Count { get; set; }

    /// <summary>
    /// Gets or sets the list of map IDs where the monster can spawn.
    /// </summary>
    [Required]
    [MinLength(1)]
    public IList<ushort> MapIds { get; set; } = [];

    /// <summary>
    /// Gets or sets the strategy used to select a map when spawning.
    /// </summary>
    public SpawnMapStrategy MapStrategy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the monster spawns on all maps in <see cref="MapIds"/>.
    /// When set to <c>true</c>, <see cref="MapStrategy"/> is changed to <see cref="SpawnMapStrategy.AllMaps"/>.
    /// When set to <c>false</c>, <see cref="MapStrategy"/> is changed to <see cref="SpawnMapStrategy.RandomMap"/>.
    /// This property exists for UI binding and serialization compatibility.
    /// </summary>
    public bool IsSpawnOnAllMaps
    {
        get => this.MapStrategy == SpawnMapStrategy.AllMaps;
        set => this.MapStrategy = value ? SpawnMapStrategy.AllMaps : SpawnMapStrategy.RandomMap;
    }

    /// <summary>
    /// Gets or sets the fixed X coordinate.
    /// If <c>null</c>, a random walkable coordinate is used.
    /// </summary>
    [Range(0, 255)]
    public byte? X { get; set; }

    /// <summary>
    /// Gets or sets the fixed Y coordinate.
    /// If <c>null</c>, a random walkable coordinate is used.
    /// </summary>
    [Range(0, 255)]
    public byte? Y { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// Equality is based solely on <see cref="MonsterId"/>, as each monster type
    /// may only have one spawn configuration per invasion event.
    /// </remarks>
    public override bool Equals(object? obj)
    {
        if (obj is not InvasionSpawnConfiguration other)
        {
            return false;
        }

        return this.MonsterId == other.MonsterId;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The hash code is based solely on <see cref="MonsterId"/>, consistent with <see cref="Equals"/>.
    /// This means the object can be safely mutated (maps, count, strategy, coordinates)
    /// while held in a hash-based collection without becoming unreachable.
    /// </remarks>
    public override int GetHashCode() => this.MonsterId.GetHashCode();
}