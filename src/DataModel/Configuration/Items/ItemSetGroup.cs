// <copyright file="ItemSetGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Defines an item set group. With (partial) completion of the set, additional options are getting applied.
/// </summary>
/// <remarks>
/// With this we can define a lot of things, for example:
///   - double wear bonus of single swords
///   - set bonus for defense rate
///   - set bonus for defense, if level is greater than 9
///   - ancient sets.
/// </remarks>
[Cloneable]
public partial class ItemSetGroup
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the options of this item set always apply to an item,
    /// even if the group wasn't explicitly added to the <see cref="Item.ItemSetGroups"/>.
    /// The minimum item count and the minimum set levels are respected.
    /// </summary>
    public bool AlwaysApplies { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the items are counted distincly.
    /// </summary>
    /// <remarks>
    /// For example, for the double wear bonus this has to be non-distinct, else we wouldn't get the bonus for wearing two of the same kind of swords.
    /// </remarks>
    public bool CountDistinct { get; set; }

    /// <summary>
    /// Gets or sets the minimum item count which is needed to get the bonus.
    /// </summary>
    public int MinimumItemCount { get; set; }

    /// <summary>
    /// Gets or sets the minimum level which all of the items of the set need to have to get the bonus.
    /// </summary>
    public int SetLevel { get; set; }

    /// <summary>
    /// Gets or sets the options. If the options depend on the item count, this options need to be ordered correctly.
    /// </summary>
    /// <remarks>
    /// The order is defined by <see cref="ItemOption.Number"/>.
    /// </remarks>
    public virtual ItemOptionDefinition? Options { get; set; } = null!;

    /// <summary>
    /// Gets or sets the items of this set.
    /// </summary>
    /// <remarks>
    /// Here we can define additional bonus options, like the ancient options (e.g. +5 / +10 Str etc.).
    /// </remarks>
    [MemberOfAggregate]
    public virtual ICollection<ItemOfItemSet> Items { get; protected set; } = null!;
}