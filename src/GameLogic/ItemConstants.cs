// <copyright file="ItemConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// A central place to keep item identifiers, so we can keep track of them.
/// </summary>
public class ItemConstants
{
    /// <summary>
    /// Gets the identifier for the summon orb.
    /// </summary>
    public static ItemIdentifier SummonOrb => new(11, 12);

    /// <summary>
    /// Gets the identifier for the fruits.
    /// </summary>
    public static ItemIdentifier Fruits => new(15, 13);

    /// <summary>
    /// Gets the identifier for the alcohol.
    /// </summary>
    public static ItemIdentifier Alcohol => new(9, 14);

    /// <summary>
    /// Gets the identifier for the apple.
    /// </summary>
    public static ItemIdentifier Apple => new(0, 14);

    /// <summary>
    /// Gets the identifier for the small healing potion.
    /// </summary>
    public static ItemIdentifier SmallHealingPotion => new(1, 14);

    /// <summary>
    /// Gets the identifier for the medium healing potion.
    /// </summary>
    public static ItemIdentifier MediumHealingPotion => new(2, 14);

    /// <summary>
    /// Gets the identifier for the large healing potion.
    /// </summary>
    public static ItemIdentifier LargeHealingPotion => new(3, 14);

    /// <summary>
    /// Gets the identifier for the small mana potion.
    /// </summary>
    public static ItemIdentifier SmallManaPotion => new(4, 14);

    /// <summary>
    /// Gets the identifier for the medium mana potion.
    /// </summary>
    public static ItemIdentifier MediumManaPotion => new(5, 14);

    /// <summary>
    /// Gets the identifier for the large mana potion.
    /// </summary>
    public static ItemIdentifier LargeManaPotion => new(6, 14);

    /// <summary>
    /// Gets the identifier for the siege potion.
    /// </summary>
    public static ItemIdentifier SiegePotion => new(7, 14);

    /// <summary>
    /// Gets the identifier for the antidote.
    /// </summary>
    public static ItemIdentifier Antidote => new(8, 14);

    /// <summary>
    /// Gets the identifier for the town portal scroll.
    /// </summary>
    public static ItemIdentifier TownPortalScroll => new(10, 14);

    /// <summary>
    /// Gets the identifier for the jewel of bless.
    /// </summary>
    public static ItemIdentifier JewelOfBless => new(13, 14);

    /// <summary>
    /// Gets the identifier for the jewel of soul.
    /// </summary>
    public static ItemIdentifier JewelOfSoul => new(14, 14);

    /// <summary>
    /// Gets the identifier for the jewel of life.
    /// </summary>
    public static ItemIdentifier JewelOfLife => new(16, 14);

    /// <summary>
    /// Gets the identifier for the small shield potion.
    /// </summary>
    public static ItemIdentifier SmallShieldPotion => new(35, 14);

    /// <summary>
    /// Gets the identifier for the medium shield potion.
    /// </summary>
    public static ItemIdentifier MediumShieldPotion => new(36, 14);

    /// <summary>
    /// Gets the identifier for the large shield potion.
    /// </summary>
    public static ItemIdentifier LargeShieldPotion => new(37, 14);

    /// <summary>
    /// Gets the identifier for the small complex potion.
    /// </summary>
    public static ItemIdentifier SmallComplexPotion => new(38, 14);

    /// <summary>
    /// Gets the identifier for the medium complex potion.
    /// </summary>
    public static ItemIdentifier MediumComplexPotion => new(39, 14);

    /// <summary>
    /// Gets the identifier for the large complex potion.
    /// </summary>
    public static ItemIdentifier LargeComplexPotion => new(40, 14);

    /// <summary>
    /// Gets the identifier for the jewel of harmony.
    /// </summary>
    public static ItemIdentifier JewelOfHarmony => new(42, 14);

    /// <summary>
    /// Gets the identifier for the lower refine stone.
    /// </summary>
    public static ItemIdentifier LowerRefineStone => new(43, 14);

    /// <summary>
    /// Gets the identifier for the higher refine stone.
    /// </summary>
    public static ItemIdentifier HigherRefineStone => new(44, 14);

    /// <summary>
    /// Gets all scrolls.
    /// </summary>
    public static ItemIdentifier AllScrolls => new(null, 15);
}