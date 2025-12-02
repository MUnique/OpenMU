// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Extension methods for <see cref="Item"/>.
/// </summary>
public static class ItemExtensions
{
    private const byte ShieldItemGroup = 6;

    private static readonly byte[] AdditionalDurabilityPerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };

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
    /// Determines whether this item is a defensive item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><see langword="true"/>, if the item is defensive.</returns>
    public static bool IsDefensiveItem(this Item item)
    {
        return InventoryConstants.IsDefenseItemSlot(item.ItemSlot) || item.IsShield();
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
    /// Returns a random offensive item of the storage.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <returns>A randomly selected offensive item.</returns>
    public static Item? GetRandomOffensiveItem(this IInventoryStorage storage)
    {
        var left = storage.GetItem(InventoryConstants.LeftHandSlot);
        var right = storage.GetItem(InventoryConstants.RightHandSlot);
        var pendant = storage.GetItem(InventoryConstants.PendantSlot);

        if ((left?.Definition?.IsAmmunition ?? false)
            || left?.Definition?.Group == ShieldItemGroup)
        {
            left = null;
        }

        if ((right?.Definition?.IsAmmunition ?? false)
            || right?.Definition?.Group == ShieldItemGroup)
        {
            right = null;
        }

        var random = Rand.NextInt(3, 6);
        var result = left ?? right ?? pendant;
        if (result is null)
        {
            return null;
        }

        switch (random % 3)
        {
            case 0 when left is { }: result = left;
                break;
            case 1 when right is { }: result = right;
                break;
            case 2 when pendant is { }: result = pendant;
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
    /// <returns>The requirement as a tuple of an attribute and the corresponding value.</returns>
    /// <remarks>
    /// Some requirements are depending on item level, drop level and item options.
    /// </remarks>
    public static (AttributeDefinition Attr, int Value) GetRequirement(this Item item, AttributeRequirement requirement)
    {
        requirement.ThrowNotInitializedProperty(requirement.Attribute is null, nameof(requirement.Attribute));

        if (RequirementAttributeMapping.TryGetValue(requirement.Attribute, out var totalAttribute))
        {
            if (!item.IsWearable())
            {
                return (totalAttribute, requirement.MinimumValue);
            }

            var multiplier = 3;
            if (totalAttribute == Stats.TotalEnergy)
            {
                multiplier = 4;

                // Summoner Books are calculated differently. They are in group 5 (staffs) and are the only items in the group which can have skill.
                if (item.Definition?.Skill != null && item.Definition.Group == 5)
                {
                    return (totalAttribute, item.CalculateBookEnergyRequirement(requirement.MinimumValue));
                }
            }

            var value = item.CalculateRequirement(requirement.MinimumValue, multiplier);
            if (value > 0 && totalAttribute == Stats.TotalStrength)
            {
                var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
                if (itemOption != null)
                {
                    value += itemOption.Level * 4;
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

        return (requirement.Attribute, requirement.MinimumValue);
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
    /// Calculates the drop level.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="isAncient">A value indicating whether the item is ancient.</param>
    /// <param name="isExcellent">A value indicating whether the item is excellent.</param>
    /// <param name="itemLevel">The item level.</param>
    /// <returns>The calculated drop level.</returns>
    public static int CalculateDropLevel(this ItemDefinition item, bool isAncient, bool isExcellent, int itemLevel)
    {
        int dropLevel = item.DropLevel;
        if (isAncient)
        {
            dropLevel += 30;
        }
        else if (isExcellent)
        {
            dropLevel += 25;
        }
        else
        {
            // nothing to add
        }

        dropLevel += 3 * itemLevel;
        return dropLevel;
    }

    /// <summary>
    /// Calculates the drop level of the item in the current state.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The calculated drop level.</returns>
    public static int CalculateDropLevel(this Item item)
    {
        return item.Definition?.CalculateDropLevel(item.IsAncient(), item.IsExcellent(), item.Level) ?? 0;
    }

    private static int CalculateRequirement(this Item item, int requirementValue, int multiplier)
    {
        if (requirementValue == 0)
        {
            return 0;
        }

        var dropLevel = item.Definition!.CalculateDropLevel(item.IsAncient(), item.IsExcellent(), item.Level);

        return (multiplier * dropLevel * requirementValue / 100) + 20;
    }

    private static int CalculateBookEnergyRequirement(this Item item, int energyRequirementValue)
    {
        var dropLevel = item.Definition!.CalculateDropLevel(item.IsAncient(), item.IsExcellent(), 0);

        return (((energyRequirementValue * (dropLevel + item.Level)) * 3) / 100) + 20;
    }

    private sealed class TemporaryItemAppearance : ItemAppearance
    {
        public override ICollection<ItemOptionType> VisibleOptions => base.VisibleOptions ??= new List<ItemOptionType>();
    }
}