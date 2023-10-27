// <copyright file="ItemOfItemSet.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using System.Text.Json.Serialization;

/// <summary>
/// Defines additional bonus options for this item of a set.
/// </summary>
/// <remarks>
/// Here we can define additional bonus options, like the ancient options (e.g. +5 / +10 Str etc.).
/// </remarks>
[Cloneable]
public partial class ItemOfItemSet
{
    /// <summary>
    /// Gets or sets the ancient set discriminator.
    /// </summary>
    /// <remarks>
    /// Only relevant to ancient sets. One item can only be in one ancient set with the same discriminator.
    /// The original mu online protocol supports up to two different ancient sets per item - with discriminator values 1 and 2.
    /// E.g. a 'Warrior Leather' set would have a discriminator value of 1, the 'Anonymous Leather' set would have 2.
    /// </remarks>
    public int AncientSetDiscriminator { get; set; }

    /// <summary>
    /// Gets or sets the item set group to which this instance belongs.
    /// </summary>
    public virtual ItemSetGroup? ItemSetGroup { get; set; }

    /// <summary>
    /// Gets or sets the item's definition for which the bonus should apply.
    /// </summary>
    public virtual ItemDefinition? ItemDefinition { get; set; }

    /// <summary>
    /// Gets or sets the bonus option.
    /// </summary>
    public virtual IncreasableItemOption? BonusOption { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [JsonIgnore]
    public string Name => $"{this.ItemSetGroup?.Name} {this.ItemDefinition?.Name}";

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.BonusOption?.PowerUpDefinition?.ToString() ?? string.Empty;
    }
}