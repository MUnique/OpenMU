// <copyright file="LegacyItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests.ItemFormulas.Legacy;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// A frozen copy of <c>src/GameLogic/ItemPowerUpFactory.cs</c> at commit 90a571100,
/// which <see cref="ItemFormulasEqualTodayTest"/> compares with the current code.
/// Change it only together with an intentional change of the formula, see the remarks of the test.
/// </summary>
/// <remarks>
/// Lines 17-101 and 218-455 are copied unchanged, except the class name in lines 30, 33 and 36, and the documentation of
/// <c>GetPowerUps</c> (line 41), which took it from the interface the copy doesn't implement, so that a change of the live
/// interface doesn't change the copy. The set power-ups (lines 103-216) are left out: the test doesn't compare them,
/// and the item formulas aren't part of them.
/// The item checks, the drop level, the durability factor and <c>IsFixedOption</c> come from <see cref="LegacyItemExtensions"/>,
/// which also names the live code the copies still call.
/// </remarks>
internal class LegacyItemPowerUpFactory
{
    private static readonly HashSet<AttributeDefinition> DurabilityAffectedItemAttributes =
    [
        Stats.DefenseBase,
        Stats.DefenseShield,
        Stats.DefenseRatePvm,
        Stats.MinimumPhysBaseDmgByWeapon,
        Stats.MaximumPhysBaseDmgByWeapon,
        Stats.StaffRise,
        Stats.ScepterRise,
        Stats.BookRise,
    ];

    private readonly ILogger<LegacyItemPowerUpFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyItemPowerUpFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public LegacyItemPowerUpFactory(ILogger<LegacyItemPowerUpFactory> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Gets the power ups of an individual item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="attributeHolder">The attribute system of the player who equipped the item.</param>
    /// <returns>The created power ups.</returns>
    public IEnumerable<PowerUpWrapper> GetPowerUps(Item item, AttributeSystem attributeHolder)
    {
        if (item.Definition is null)
        {
            this._logger.LogWarning("Item of slot {itemSlot} ({itemId}) has no definition.", item.ItemSlot, item.GetId());
            yield break;
        }

        if (item.Durability <= 0)
        {
            yield break;
        }

        if (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex)
        {
            yield break;
        }

        var isRightWieldWeapon = item.ItemSlot == InventoryConstants.RightHandSlot
            && item.Definition!.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount || pu.TargetAttribute == Stats.IsOneHandedStaffEquipped);

        AttributeDefinition? targetAttribute = null;
        foreach (var attribute in item.Definition.BasePowerUpAttributes)
        {
            if (isRightWieldWeapon)
            {
                if (attribute.TargetAttribute == Stats.StaffRise)
                {
                    continue;
                }
                else if (attribute.TargetAttribute == Stats.MinimumPhysBaseDmgByWeapon)
                {
                    targetAttribute = Stats.MinPhysBaseDmgByRightWeapon;
                }
                else if (attribute.TargetAttribute == Stats.MaximumPhysBaseDmgByWeapon)
                {
                    targetAttribute = Stats.MaxPhysBaseDmgByRightWeapon;
                }
                else
                {
                    targetAttribute = null;
                }
            }

            foreach (var powerUp in this.GetBasePowerUpWrappers(item, attributeHolder, attribute, targetAttribute))
            {
                yield return powerUp;
            }
        }

        foreach (var powerUp in this.GetPowerUpsOfItemOptions(item, attributeHolder))
        {
            yield return powerUp;
        }

        if (this.GetPetLevel(item, attributeHolder) is { } petLevel)
        {
            yield return petLevel;
        }
    }

