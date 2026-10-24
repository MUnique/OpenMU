// <copyright file="ItemStatProfiles.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Creates the item stat profiles of a configuration and gives its items their profile.
/// Used by the initialization data of every version and by the configuration update which adds the profiles.
/// </summary>
internal static class ItemStatProfiles
{
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
    /// Adds the built-in item stat profiles which the configuration doesn't have yet (by name), and gives each item
    /// without a profile the one of its kind. Profiles and items which already have a profile stay as they are.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void Apply(IContext context, GameConfiguration gameConfiguration)
    {
        AddMissingProfiles(context, gameConfiguration);
        AssignProfiles(gameConfiguration);
    }

    /// <summary>
    /// Gets the name of the built-in profile of the item, following the profile rules of the game client's item stat file
    /// with what the item data of the server tells.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The name of the item's profile.</returns>
    internal static string GetProfileName(ItemDefinition item)
    {
        var slots = item.ItemSlot?.ItemSlots ?? [];
        if (item.IsAmmunition)
        {
            return ItemStatProfile.DefaultName;
        }

        if (HasPowerUp(item, Stats.ScepterRise))
        {
            return BuiltInItemStatProfiles.Sceptre;
        }

        if (HasPowerUp(item, Stats.BookRise))
        {
            return BuiltInItemStatProfiles.Book;
        }

        if (item.Group == (byte)ItemGroups.Shields)
        {
            return BuiltInItemStatProfiles.Shield;
        }

        if (item.Group <= (byte)ItemGroups.Staff && slots.Count > 0)
        {
            return BuiltInItemStatProfiles.Weapon;
        }

        if (slots.Any(slot => slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot))
        {
            return BuiltInItemStatProfiles.Armor;
        }

        if (slots.Contains(InventoryConstants.WingsSlot))
        {
            return GetWingProfileName(item);
        }

        if (item.Group == (byte)ItemGroups.Orbs && HasBaseRequirement(item, StatRequirements.Contains))
        {
            return BuiltInItemStatProfiles.Orb;
        }

        if (item.Group == (byte)ItemGroups.Scrolls && HasBaseRequirement(item, attribute => attribute == Stats.Level || StatRequirements.Contains(attribute)))
        {
            return BuiltInItemStatProfiles.Scroll;
        }

        if (item.Group == ItemConstants.Fenrir.Group && item.Number == ItemConstants.Fenrir.Number)
        {
            // The game client's item data gives the Horn of Fenrir the default, although the helper rule matches it.
            return ItemStatProfile.DefaultName;
        }

        if (item.Group is (byte)ItemGroups.Misc1 or (byte)ItemGroups.Misc2 && HasBaseRequirement(item, attribute => attribute == Stats.Level))
        {
            return BuiltInItemStatProfiles.Helper;
        }

        return ItemStatProfile.DefaultName;
    }

    private static void AddMissingProfiles(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var name in BuiltInItemStatProfiles.Names)
        {
            if (gameConfiguration.ItemStatProfiles.Any(profile => profile.Name == name))
            {
                continue;
            }

            var profile = context.CreateNew<ItemStatProfile>();
            profile.Name = name;
            BuiltInItemStatProfiles.Fill(
                profile,
                () => context.CreateNew<ItemStatTerms>(),
                () => context.CreateNew<ItemRequirementFormula>(),
                table => FindTable(gameConfiguration, table));
            gameConfiguration.ItemStatProfiles.Add(profile);
        }
    }

    private static void AssignProfiles(GameConfiguration gameConfiguration)
    {
        var defaultProfile = gameConfiguration.ItemStatProfiles.First(profile => profile.Name == ItemStatProfile.DefaultName);
        foreach (var item in gameConfiguration.Items.Where(item => item.StatProfile is null))
        {
            var name = GetProfileName(item);
            item.StatProfile = gameConfiguration.ItemStatProfiles.FirstOrDefault(profile => profile.Name == name) ?? defaultProfile;
        }
    }

    private static ItemLevelBonusTable? FindTable(GameConfiguration gameConfiguration, BuiltInLevelBonusTable table)
    {
        var name = table switch
        {
            BuiltInLevelBonusTable.DamageIncreaseWeapons => LocalizedString.FromResource(() => LevelBonusNames.DamageIncreaseWeapons),
            BuiltInLevelBonusTable.DefenseRateIncreaseShields => LocalizedString.FromResource(() => LevelBonusNames.DefenseRateIncreaseShields),
            BuiltInLevelBonusTable.DefenseIncreaseArmors => LocalizedString.FromResource(() => LevelBonusNames.DefenseIncreaseArmors),
            BuiltInLevelBonusTable.DefenseIncreaseShields => LocalizedString.FromResource(() => LevelBonusNames.DefenseIncreaseShields),
            BuiltInLevelBonusTable.DefenseBonus1stAnd2ndWings => LocalizedString.FromResource(() => LevelBonusNames.DefenseBonus1stAnd2ndWings),
            BuiltInLevelBonusTable.DefenseBonus3rdWings => LocalizedString.FromResource(() => LevelBonusNames.DefenseBonus3rdWings),
            _ => throw new ArgumentOutOfRangeException(nameof(table), table, null),
        };

        var neutralName = name.ValueInNeutralLanguage;
        return gameConfiguration.ItemLevelBonusTables.FirstOrDefault(t => t.Name.ValueInNeutralLanguage == neutralName);
    }

    private static string GetWingProfileName(ItemDefinition item)
    {
        // The wings are in group 12, the Cape of Lord in group 13; their tier is the one of the game client's item data.
        return (item.Group, item.Number) switch
        {
            ((byte)ItemGroups.Orbs, >= 130 and <= 135) => BuiltInItemStatProfiles.SmallWing,
            ((byte)ItemGroups.Orbs, >= 0 and <= 2 or 41) => BuiltInItemStatProfiles.FirstTierWing,
            ((byte)ItemGroups.Orbs, 49) or ((byte)ItemGroups.Misc1, 30) => BuiltInItemStatProfiles.SecondTierCape,
            ((byte)ItemGroups.Orbs, >= 3 and <= 6 or 42) => BuiltInItemStatProfiles.SecondTierWing,
            ((byte)ItemGroups.Orbs, >= 36 and <= 40 or 43 or 50) => BuiltInItemStatProfiles.ThirdTierWing,
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
