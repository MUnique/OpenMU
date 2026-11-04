// <copyright file="ItemOptionLink.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// This class defines a link between the item and the concrete item option which the actual item instance possess.
/// </summary>
[Cloneable]
public partial class ItemOptionLink
{
    /// <summary>
    /// Gets or sets the item option.
    /// Link to <see cref="ItemDefinition.PossibleItemOptions"/>, <see cref="ItemOptionDefinition.PossibleOptions"/>.
    /// </summary>
    [Required]
    public virtual IncreasableItemOption? ItemOption { get; set; }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the index of the option. This is required when the options are sorted, e.g. for socket options.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>The cloned instance.</returns>
    public virtual ItemOptionLink Clone()
    {
        var link = new ItemOptionLink();
        link.AssignValues(this);
        return link;
    }

    /// <summary>
    /// Assigns the values.
    /// </summary>
    /// <param name="otherLink">The other link.</param>
    public void AssignValues(ItemOptionLink otherLink)
    {
        this.ItemOption = otherLink.ItemOption;
        this.Level = otherLink.Level;
        this.Index = otherLink.Index;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var powerUpDefinition = this.ItemOption?.LevelDependentOptions?.FirstOrDefault(ldo => ldo.Level == this.Level)?.PowerUpDefinition
                                ?? this.ItemOption?.PowerUpDefinition;
        return powerUpDefinition?.ToString() ?? "empty";
    }
}