    private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute, AttributeDefinition? targetAttribute = null)
    {
        attribute.ThrowNotInitializedProperty(attribute.TargetAttribute is null, nameof(attribute.TargetAttribute));

        var durabilityFactor = DurabilityAffectedItemAttributes.Contains(attribute.TargetAttribute) ? item.GetCurrentDurabilityFactor() : 1;
        var baseValueElmt = attribute.GetBaseValueElement(durabilityFactor);

        var levelBonusElmt = (attribute.BonusPerLevelTable?.BonusPerLevel ?? Enumerable.Empty<LevelBonus>())
            .FirstOrDefault(bonus => bonus.Level == item.Level)?
            .GetAdditionalValueElement(attribute.AggregateType, durabilityFactor);
        if (levelBonusElmt is null)
        {
            yield return new PowerUpWrapper(baseValueElmt, targetAttribute ?? attribute.TargetAttribute, attributeHolder);
        }
        else
        {
            yield return new PowerUpWrapper(
                new CombinedElement(baseValueElmt, levelBonusElmt),
                targetAttribute ?? attribute.TargetAttribute,
                attributeHolder);
        }
    }

    private IEnumerable<PowerUpWrapper> GetPowerUpsOfItemOptions(Item item, AttributeSystem attributeHolder)
    {
        var options = item.ItemOptions;
        if (options is null)
        {
            yield break;
        }

        foreach (var optionLink in options)
        {
            var option = optionLink.ItemOption;
            if (option is null)
            {
                this._logger.LogWarning("Item {item} (id {itemId}) has ItemOptionLink ({optionLinkId}) without option.", item, item.GetId(), optionLink.GetId());
                continue;
            }

            if (item.ItemSlot == InventoryConstants.RightHandSlot
                && option.OptionType == ItemOptionTypes.Option
                && option.PowerUpDefinition?.TargetAttribute == Stats.WizardryBaseDmg)
            {
                // For a RH-wielded staff (MG), its wizardry item option doesn't count (but the others do!)
                continue;
            }

            var level = option.LevelType == LevelType.ItemLevel ? item.Level : optionLink.Level;

            var optionOfLevel = option.LevelDependentOptions?.FirstOrDefault(l => l.Level == level);

            // Fixed options (e.g. of the Dinorant) have no level dependent values, so their level doesn't matter.
            if (optionOfLevel is null && level > 1 && !option.IsFixedOption())
            {
                this._logger.LogWarning("Item {item} (id {itemId}) has IncreasableItemOption ({option}, id {optionId}) with level {level}, but no definition in LevelDependentOptions.", item, item.GetId(), option, option.GetId(), level);
                continue;
            }

            if (optionOfLevel?.RequiredItemLevel > item.Level)
            {
                // Some options (like harmony) although on the item can be inactive.
                continue;
            }

            var powerUp = optionOfLevel?.PowerUpDefinition ?? option.PowerUpDefinition;

            if (powerUp?.Boost is null)
            {
                // Some options are level dependent. If they are at level 0, they might not have any boost yet.
                continue;
            }

            AggregateType? aggregateType = null;
            if (option.OptionType == ItemOptionTypes.Excellent)
            {
                if (item.ItemSlot == InventoryConstants.PendantSlot
                    && option.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmg)
                {
                    // Pendant options are not subject to double wield averaging
                    aggregateType = AggregateType.AddFinal;
                }
                else if (option.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmgIncrease
                    && item.Definition!.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount))
                {
                    // This needs special treatment, since this option is averaged when double wielding
                    aggregateType = AggregateType.AddRaw;
                }
                else
                {
                    // the normal aggregate type should be used
                }
            }

            var durabilityFactor = option.OptionType == ItemOptionTypes.AncientBonus ||
                    (option.OptionType == ItemOptionTypes.Option && powerUp.TargetAttribute != Stats.HealthRecoveryMultiplier && !item.IsJewelry())
                ? item.GetCurrentDurabilityFactor() : 1.0f;
            foreach (var wrapper in PowerUpWrapper.CreateByPowerUpDefinition(powerUp, attributeHolder, aggregateType, durabilityFactor))
            {
                yield return wrapper;
            }
        }

        foreach (var powerUpWrapper in this.CreateExcellentAndAncientBasePowerUpWrappers(item, attributeHolder))
        {
            yield return powerUpWrapper;
        }
    }

    // TODO: Make this more generic and configurable?
    private IEnumerable<PowerUpWrapper> CreateExcellentAndAncientBasePowerUpWrappers(Item item, AttributeSystem attributeHolder)
    {
        var itemIsExcellent = item.IsExcellent();
        var itemIsAncient = item.IsAncient();

        if (!itemIsAncient && !itemIsExcellent)
        {
            yield break;
        }

        var baseDropLevel = item.Definition!.DropLevel;
        var ancientDropLevel = item.Definition!.CalculateDropLevel(true, false, 0);
        var durabilityFactor = item.GetCurrentDurabilityFactor();

        if (item.IsArmorItem())
        {
            var baseDefense = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseBase)?.BaseValue ?? 0);
            var additionalDefense = (baseDefense * 12 / baseDropLevel) + (baseDropLevel / 5) + 4;
            yield return new PowerUpWrapper(new SimpleElement(additionalDefense * durabilityFactor, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);
            if (itemIsAncient)
            {
                var ancientDefenseBonus = 2 + ((baseDefense + additionalDefense) * 3 / ancientDropLevel) + (ancientDropLevel / 30);
                yield return new PowerUpWrapper(new SimpleElement(ancientDefenseBonus * durabilityFactor, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);
            }
        }

        if (item.IsShield())
        {
            var baseDefenseRate = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseRatePvm)?.BaseValue ?? 0);
            var additionalRate = (baseDefenseRate * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalRate * durabilityFactor, AggregateType.AddRaw), Stats.DefenseRatePvm, attributeHolder);
            if (itemIsAncient)
            {
                var baseDefense = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseShield)?.BaseValue ?? 0);
                var ancientDefenseBonus = 2 + ((baseDefense + item.Level) * 20 / ancientDropLevel);
                yield return new PowerUpWrapper(new SimpleElement(ancientDefenseBonus * durabilityFactor, AggregateType.AddRaw), Stats.DefenseShield, attributeHolder);
            }
        }

        if (item.IsPhysicalWeapon(out var minPhysDmg))
        {
            var minDmgAttribute = Stats.MinimumPhysBaseDmgByWeapon;
            var maxDmgAttribute = Stats.MaximumPhysBaseDmgByWeapon;
            if (item.ItemSlot == InventoryConstants.RightHandSlot
                && item.Definition!.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount || pu.TargetAttribute == Stats.IsOneHandedStaffEquipped))
            {
                minDmgAttribute = Stats.MinPhysBaseDmgByRightWeapon;
                maxDmgAttribute = Stats.MaxPhysBaseDmgByRightWeapon;
            }

            var additionalDmg = ((int)minPhysDmg * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalDmg * durabilityFactor, AggregateType.AddRaw), minDmgAttribute, attributeHolder);
            yield return new PowerUpWrapper(new SimpleElement(additionalDmg * durabilityFactor, AggregateType.AddRaw), maxDmgAttribute, attributeHolder);
            if (itemIsAncient)
            {
                var ancientBonus = 5 + (ancientDropLevel / 40);
                yield return new PowerUpWrapper(new SimpleElement(ancientBonus * durabilityFactor, AggregateType.AddRaw), minDmgAttribute, attributeHolder);
                yield return new PowerUpWrapper(new SimpleElement(ancientBonus * durabilityFactor, AggregateType.AddRaw), maxDmgAttribute, attributeHolder);
            }

            if (itemIsExcellent
                && item.ItemOptions.Any(io => io.ItemOption?.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmgIncrease)
                && item.Definition!.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount))
            {
                yield return new PowerUpWrapper(new SimpleElement(-1, AggregateType.AddRaw), Stats.PhysicalBaseDmgIncrease, attributeHolder);
            }
        }

        if (item.IsWizardryWeapon(out var staffRise) && item.ItemSlot == InventoryConstants.LeftHandSlot)
        {
            var additionalRise = (((int)staffRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise * durabilityFactor, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus * durabilityFactor, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            }
        }

        if (item.IsScepter(out var scepterRise))
        {
            var additionalRise = (((int)scepterRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise * durabilityFactor, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus * durabilityFactor, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
            }
        }

        if (item.IsBook(out var curseRise))
        {
            var additionalRise = (((int)curseRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise * durabilityFactor, AggregateType.AddRaw), Stats.BookRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus * durabilityFactor, AggregateType.AddRaw), Stats.BookRise, attributeHolder);
            }
        }

        if (itemIsAncient && item.IsJewelry())
        {
            foreach (var baseAttribute in item.Definition!.BasePowerUpAttributes)
            {
                if (Stats.ElementResistanceToDamageBonus.Keys.FirstOrDefault(resistance => resistance == baseAttribute.TargetAttribute) is { } key)
                {
                    yield return new PowerUpWrapper(new SimpleElement(5 * durabilityFactor, AggregateType.AddRaw), Stats.ElementResistanceToDamageBonus[key], attributeHolder);
                }
            }
        }
    }

    private PowerUpWrapper? GetPetLevel(Item item, AttributeSystem attributeHolder)
    {
        const byte darkHorseNumber = 4;

        if (!item.IsTrainablePet())
        {
            return null;
        }

        return new PowerUpWrapper(
            new SimpleElement(item.Level, AggregateType.AddRaw),
            item.Definition?.Number == darkHorseNumber ? Stats.HorseLevel : Stats.RavenLevel,
            attributeHolder);
    }
}
