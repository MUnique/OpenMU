// <copyright file="IncreasableItemOption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines by which "level" the option is increased with <see cref="IncreasableItemOption.LevelDependentOptions"/>.
/// </summary>
public enum LevelType
{
    /// <summary>
    /// It's increased by the option level.
    /// </summary>
    /// <remarks>
    /// This one is used by item options which can be increased by separate jewels, e.g. Jewel of Life or Jewel of Harmony.
    /// </remarks>
    OptionLevel,

    /// <summary>
    /// It's increased by the level of the item which has the option.
    /// </summary>
    /// <remarks>
    /// As far as I know, this is only required for wing options, e.g. 'Increase max HP +50~125'. That's why <see cref="OptionLevel"/> is the default, too.
    /// </remarks>
    ItemLevel,
}

/// <summary>
/// Defines an item option which can be increased.
/// </summary>
[Cloneable]
public partial class IncreasableItemOption : ItemOption
{
    /// <summary>
    /// Gets or sets a value which defines by which "level" the option is increased with <see cref="LevelDependentOptions"/>.
    /// </summary>
    /// <value>
    /// The type of the level.
    /// </value>
    public LevelType LevelType { get; set; }

    /// <summary>
    /// Gets or sets a value which is considered when randomizing the option to an item.
    /// </summary>
    /// <remarks>This is used for Jewel of Harmony options rollout.</remarks>
    /// <value>The statistical weight.</value>
    public byte Weight { get; set; }

    /// <summary>
    /// Gets or sets the level dependent options for option levels over 1.
    /// </summary>
    [MemberOfAggregate]
    public virtual ICollection<ItemOptionOfLevel> LevelDependentOptions { get; protected set; } = null!;
}