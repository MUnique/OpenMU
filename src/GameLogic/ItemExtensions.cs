// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Extension methods for <see cref="Item"/>.
/// </summary>
public static class ItemExtensions
{
    private const byte StaffItemGroup = 5;
    private const byte ShieldItemGroup = 6;

    private static readonly byte[] AdditionalDurabilityPerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };

    private static readonly float[] DurabilityFactors = { 0.2f, 0.3f, 0.5f };

    private static readonly IDictionary<AttributeDefinition, AttributeDefinition> RequirementAttributeMapping = new Dictionary<AttributeDefinition, AttributeDefinition>
    {
        { Stats.TotalStrengthRequirementValue, Stats.TotalStrength },
        { Stats.TotalAgilityRequirementValue, Stats.TotalAgility },
        { Stats.TotalEnergyRequirementValue, Stats.TotalEnergy },
        { Stats.TotalVitalityRequirementValue, Stats.TotalVitality },
        { Stats.TotalLeadershipRequirementValue, Stats.TotalLeadership },
    };

    private static readonly IDictionary<AttributeDefinition, AttributeDefinition> RequirementReductionAttributeMapping = new Dictionary<AttributeDefinition, AttributeDefinition>
    {
        { Stats.TotalStrengthRequirementValue, Stats.RequiredStrengthReduction },
        { Stats.TotalAgilityRequirementValue, Stats.RequiredAgilityReduction },
        { Stats.TotalEnergyRequirementValue, Stats.RequiredEnergyReduction },
        { Stats.TotalVitalityRequirementValue, Stats.RequiredVitalityReduction },
        { Stats.TotalLeadershipRequirementValue, Stats.RequiredLeadershipReduction },
    };

    /// <summary>
    /// Gets the maximum durability of the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The maximum durability of the item.</returns>
    /// <remarks>
    /// I think this is more like the durability which can be dropped.
    /// Some items can be stacked up to 255 pieces, which increases the durability value.
    /// </remarks>
    public static byte GetMaximumDurabilityOfOnePiece(this Item item)
    {
        if (!item.IsWearable())
        {
            // Items which are not wearable don't have a "real" durability. If the item is stackable, durability means number of pieces in this case
            return 1;
        }

        if (item.IsTrainablePet())
        {
            return 255;
        }

        var result = item.Definition!.Durability + AdditionalDurabilityPerLevel[item.Level];
        if (item.IsAncient())
        {
            result += 20;
        }
        else if (item.IsExcellent())
        {
            // TODO: archangel weapons, but I guess it's not a big issue if we don't, because of their already high durability
            result += 15;
        }
        else
        {
            // there are no other options which increase the durability.
            // It might be nice to add the magic values above to the ItemOptionType, as data.
        }

        return (byte)Math.Min(byte.MaxValue, result);
    }

    /// <summary>
    /// Determines whether this instance is ancient.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is ancient; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAncient(this Item item)
    {
        return item.ItemSetGroups.Any(itemSet => itemSet.AncientSetDiscriminator > 0);
    }

    /// <summary>
    /// Determines whether this instance is excellent.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is excellent; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsExcellent(this Item item)
    {
        return item.ItemOptions.Any(link => link.ItemOption?.OptionType == ItemOptionTypes.Excellent);
    }

    /// <summary>
    /// Determines whether this instance is a "380 item", that is, if it can be upgraded with Jewel of Guardian.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a "380 item"; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsGuardian(this Item item)
    {
        return item.Definition!.PossibleItemOptions.Any(pio => pio.PossibleOptions
            .Any(po => po.OptionType == ItemOptionTypes.GuardianOption));
    }

    /// <summary>
    /// Determines whether this item is a defensive item (armor or shield).
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><see langword="true"/>, if the item is defensive.</returns>
    public static bool IsDefensiveItem(this Item item)
    {
        return IsArmorItem(item) || item.IsShield();
    }

    /// <summary>
    /// Determines whether this instance is a shield.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a shield; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsShield(this Item item)
    {
        return item.Definition?.Group == ShieldItemGroup;
    }

    /// <summary>
    /// Determines whether equipping an item of this definition into the given hand slot would conflict
    /// with what the other hand already holds: a two-handed item needs the other hand free (ammunition
    /// aside), and nothing but ammunition fits next to an already equipped two-handed item.
    /// </summary>
    /// <param name="itemDefinition">The definition of the item which would be equipped.</param>
    /// <param name="inventory">The inventory to check the other hand in.</param>
    /// <param name="toSlot">The hand slot the item would be equipped into.</param>
    /// <returns><c>true</c> if the other hand blocks this item; otherwise, <c>false</c>.</returns>
    public static bool ConflictsWithEquippedHands(this ItemDefinition itemDefinition, IStorage inventory, byte toSlot)
    {
        if (itemDefinition.ItemSlot is null)
        {
            return false;
        }

        static bool IsOneHandedOrShield(ItemDefinition definition) =>
            (definition.ItemSlot!.ItemSlots.Contains(InventoryConstants.RightHandSlot) && definition.ItemSlot.ItemSlots.Contains(InventoryConstants.LeftHandSlot))
            || definition.Group == ShieldItemGroup;

        var rightHandItemDefinition = inventory.GetItem(InventoryConstants.RightHandSlot)?.Definition;

        return (toSlot == InventoryConstants.LeftHandSlot
                && itemDefinition.Width >= 2
                && rightHandItemDefinition is not null
                && !rightHandItemDefinition.IsAmmunition)
               || (toSlot == InventoryConstants.RightHandSlot
                   && IsOneHandedOrShield(itemDefinition)
                   && inventory.GetItem(InventoryConstants.LeftHandSlot)?.Definition?.Width >= 2);
    }

    /// <summary>
    /// Determines whether this item is a jewelry (pendant or ring) item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is jewelry; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsJewelry(this Item item)
    {
        return item.ItemSlot >= InventoryConstants.PendantSlot && item.ItemSlot <= InventoryConstants.Ring2Slot;
    }

    /// <summary>
    /// Determines whether this item is a transformation ring.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a transformation ring; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTransformationRing(this Item item)
    {
        return item.Definition?.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.TransformationSkin) ?? false;
    }

    /// <summary>
    /// Determines whether this item is an armor item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is armor; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsArmorItem(this Item item)
    {
        return item.Definition?.ItemSlot?.ItemSlots.Any(slot =>
            slot >= InventoryConstants.HelmSlot && slot <= InventoryConstants.BootsSlot) ?? false;
    }

    /// <summary>
    /// Determines whether this instance is a is weapon which deals physical damage.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="minimumDmg">The minimum physical damage of the weapon.</param>
    /// <returns>
    ///   <c>true</c> if this instance is a weapon which deals physical damage; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPhysicalWeapon(this Item item, [NotNullWhen(true)] out float? minimumDmg)
    {
        minimumDmg = item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon)?.BaseValue;
        return minimumDmg is not null;
    }

    /// <summary>
    /// Determines whether this instance is a weapon which increases wizardry damage.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="staffRise">The staff/sword/stick's wizardry damage rise percentage.</param>
    /// <returns>
    ///   <c>true</c> if this instance is a weapon which increases wizardry damage; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWizardryWeapon(this Item item, [NotNullWhen(true)] out float? staffRise)
    {
        staffRise = item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.StaffRise)?.BaseValue;
        return staffRise is not null;
    }

    /// <summary>
    /// Determines whether this instance is a scepter which increases raven damage.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="scepterRise">The scepter's pet attack rise percentage.</param>
    /// <returns>
    ///   <c>true</c> if this instance is a scepter which increases raven damage; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsScepter(this Item item, [NotNullWhen(true)] out float? scepterRise)
    {
        scepterRise = item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.ScepterRise)?.BaseValue;
        return scepterRise is not null;
    }

    /// <summary>
    /// Determines whether this instance is a book which increases curse damage.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="bookRise">The book's curse damage rise percentage.</param>
    /// <returns>
    ///   <c>true</c> if this instance is a book which increases curse damage; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBook(this Item item, [NotNullWhen(true)] out float? bookRise)
    {
        bookRise = item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.BookRise)?.BaseValue;
        return bookRise is not null;
    }

    /// <summary>
    /// Determines whether this instance's durability decays with time.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if this instance's durability decays with time; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTimeDecaying(this Item item)
    {
        return item.IsJewelry() || item.IsWing();
    }

    /// <summary>
    /// Determines whether this instance's power-ups depend on its current durability.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if this instance's power-ups are subject to a durabiity factor; otherwise, <c>false</c>.
    /// </returns>
    public static bool DegradesWithDurability(this Item item)
    {
        return item.ItemSlot != InventoryConstants.PetSlot && !item.IsTrainablePet() && !item.IsTransformationRing();
    }

    /// <summary>
    /// Returns a random equipped weapon from the storage.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <param name="damageType">The damage type.</param>
    /// <returns>A randomly selected offensive item.</returns>
    public static Item? GetRandomWeapon(this IInventoryStorage storage, DamageType damageType)
    {
        if (damageType == DamageType.Fenrir)
        {
            return null;
        }

        var left = storage.GetItem(InventoryConstants.LeftHandSlot);
        var right = storage.GetItem(InventoryConstants.RightHandSlot);

        if (left?.Definition?.IsAmmunition ?? false)
        {
            left = null;
        }

        if ((right?.Definition?.IsAmmunition ?? false)
            || right?.Definition?.Group == ShieldItemGroup
            || (right?.IsTrainablePet() ?? false))
        {
            right = null;
        }

        if (damageType is DamageType.Wizardry or DamageType.Curse)
        {
            if (!(left?.IsWizardryWeapon(out _) ?? true) || (left?.CanHaveSkill() ?? false))
            {
                left = null;
            }

            if (!right?.IsBook(out _) ?? true)
            {
                right = null;
            }
        }
        else
        {
            if (left?.Definition?.Group == StaffItemGroup || right?.Definition?.Group == StaffItemGroup)
            {
                right = null;
            }
        }

        var result = left ?? right;
        if (result is null)
        {
            return null;
        }

        switch (Rand.NextInt(0, 2))
        {
            case 0 when left is { }:
                result = left;
                break;
            case 1 when right is { }:
                result = right;
                break;
            default:
                // keep first available result
                break;
        }

        return result;
    }

    /// <summary>
    /// Gets the requirement as a tuple of an attribute and the corresponding value.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="requirement">The requirement.</param>
    /// <param name="gameConfiguration">The game configuration, whose item stat profiles hold the requirement formulas.</param>
    /// <returns>The requirement as a tuple of an attribute and the corresponding value.</returns>
    /// <remarks>
    /// The stat requirements of an item which can be worn grow with its drop level, its item level and its option, by the
    /// requirement formulas of its item stat profile (see <see cref="ResolvedItemStatProfile"/>); the level requirement
    /// grows by the profile's level growth.
    /// </remarks>
    public static (AttributeDefinition Attr, int Value) GetRequirement(this Item item, AttributeRequirement requirement, GameConfiguration gameConfiguration)
    {
        ResolvedItemStatProfile? profile = null;
        return item.GetRequirement(requirement, gameConfiguration, ref profile);
    }

    /// <summary>
    /// Gets the requirements of the item, each as a tuple of an attribute and the corresponding value (see
    /// <see cref="GetRequirement(Item, AttributeRequirement, GameConfiguration)"/>).
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameConfiguration">The game configuration, whose item stat profiles hold the requirement formulas.</param>
    /// <returns>The requirements of the item.</returns>
    /// <remarks>The item stat profile of the item is resolved only once for all its requirements.</remarks>
    public static IReadOnlyList<(AttributeDefinition Attr, int Value)> GetRequirements(this Item item, GameConfiguration gameConfiguration)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));
        ResolvedItemStatProfile? profile = null;
        var requirements = new List<(AttributeDefinition Attr, int Value)>(item.Definition.Requirements.Count);
        foreach (var requirement in item.Definition.Requirements)
        {
            requirements.Add(item.GetRequirement(requirement, gameConfiguration, ref profile));
        }

        return requirements;
    }

    /// <summary>
    /// Gets the item data which is relevant for the visual appearance of an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The item data which is relevant for the visual appearance of an item.</returns>
    public static ItemAppearance GetAppearance(this Item item)
    {
        var appearance = new TemporaryItemAppearance
        {
            Definition = item.Definition,
            ItemSlot = item.ItemSlot,
            Level = item.Level,
        };
        item.ItemOptions
            .Where(option => option.ItemOption?.OptionType is not null && option.ItemOption.OptionType.IsVisible)
            .Select(option => option.ItemOption!.OptionType!)
            .Distinct()
            .ForEach(appearance.VisibleOptions.Add);
        if (item.IsAncient())
        {
            // 1. The ancient option is not included in the item.ItemOptions.
            // 2. The bonus option is not always existing for ancient items. And it's not marked as visible.
            // -> we check it based on the item set group.
            appearance.VisibleOptions.Add(ItemOptionTypes.AncientOption);
        }

        return appearance;
    }

    /// <summary>
    /// Creates a persistent instance of the given <see cref="ItemAppearance"/> and returns it.
    /// </summary>
    /// <param name="itemAppearance">The item appearance.</param>
    /// <param name="persistenceContext">The persistence context where the object should be added.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>A persistent instance of the given <see cref="ItemAppearance"/>.</returns>
    public static ItemAppearance MakePersistent(this ItemAppearance itemAppearance, IContext persistenceContext, GameConfiguration gameConfiguration)
    {
        var persistent = persistenceContext.CreateNew<ItemAppearance>();
        persistent.ItemSlot = itemAppearance.ItemSlot;
        persistent.Definition = itemAppearance.Definition;
        persistent.Level = itemAppearance.Level;
        itemAppearance.VisibleOptions.Distinct().ForEach(o => persistent.VisibleOptions.Add(gameConfiguration.ItemOptionTypes.First(iot => iot.Equals(o))));
        return persistent;
    }

    /// <summary>
    /// Calculates the drop level which the item formulas use: the drop level of the item definition, plus the
    /// <see cref="GameConfiguration.AncientDropLevelBonus"/> for an ancient item, or else the
    /// <see cref="GameConfiguration.ExcellentDropLevelBonus"/> for an excellent one.
    /// </summary>
    /// <param name="item">The item definition.</param>
    /// <param name="isAncient">A value indicating whether the item is ancient.</param>
    /// <param name="isExcellent">A value indicating whether the item is excellent.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The calculated drop level, without the item level.</returns>
    public static int CalculateDropLevel(this ItemDefinition item, bool isAncient, bool isExcellent, GameConfiguration gameConfiguration)
    {
        if (isAncient)
        {
            return item.DropLevel + gameConfiguration.AncientDropLevelBonus;
        }

        return isExcellent ? item.DropLevel + gameConfiguration.ExcellentDropLevelBonus : item.DropLevel;
    }

    /// <summary>
    /// Gets the current durability factor for the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <remarks>
    /// Directly related to the client's UI item durability icons, whose colors span yellow (&lt;50%), orange (&lt;30%), light red (&lt;20%) and red (exhausted).
    /// </remarks>
    /// <returns>The factor.</returns>
    public static float GetCurrentDurabilityFactor(this Item item)
    {
        if (item.Durability == 0)
        {
            return 0;
        }

        if (!item.DegradesWithDurability())
        {
            return 1;
        }

        var maxDurability = item.GetMaximumDurabilityOfOnePiece();
        for (int i = 0; i < DurabilityFactors.Length; i++)
        {
            if (item.Durability < maxDurability * DurabilityFactors[i])
            {
                return 1 - DurabilityFactors[^(i + 1)];
            }
        }

        return 1;
    }

    /// <summary>
    /// Gets the requirement, and resolves the item stat profile of the item into <paramref name="profile"/> when it's
    /// needed and not resolved yet.
    /// </summary>
    private static (AttributeDefinition Attr, int Value) GetRequirement(this Item item, AttributeRequirement requirement, GameConfiguration gameConfiguration, ref ResolvedItemStatProfile? profile)
    {
        requirement.ThrowNotInitializedProperty(requirement.Attribute is null, nameof(requirement.Attribute));

        var minimumValue = requirement.MinimumItemLevel is { } minimumItemLevel && item.Level < minimumItemLevel
            ? 0
            : requirement.MinimumValue;

        if (requirement.Attribute == Stats.Level)
        {
            return (requirement.Attribute, item.CalculateLevelRequirement(minimumValue, gameConfiguration, ref profile));
        }

        if (!RequirementAttributeMapping.TryGetValue(requirement.Attribute, out var totalAttribute))
        {
            return (requirement.Attribute, minimumValue);
        }

        if (!item.IsWearable())
        {
            return (totalAttribute, minimumValue);
        }

        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));
        profile ??= ResolvedItemStatProfile.Of(item.Definition, gameConfiguration);
        int value;
        if (profile.GetRequirementFormula(requirement.Attribute) is not { KeepBase: false } formula)
        {
            // Without a formula, or with "keep base", the requirement doesn't grow; the option and the reduction still
            // apply, as in the game client.
            value = minimumValue;
        }
        else
        {
            value = minimumValue == 0 ? 0 : item.CalculateRequirement(formula, minimumValue, gameConfiguration);
        }

        if (value > 0 && totalAttribute == Stats.TotalStrength)
        {
            var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
            if (itemOption != null)
            {
                value = LimitToInt(value + ((long)itemOption.Level * profile.StrengthPerOptionLevel));
            }
        }

        if (RequirementReductionAttributeMapping.TryGetValue(requirement.Attribute, out var reductionAttribute)
            && item.ItemOptions.FirstOrDefault(o =>
                    o.ItemOption?.PowerUpDefinition?.TargetAttribute == reductionAttribute
                    || (o.ItemOption?.LevelDependentOptions.Any(l => l.PowerUpDefinition?.TargetAttribute == reductionAttribute) ?? false))
                is { } reductionOption)
        {
            var optionOfLevelPowerUp = reductionOption.ItemOption!.LevelDependentOptions
                                           .FirstOrDefault(o => o.Level == reductionOption.Level)?.PowerUpDefinition
                                       ?? reductionOption.ItemOption.PowerUpDefinition;
            if (optionOfLevelPowerUp?.Boost?.ConstantValue is { } reduction)
            {
                value -= (int)reduction.Value;
            }
        }

        return (totalAttribute, value);
    }

    /// <summary>
    /// Calculates a stat requirement by its formula: <c>Add + base × (drop level + item level × LevelWeight) × Percent ÷ 100</c>,
    /// or with <see cref="ItemRequirementFormula.LevelRequirementBasis"/> and a level requirement of the item
    /// <c>Add + base × level requirement × Percent ÷ 100</c>. That level requirement is the stored one of the item
    /// definition, without its growth per item level and the excellent addition, as in the game client.
    /// </summary>
    private static int CalculateRequirement(this Item item, ItemRequirementFormula formula, int baseValue, GameConfiguration gameConfiguration)
    {
        var definition = item.Definition!;
        if (formula.LevelRequirementBasis
            && definition.Requirements.FirstOrDefault(r => r.Attribute == Stats.Level && r.MinimumItemLevel is null)?.MinimumValue is > 0 and var levelRequirement)
        {
            return LimitToInt(formula.Add + ((Int128)baseValue * levelRequirement * formula.Percent / 100));
        }

        var dropLevel = definition.CalculateDropLevel(item.IsAncient(), item.IsExcellent(), gameConfiguration) + ((Int128)item.Level * formula.LevelWeight);
        return LimitToInt(formula.Add + (baseValue * dropLevel * formula.Percent / 100));
    }

    /// <summary>
    /// Calculates the level requirement: a base value above 0 grows by the profile's level growth per item level (a
    /// trainable pet counts as level 0), and an excellent item with <see cref="ItemDefinition.ExcellentRaisesLevel"/>
    /// needs the profile's <see cref="ItemStatProfile.ExcellentLevelAdd"/> more.
    /// </summary>
    private static int CalculateLevelRequirement(this Item item, int baseValue, GameConfiguration gameConfiguration, ref ResolvedItemStatProfile? profile)
    {
        if (baseValue == 0 || item.Definition is not { } definition)
        {
            return baseValue;
        }

        profile ??= ResolvedItemStatProfile.Of(definition, gameConfiguration);
        var value = baseValue + ((long)profile.LevelPerItemLevel * (item.IsTrainablePet() ? 0 : item.Level));
        if (definition.ExcellentRaisesLevel && item.IsExcellent())
        {
            value += profile.ExcellentLevelAdd;
        }

        return LimitToInt(value);
    }

    /// <summary>
    /// Limits a value of the item formulas, which an owner can configure, to the range of <see cref="int"/>, so that a
    /// large factor doesn't wrap to a negative requirement. <see cref="Int128"/> holds the product of three values of
    /// <see cref="int"/>.
    /// </summary>
    private static int LimitToInt(Int128 value) => (int)Int128.Clamp(value, int.MinValue, int.MaxValue);

    private sealed class TemporaryItemAppearance : ItemAppearance
    {
        public override ICollection<ItemOptionType> VisibleOptions => base.VisibleOptions ??= new List<ItemOptionType>();
    }
}