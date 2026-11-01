// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.ItemFormulas;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The implementation of the item power up factory.
/// </summary>
public class ItemPowerUpFactory : IItemPowerUpFactory
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

    private readonly ILogger<ItemPowerUpFactory> _logger;

    private readonly GameConfiguration _gameConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPowerUpFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="gameConfiguration">The game configuration, whose item stat profiles hold the item formulas.</param>
    public ItemPowerUpFactory(ILogger<ItemPowerUpFactory> logger, GameConfiguration gameConfiguration)
    {
        this._logger = logger;
        this._gameConfiguration = gameConfiguration;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public IEnumerable<PowerUpWrapper> GetSetPowerUps(
        IEnumerable<Item> equippedItems,
        AttributeSystem attributeHolder,
        GameConfiguration gameConfiguration)
    {
        var activeItems = equippedItems
            .Where(i => i.Durability > 0)
            .ToList();
        var itemGroups = activeItems
            .SelectMany(i => i.ItemSetGroups)
            .Select(i => i.ItemSetGroup!)
            .Distinct();

        var result = Enumerable.Empty<PowerUpDefinition>();
        var alwaysGroups = activeItems.SelectMany(i => i.Definition!.PossibleItemSetGroups).Where(i => i.AlwaysApplies).Distinct();

        foreach (var group in alwaysGroups.Concat(itemGroups).Distinct())
        {
            var itemsOfGroup = activeItems.Where(i =>
                ((group.AlwaysApplies && i.Definition!.PossibleItemSetGroups.Contains(group))
                 || i.ItemSetGroups.Any(ios => ios.ItemSetGroup == group))
                && (group.SetLevel == 0 || i.Level >= group.SetLevel));
            var setMustBeComplete = group.MinimumItemCount == group.Items.Count;
            if (group.SetLevel > 0 && setMustBeComplete && itemsOfGroup.All(i => i.Level > group.SetLevel))
            {
                // When all items are of higher level and the set bonus is applied when all items are there, another item set group will take care.
                // This should prevent that for example set bonus defense is applied multiple times.
                continue;
            }

            if (group.Options is not { } options)
            {
                this._logger.LogWarning("Options of set {group} is not initialized", group);
                continue;
            }

            var itemCount = group.CountDistinct ? itemsOfGroup.Select(item => item.Definition).Distinct().Count() : itemsOfGroup.Count();
            var setIsComplete = itemCount == group.Items.Count;
            if (setIsComplete)
            {
                // Take all options when the set is complete
                result = result.Concat(
                    options.PossibleOptions
                        .Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
                continue;
            }

            if (itemCount >= group.MinimumItemCount)
            {
                // Take the first n-1 options
                result = result.Concat(options.PossibleOptions.OrderBy(o => o.Number)
                    .Take(itemCount - 1)
                    .Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
            }
        }

        result = result.Concat(this.GetOptionCombinationBonus(activeItems, gameConfiguration));

        return result.SelectMany(p => PowerUpWrapper.CreateByPowerUpDefinition(p, attributeHolder));
    }

    /// <summary>
    /// Finds the level bonus of the level table of a base power-up at the level.
    /// </summary>
    private static LevelBonus? FindLevelBonus(ItemBasePowerUpDefinition? powerUp, int level)
    {
        return powerUp?.BonusPerLevelTable?.BonusPerLevel.FirstOrDefault(bonus => bonus.Level == level);
    }

    /// <summary>
    /// Gets the level bonus of the item's base power-up of the attribute at the item's level.
    /// </summary>
    private static int GetLevelBonus(Item item, AttributeDefinition attribute)
    {
        var powerUp = item.Definition!.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == attribute);
        return (int)(FindLevelBonus(powerUp, item.Level)?.AdditionalValue ?? 0);
    }

    private IEnumerable<PowerUpDefinition> GetOptionCombinationBonus(IEnumerable<Item> activeItems, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration?.ItemOptionCombinationBonuses is null
            || gameConfiguration.ItemOptionCombinationBonuses.Count == 0)
        {
            yield break;
        }

        var activeItemOptions = activeItems.SelectMany(i => i.ItemOptions.Select(o => o.ItemOption ?? throw Error.NotInitializedProperty(o, nameof(o.ItemOption)))).ToList();
        foreach (var combinationBonus in gameConfiguration.ItemOptionCombinationBonuses.Where(c => c.Bonus is { }))
        {
            var remainingOptions = activeItemOptions.ToList<ItemOption>();
            while (this.AreRequiredOptionsFound(combinationBonus, remainingOptions))
            {
                if (combinationBonus.Bonus is not null)
                {
                    yield return combinationBonus.Bonus;
                }
                else
                {
                    this._logger.LogWarning("Bonus of ItemOptionCombinationBonus '{combinationBonusName}' is not initialized, id: {id}", combinationBonus.Description, combinationBonus.GetId());
                }

                if (!combinationBonus.AppliesMultipleTimes)
                {
                    break;
                }
            }
        }
    }

    private bool AreRequiredOptionsFound(ItemOptionCombinationBonus bonus, IList<ItemOption> itemOptions)
    {
        var allMatches = new List<ItemOption>();
        foreach (var requirement in bonus.Requirements)
        {
            var matches = itemOptions
                .Where(o => o.OptionType is not null)
                .Where(o => o.OptionType == requirement.OptionType && o.SubOptionType == requirement.SubOptionType)
                .Take(requirement.MinimumCount)
                .ToList();
            if (matches.Count < requirement.MinimumCount)
            {
                return false;
            }

            allMatches.AddRange(matches);
        }

        allMatches.ForEach(o => itemOptions.Remove(o));
        return true;
    }

    private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute, AttributeDefinition? targetAttribute = null)
    {
        attribute.ThrowNotInitializedProperty(attribute.TargetAttribute is null, nameof(attribute.TargetAttribute));

        var durabilityFactor = DurabilityAffectedItemAttributes.Contains(attribute.TargetAttribute) ? item.GetCurrentDurabilityFactor() : 1;
        var baseValueElmt = attribute.GetBaseValueElement(durabilityFactor);

        var levelBonusElmt = FindLevelBonus(attribute, item.Level)?.GetAdditionalValueElement(attribute.AggregateType, durabilityFactor);
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

    /// <summary>
    /// Creates the power-ups which an excellent or an ancient item gets on its base values. Which kind of item gets which
    /// part stays code (an ancient item also gets the excellent part); the numbers come from the terms of the item's
    /// stat profile (see <see cref="ResolvedItemStatProfile"/>).
    /// </summary>
    private IEnumerable<PowerUpWrapper> CreateExcellentAndAncientBasePowerUpWrappers(Item item, AttributeSystem attributeHolder)
    {
        var itemIsExcellent = item.IsExcellent();
        var itemIsAncient = item.IsAncient();

        if (!itemIsAncient && !itemIsExcellent)
        {
            return [];
        }

        var definition = item.Definition!;
        var profile = ResolvedItemStatProfile.Of(definition, this._gameConfiguration);
        var baseDropLevel = definition.DropLevel;
        var ancientDropLevel = definition.CalculateDropLevel(true, false, this._gameConfiguration);
        var durabilityFactor = item.GetCurrentDurabilityFactor();
        var powerUps = new List<PowerUpWrapper>();

        PowerUpWrapper Wrap(int value, AttributeDefinition target) => new(new SimpleElement(value * durabilityFactor, AggregateType.AddRaw), target, attributeHolder);

        void AddPart(int? part, int divisor, AttributeDefinition target, AttributeDefinition? secondTarget)
        {
            if (part is not { } value)
            {
                return;
            }

            powerUps.Add(Wrap(value / divisor, target));
            if (secondTarget is not null)
            {
                powerUps.Add(Wrap(value / divisor, secondTarget));
            }
        }

        // Adds the excellent part of the attribute, or the fixed excellent bonus when it's above 0, and for an ancient
        // item the ancient part, whose value is the base value plus the excellent part (and the level bonus, when the
        // part says so). An item with a drop level of 0 gets no excellent part, as in the game client. The terms of a
        // rise of the magic power are for the full magic power, which is twice the rise, and each part is halved (the
        // client halves their sum).
        void AddParts(AttributeDefinition attribute, int baseValue, AttributeDefinition target, AttributeDefinition? secondTarget = null, int fixedExcellentBonus = 0, bool isRise = false)
        {
            var factor = isRise ? 2 : 1;

            // The sums are long, so that parts which are limited to the range of int don't wrap when they're added up.
            var value = (long)baseValue * factor;
            var excellentPart = profile.GetExcellentPart(attribute);
            int? excellent = baseDropLevel <= 0 ? null
                : fixedExcellentBonus > 0 ? fixedExcellentBonus
                : excellentPart.HasValues ? excellentPart.Compute(value, baseDropLevel)
                : null;
            AddPart(excellent, factor, target, secondTarget);

            if (itemIsAncient && profile.GetAncientPart(attribute) is { HasValues: true } ancientPart)
            {
                var levelBonus = ancientPart.AfterLevelBonus ? (long)GetLevelBonus(item, attribute) * factor : 0;
                AddPart(ancientPart.Compute(value + (excellent ?? 0) + levelBonus, ancientDropLevel), factor, target, secondTarget);
            }
        }

        int BaseValueOf(AttributeDefinition attribute) => (int)(definition.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == attribute)?.BaseValue ?? 0);

        if (item.IsArmorItem())
        {
            AddParts(Stats.DefenseBase, BaseValueOf(Stats.DefenseBase), Stats.DefenseBase);
        }

        if (item.IsShield())
        {
            AddParts(Stats.DefenseRatePvm, BaseValueOf(Stats.DefenseRatePvm), Stats.DefenseRatePvm);
            AddParts(Stats.DefenseShield, BaseValueOf(Stats.DefenseShield), Stats.DefenseShield);
        }

        if (item.IsPhysicalWeapon(out var minPhysDmg))
        {
            var minDmgAttribute = Stats.MinimumPhysBaseDmgByWeapon;
            var maxDmgAttribute = Stats.MaximumPhysBaseDmgByWeapon;
            if (item.ItemSlot == InventoryConstants.RightHandSlot
                && definition.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount || pu.TargetAttribute == Stats.IsOneHandedStaffEquipped))
            {
                minDmgAttribute = Stats.MinPhysBaseDmgByRightWeapon;
                maxDmgAttribute = Stats.MaxPhysBaseDmgByRightWeapon;
            }

            // The terms of the minimum damage apply to the minimum and the maximum damage, computed from the minimum damage.
            AddParts(Stats.MinimumPhysBaseDmgByWeapon, (int)minPhysDmg, minDmgAttribute, maxDmgAttribute, definition.ExcellentBonus);

            if (itemIsExcellent
                && item.ItemOptions.Any(io => io.ItemOption?.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmgIncrease)
                && definition.BasePowerUpAttributes.Any(pu => pu.TargetAttribute == Stats.DoubleWieldWeaponCount))
            {
                powerUps.Add(new PowerUpWrapper(new SimpleElement(-1, AggregateType.AddRaw), Stats.PhysicalBaseDmgIncrease, attributeHolder));
            }
        }

        if (item.IsWizardryWeapon(out var staffRise) && item.ItemSlot == InventoryConstants.LeftHandSlot)
        {
            AddParts(Stats.StaffRise, (int)staffRise.Value, Stats.StaffRise, fixedExcellentBonus: definition.ExcellentBonus, isRise: true);
        }

        if (item.IsScepter(out var scepterRise))
        {
            AddParts(Stats.ScepterRise, (int)scepterRise.Value, Stats.ScepterRise, fixedExcellentBonus: definition.ExcellentBonus, isRise: true);
        }

        if (item.IsBook(out var curseRise))
        {
            AddParts(Stats.BookRise, (int)curseRise.Value, Stats.BookRise, fixedExcellentBonus: definition.ExcellentBonus, isRise: true);
        }

        if (itemIsAncient && item.IsJewelry())
        {
            foreach (var baseAttribute in definition.BasePowerUpAttributes)
            {
                if (Stats.ElementResistanceToDamageBonus.Keys.FirstOrDefault(resistance => resistance == baseAttribute.TargetAttribute) is { } key)
                {
                    powerUps.Add(Wrap(5, Stats.ElementResistanceToDamageBonus[key]));
                }
            }
        }

        return powerUps;
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