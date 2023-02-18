// <copyright file="ItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

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
    /// Determines whether the item level can be upgraded by using jewels of bless or soul.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the item level can be upgraded by using jewels of bless or soul; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanLevelBeUpgraded(this Item item)
    {
        return item.IsWearable()
               && item.Definition!.ItemSlot!.ItemSlots.Any(slot => slot <= InventoryConstants.WingsSlot)
               && item.Level < item.Definition.MaximumItemLevel;
    }

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
        if (item.ItemOptions.Any(link => link.ItemOption?.OptionType == ItemOptionTypes.AncientBonus))
        {
            result += 20;
        }
        else if (item.ItemOptions.Any(link => link.ItemOption?.OptionType == ItemOptionTypes.Excellent))
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
    /// Determines whether this item can have a skill.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item can have a skill; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanHaveSkill(this Item item) => item.IsWearable() && item.Definition?.Skill != null && item.Definition.QualifiedCharacters.Any();

    /// <summary>
    /// Determines whether this item is wearable.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is wearable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWearable(this Item item) => item.Definition?.ItemSlot != null;

    /// <summary>
    /// Determines whether this item is stackable.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is stackable; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsStackable(this Item item) => !item.IsWearable() && item.Definition?.Durability > 1;

    /// <summary>
    /// Determines whether this item can be completely stacked on the specified other item.
    /// After stacking, this item is destroyed.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item can be completely stacked on the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanCompletelyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && (item.Durability + otherItem.Durability) <= item.Definition?.Durability;

    /// <summary>
    /// Determines whether this item can be partially stacked on the specified other item.
    /// After stacking, this item is left with the rest of its durability.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item can be partially stacked on the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanPartiallyStackOn(this Item item, Item otherItem) => item.IsStackable() && item.IsSameItemAs(otherItem) && otherItem.Durability < otherItem.Definition?.Durability;

    /// <summary>
    /// Determines whether this item is of the same type as the specified other item.
    /// <see cref="Item.Definition"/> and <see cref="Item.Level"/> need to be equal to get considered as the same item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="otherItem">The other item.</param>
    /// <returns>
    ///   <c>true</c> if this item is of the same type as the specified other item; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSameItemAs(this Item item, Item otherItem) => item.Definition == otherItem.Definition && item.Level == otherItem.Level;

    /// <summary>
    /// Gets the <see cref="Item.Durability"/> as byte value, rounded off.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The durability.</returns>
    public static byte Durability(this Item item) => (byte)Math.Floor(item.Durability);

    /// <summary>
    /// Decreases the durability of an item, and returns <see langword="true"/>, if the integral number changed.
    /// </summary>
    /// <param name="item">The item whose durability should be decreased.</param>
    /// <param name="decrement">The decrement value.</param>
    /// <returns><see langword="true"/>, if the integral number changed.</returns>
    public static bool DecreaseDurability(this Item item, double decrement)
    {
        var previous = item.Durability;
        item.Durability = Math.Max(previous - decrement, 0.0);
        return (byte)Math.Floor(item.Durability) != (byte)Math.Floor(previous);
    }

    /// <summary>
    /// Determines whether this item is a pet (Dark Raven, Dark Horse) which can gain levels itself.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><see langword="true"/>, if the item is a pet.</returns>
    public static bool IsTrainablePet(this Item item)
    {
        return item.Definition?.PetExperienceFormula is not null;
    }

    /// <summary>
    /// Determines whether this item is a defensive item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><see langword="true"/>, if the item is defensive.</returns>
    public static bool IsDefensiveItem(this Item item)
    {
        return InventoryConstants.IsDefenseItemSlot(item.ItemSlot) || item.Definition?.Group == ShieldItemGroup;
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
    public static (AttributeDefinition, int) GetRequirement(this Item item, AttributeRequirement requirement)
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
        return appearance;
    }

    /// <summary>
    /// Creates a persistent instance of the given <see cref="ItemAppearance"/> and returns it.
    /// </summary>
    /// <param name="itemAppearance">The item appearance.</param>
    /// <param name="persistenceContext">The persistence context where the object should be added.</param>
    /// <returns>A persistent instance of the given <see cref="ItemAppearance"/>.</returns>
    public static ItemAppearance MakePersistent(this ItemAppearance itemAppearance, IContext persistenceContext)
    {
        var persistent = persistenceContext.CreateNew<ItemAppearance>();
        persistent.ItemSlot = itemAppearance.ItemSlot;
        persistent.Definition = itemAppearance.Definition;
        persistent.Level = itemAppearance.Level;
        itemAppearance.VisibleOptions.Distinct().ForEach(o => persistent.VisibleOptions.Add(o));
        return persistent;
    }

    private static int CalculateRequirement(this Item item, int requirementValue, int multiplier)
    {
        if (requirementValue == 0)
        {
            return 0;
        }

        var dropLevel = item.Definition!.DropLevel;
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent || o.ItemOption?.OptionType == ItemOptionTypes.AncientOption))
        {
            dropLevel += 25;
        }

        return (multiplier * ((3 * item.Level) + dropLevel) * requirementValue / 100) + 20;
    }

    private static int CalculateBookEnergyRequirement(this Item item, int energyRequirementValue)
    {
        var dropLevel = item.Definition!.DropLevel;
        if (item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Excellent))
        {
            dropLevel += 25;
        }

        return (((energyRequirementValue * (dropLevel + item.Level)) * 3) / 100) + 20;
    }

    private sealed class TemporaryItemAppearance : ItemAppearance
    {
        public override ICollection<ItemOptionType> VisibleOptions => base.VisibleOptions ??= new List<ItemOptionType>();
    }
}