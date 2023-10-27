// <copyright file="ItemOptionDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;

/// <summary>
/// The definition of an item option.
/// </summary>
[Cloneable]
public partial class ItemOptionDefinition
{
    /// <summary>
    /// Gets or sets the name of the option, for example "Luck", "Skill", "Normal Option".
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this option adds randomly.
    /// </summary>
    public bool AddsRandomly { get; set; }

    /// <summary>
    /// Gets or sets the add chance if this option adds randomly.
    /// </summary>
    public float AddChance { get; set; }

    /// <summary>
    /// Gets or sets the maximum options per item when it adds randomly by drop.
    /// </summary>
    /// <remarks>
    /// Usually this is 1. But for some options (e.g. excellent) this can be bigger than 1.
    /// </remarks>
    public int MaximumOptionsPerItem { get; set; }

    /// <summary>
    /// Gets or sets the possible options.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<IncreasableItemOption> PossibleOptions { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name;
    }
}