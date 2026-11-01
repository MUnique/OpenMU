// <copyright file="LegacyItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests.ItemFormulas.Legacy;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// A frozen copy of the item requirement formulas in <c>src/GameLogic/ItemExtensions.cs</c> at commit 90a571100,
/// with the item checks and durability functions which the three frozen copies use,
/// which <see cref="ItemFormulasEqualTodayTest"/> compares with the current code.
/// Change it only together with an intentional change of the formula, see the remarks of the test.
/// </summary>
/// <remarks>
/// Lines 19-106, 131-141, 174-265, 279-289, 361-426, 474-500, 512-542 and 544-561 are copied unchanged, and between the
/// last two, lines 14-27 of <c>src/GameLogic/ItemOptionExtensions.cs</c> (<c>IsFixedOption</c>).
/// The extension method calls in the frozen copies bind to this class wherever it has a method of that name,
/// because extension methods of the own namespace come before imported ones; the copies don't import
/// <c>MUnique.OpenMU.GameLogic</c>, so none of its extension methods can change the copies and the live code together.
/// They still use live code which the item formulas don't contain: <c>IsWearable</c>, <c>IsTrainablePet</c>,
/// <c>IsDarkRaven</c>, <c>IsWing</c> and <c>Durability</c> of <c>src/DataModel/ItemExtensions.cs</c>; the code which builds
/// the elements of the power-ups and applies the durability factor to them: <c>PowerUpWrapper.CreateByPowerUpDefinition</c>
/// of <c>MUnique.OpenMU.GameLogic.Attributes</c>, and <c>PowerUpDefinitionValue.GetSimpleValueElement</c>,
/// <c>ItemBasePowerUpDefinition.GetBaseValueElement</c> and <c>LevelBonus.GetAdditionalValueElement</c> of the data model;
/// and the attribute definitions of <c>Stats</c>, including the <c>ElementResistanceToDamageBonus</c> map of the ancient jewelry bonus.
/// </remarks>
internal static class LegacyItemExtensions
{
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

        var minimumValue = requirement.MinimumItemLevel is { } minimumItemLevel && item.Level < minimumItemLevel
            ? 0
            : requirement.MinimumValue;

        if (RequirementAttributeMapping.TryGetValue(requirement.Attribute, out var totalAttribute))
        {
            if (!item.IsWearable())
            {
                return (totalAttribute, minimumValue);
            }

            var multiplier = 3;
            if (totalAttribute == Stats.TotalEnergy)
            {
                multiplier = 4;
            }

            // Summoner books, which increase the curse damage by their book rise, are calculated differently. Changed
            // after commit 90a571100: a base value of 0 stays 0, and the reduction applies (see MUnique/OpenMU#1301).
            var value = totalAttribute == Stats.TotalEnergy && item.IsBook(out _)
                ? item.CalculateBookEnergyRequirement(minimumValue)
                : item.CalculateRequirement(minimumValue, multiplier);
            if (value > 0 && totalAttribute == Stats.TotalStrength)
            {
                var itemOption = item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option);
                if (itemOption != null)
                {
                    value += itemOption.Level * (item.IsArmorItem() ? 5 : 4);
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

        return (requirement.Attribute, minimumValue);
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
    /// Determines whether the option is a fixed option, which is a normal option without level dependent values.
    /// </summary>
    /// <remarks>
    /// Several fixed options (e.g. of the Horn of Dinorant) can be on one item.
    /// The client receives them in the field of the normal option level, where
    /// the <see cref="ItemOption.Number"/> of each fixed option is its bit.
    /// </remarks>
    /// <param name="option">The option.</param>
    /// <returns><c>true</c>, if the option is a fixed option; otherwise, <c>false</c>.</returns>
    public static bool IsFixedOption(this IncreasableItemOption option)
    {
        return option.OptionType == ItemOptionTypes.Option && option.LevelDependentOptions is not { Count: > 0 };
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
        if (energyRequirementValue == 0)
        {
            return 0;
        }

        var dropLevel = item.Definition!.CalculateDropLevel(item.IsAncient(), item.IsExcellent(), 0);

        return (((energyRequirementValue * (dropLevel + item.Level)) * 3) / 100) + 20;
    }
}
