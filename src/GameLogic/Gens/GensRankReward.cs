// <copyright file="GensRankReward.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

/// <summary>
/// The monthly reward of a gens rank.
/// </summary>
public class GensRankReward
{
    /// <summary>
    /// Gets or sets the rank, from 1 (highest) to 14 (lowest).
    /// </summary>
    public byte Rank { get; set; }

    /// <summary>
    /// Gets or sets the group of the reward item.
    /// </summary>
    public byte ItemGroup { get; set; }

    /// <summary>
    /// Gets or sets the number of the reward item.
    /// </summary>
    public short ItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the count of the reward items.
    /// </summary>
    public int Count { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Rank {this.Rank}: {this.Count} x {this.ItemGroup}/{this.ItemNumber}";
    }
}
