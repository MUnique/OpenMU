// <copyright file="BuiltInItemStatProfiles.Assignment.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.ItemFormulas;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The rule which built-in profile an item gets.
/// </summary>
public static partial class BuiltInItemStatProfiles
{
    private const byte StaffGroup = 5;
    private const byte ShieldGroup = 6;
    private const byte WingAndOrbGroup = 12;
    private const byte HelperGroup = 13;
    private const byte JewelryAndPotionGroup = 14;
    private const byte ScrollGroup = 15;

    private static readonly HashSet<AttributeDefinition> StatRequirements =
    [
        Stats.TotalStrength,
        Stats.TotalAgility,
        Stats.TotalEnergy,
        Stats.TotalVitality,
        Stats.TotalLeadership,
        Stats.TotalStrengthRequirementValue,
        Stats.TotalAgilityRequirementValue,
        Stats.TotalEnergyRequirementValue,
        Stats.TotalVitalityRequirementValue,
        Stats.TotalLeadershipRequirementValue,
    ];

    /// <summary>
    /// Gets the name of the built-in profile of the item's kind, following the profile rules of the game client's
    /// item stat file with what the item data of the server tells.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The name of the item's profile.</returns>
    /// <remarks>
    /// It's the one rule for an item without a profile: the initialization data and the configuration update give
    /// the items their profile by it, and an item which is created later without one belongs to the same profile.
    /// </remarks>
    public static string GetProfileName(ItemDefinition item)
    {
        var slots = item.ItemSlot?.ItemSlots ?? [];
        if (item.IsAmmunition)
        {
            return ItemStatProfile.DefaultName;
        }

        if (HasPowerUp(item, Stats.ScepterRise))
        {
            return Sceptre;
        }

        if (HasPowerUp(item, Stats.BookRise))
        {
            return Book;
        }

        if (item.Group == ShieldGroup)
        {
            return Shield;
        }

        if (item.Group <= StaffGroup && slots.Count > 0)
        {
            return Weapon;
        }

        if (slots.Any(slot => slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot))
        {
            return Armor;
        }

        if (slots.Contains(InventoryConstants.WingsSlot))
        {
            return GetWingProfileName(item);
        }

        if (item.Group == WingAndOrbGroup && HasBaseRequirement(item, StatRequirements.Contains))
        {
            return Orb;
        }

        if (item.Group == ScrollGroup && HasBaseRequirement(item, attribute => attribute == Stats.Level || StatRequirements.Contains(attribute)))
        {
            return Scroll;
        }

        if (item.Group == ItemConstants.Fenrir.Group && item.Number == ItemConstants.Fenrir.Number)
        {
            // The game client's item data gives the Horn of Fenrir the default, although the helper rule matches it.
            return ItemStatProfile.DefaultName;
        }

        if (item.Group is HelperGroup or JewelryAndPotionGroup && HasBaseRequirement(item, attribute => attribute == Stats.Level))
        {
            return Helper;
        }

        return ItemStatProfile.DefaultName;
    }

    private static string GetWingProfileName(ItemDefinition item)
    {
        // The wings are in group 12, the Cape of Lord in group 13; their tier is the one of the game client's item data.
        return (item.Group, item.Number) switch
        {
            (WingAndOrbGroup, >= 130 and <= 135) => SmallWing,
            (WingAndOrbGroup, >= 0 and <= 2 or 41) => FirstTierWing,
            (WingAndOrbGroup, 49) or (HelperGroup, 30) => SecondTierCape,
            (WingAndOrbGroup, >= 3 and <= 6 or 42) => SecondTierWing,
            (WingAndOrbGroup, >= 36 and <= 40 or 43 or 50) => ThirdTierWing,
            _ => ItemStatProfile.DefaultName,
        };
    }

    private static bool HasPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Any(powerUp => powerUp.TargetAttribute == attribute);
    }

    /// <summary>
    /// Determines whether the item has a base requirement above 0 of one of the wanted attributes.
    /// A requirement which depends on the item level (one of its rows applies from an item level on) doesn't count,
    /// as in the game client, where such a requirement is given by level.
    /// </summary>
    private static bool HasBaseRequirement(ItemDefinition item, Func<AttributeDefinition, bool> isWanted)
    {
        return item.Requirements.Any(requirement => requirement.Attribute is { } attribute
                                                    && isWanted(attribute)
                                                    && requirement.MinimumValue > 0
                                                    && requirement.MinimumItemLevel is null
                                                    && !item.Requirements.Any(other => other.Attribute == attribute && other.MinimumItemLevel is not null));
    }
}
