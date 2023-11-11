// <copyright file="DropItemGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Enumeration of special item types.
/// </summary>
public enum SpecialItemType
{
    /// <summary>
    /// No special item type.
    /// </summary>
    None,

    /// <summary>
    /// The ancient special item type.
    /// </summary>
    Ancient,

    /// <summary>
    /// The excellent special item type.
    /// </summary>
    Excellent,

    /// <summary>
    /// The random item special item type.
    /// </summary>
    RandomItem,

    /// <summary>
    /// The socket item special item type.
    /// </summary>
    SocketItem,

    /// <summary>
    /// The money special item type.
    /// </summary>
    Money,
}

/// <summary>
/// Idea: append several "drop item groups" with its certain probability.
/// In the drop generator sort all DropItemGroups by its chance.
/// Classes which can have DropItemGroups: Maps, Monsters(for example the kundun drops), Players(for quest items).
/// </summary>
[Cloneable]
public partial class DropItemGroup
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the chance of the item drop group to apply. From 0.0 to 1.0.
    /// </summary>
    public double Chance { get; set; }

    /// <summary>
    /// Gets or sets the minimum monster level. If <c>null</c>, then it doesn't apply.
    /// </summary>
    public byte? MinimumMonsterLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum monster level. If <c>null</c>, then it doesn't apply.
    /// </summary>
    public byte? MaximumMonsterLevel { get; set; }

    /// <summary>
    /// Gets or sets a specific monster for which this drop group is valid.
    /// If <c>null</c>, then it doesn't apply and it's valid for all monsters.
    /// </summary>
    /// <remarks>This is required for some quest items which should drop only from specific monsters.</remarks>
    public virtual MonsterDefinition? Monster { get; set; }

    /// <summary>
    /// Gets or sets the item level which will be assigned to the dropped instance of <see cref="PossibleItems"/>.
    /// </summary>
    /// <remarks>
    /// Use cases: Quest items (e.g. Broken Sword+1 = Dark Stone), Event Ticket Items, Summoning Orbs, etc. where one item type is used
    /// for multiple "visible" items.
    /// </remarks>
    public byte? ItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the special type of the item.
    /// </summary>
    public SpecialItemType ItemType { get; set; }

    /// <summary>
    /// Gets or sets the possible items which can be dropped.
    /// </summary>
    public virtual ICollection<ItemDefinition> PossibleItems { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Description;
    }
}