// <copyright file="ItemDropItemGroup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Defines an effect which is shown in the game, when a <see cref="ItemDropItemGroup"/> is applied.
/// </summary>
public enum ItemDropEffect
{
    /// <summary>
    /// No effect.
    /// </summary>
    Undefined,

    /// <summary>
    /// A fireworks is shown at specific coordinates.
    /// </summary>
    Fireworks,

    /// <summary>
    /// A christmas fireworks is shown at specific coordinates.
    /// </summary>
    ChristmasFireworks,

    /// <summary>
    /// A fanfare sound is played.
    /// </summary>
    FanfareSound,

    /// <summary>
    /// A swirl is shown around an object, usually the player which caused the action.
    /// </summary>
    Swirl,
}

/// <summary>
/// A <see cref="DropItemGroup"/> which acts a definition of possible items when a special item (e.g. Box of Luck) is dropped by the player.
/// </summary>
[Cloneable]
public partial class ItemDropItemGroup : DropItemGroup
{
    /// <summary>
    /// Gets or sets the <see cref="Item.Level"/> of the source item which was dropped by the player.
    /// </summary>
    public byte SourceItemLevel { get; set; }

    /// <summary>
    /// Gets or sets the amount of money, in case <see cref="DropItemGroup.ItemType"/> is <see cref="SpecialItemType.Money"/>.
    /// </summary>
    public int MoneyAmount { get; set; }

    /// <summary>
    /// Gets or sets the minimum level of the <see cref="DropItemGroup.PossibleItems"/>.
    /// </summary>
    public byte MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum level of the <see cref="DropItemGroup.PossibleItems"/>.
    /// </summary>
    public byte MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the character level which is required to drop this item.
    /// </summary>
    public short RequiredCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the effect which should be shown in the game client when this drop group is applied.
    /// </summary>
    public ItemDropEffect DropEffect { get; set; }
}