// <copyright file="GoldenArcherConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Configuration for the Golden Archer feature.
/// </summary>
[Cloneable]
public partial class GoldenArcherConfiguration
{
    /// <summary>
    /// Gets or sets the required renas to get a reward.
    /// </summary>
    public int RequiredRenas { get; set; } = 1;

    /// <summary>
    /// Gets or sets the zen reward from the Golden Archer.
    /// </summary>
    public int RewardZen { get; set; } = 5000000;

    /// <summary>
    /// Gets or sets the chance of dropping an item from the <see cref="RewardItems"/> list. A value between 0.0 and 1.0 (e.g. 0.5 for 50%).
    /// </summary>
    public double ItemDropChance { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets the possible items that can drop. If an item drops, one is picked randomly from this list.
    /// </summary>
    public virtual ICollection<ItemDefinition> RewardItems { get; protected set; } = new List<ItemDefinition>();

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Golden Archer Config: {this.RequiredRenas} Renas";
    }
}
