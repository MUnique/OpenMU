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
    /// <param name="isSpawnOnAllMaps">If true, spawns on all maps in the list. If false, picks a random map.</param>
    /// <param name="x">The optional fixed X coordinate.</param>
    /// <param name="y">The optional fixed Y coordinate.</param>
    public InvasionSpawnConfiguration(ushort monsterId, ushort count, IList<ushort> mapIds, bool isSpawnOnAllMaps, byte? x = null, byte? y = null)
    {
        this.MonsterId = monsterId;
        this.Count = count;
        this.MapIds = mapIds;
        this.IsSpawnOnAllMaps = isSpawnOnAllMaps;
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
    public IList<ushort> MapIds { get; set; } = new List<ushort>();

    /// <summary>
    /// Gets or sets a value indicating whether to spawn on all maps in the list.
    /// If false, picks a random map.
    /// </summary>
    public bool IsSpawnOnAllMaps { get; set; }

    /// <summary>
    /// Gets or sets the fixed X coordinate.
    /// If null, a random coordinate is used.
    /// </summary>
    [Range(0, 255)]
    public byte? X { get; set; }

    /// <summary>
    /// Gets or sets the fixed Y coordinate.
    /// If null, a random coordinate is used.
    /// </summary>
    [Range(0, 255)]
    public byte? Y { get; set; }

    /// <summary>
    /// Determines whether this instance is equal to another <see cref="InvasionSpawnConfiguration"/>.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not InvasionSpawnConfiguration other)
        {
            return false;
        }

        return this.MonsterId == other.MonsterId
            && this.Count == other.Count
            && this.IsSpawnOnAllMaps == other.IsSpawnOnAllMaps
            && this.X == other.X
            && this.Y == other.Y
            && this.MapIds.SequenceEqual(other.MapIds);
    }

    /// <summary>
    /// Returns a hash code for this instance based on <see cref="MonsterId"/>, <see cref="Count"/>, <see cref="IsSpawnOnAllMaps"/>, <see cref="X"/>, <see cref="Y"/>, and <see cref="MapIds"/>.
    /// </summary>
    /// <returns>A hash code based on the configuration properties.</returns>
    public override int GetHashCode()
    {
        var hash = HashCode.Combine(this.MonsterId, this.Count, this.IsSpawnOnAllMaps, this.X, this.Y);
        foreach (var mapId in this.MapIds)
        {
            hash = HashCode.Combine(hash, mapId);
        }

        return hash;
    }
}