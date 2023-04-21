// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The implementation of the item power up factory.
/// </summary>
public class ItemPowerUpFactory : IItemPowerUpFactory
{
    private readonly ILogger<ItemPowerUpFactory> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemPowerUpFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ItemPowerUpFactory(ILogger<ItemPowerUpFactory> logger)
    {
        this._logger = logger;
    }

    /// <inheritdoc/>
    public IEnumerable<PowerUpWrapper> GetPowerUps(Item item, AttributeSystem attributeHolder)
    {
        if (item.Definition is null)
        {
            throw new ArgumentException($"Item of slot {item.ItemSlot} got no Definition.", nameof(item));
        }

        if (item.Durability <= 0)
        {
            yield break;
        }

        if (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex)
        {
            yield break;
        }

        foreach (var attribute in item.Definition.BasePowerUpAttributes)
        {
            foreach (var powerUp in this.GetBasePowerUpWrappers(item, attributeHolder, attribute))
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
        foreach (var group in itemGroups)
        {
            if (group.AlwaysApplies)
            {
                result = result.Concat(group.Options.Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));

                continue;
            }

            var itemsOfGroup = activeItems.Where(i => i.ItemSetGroups.Any(ios => ios.ItemSetGroup == group)
                                                      && (group.SetLevel == 0 || i.Level >= group.SetLevel));
            var setMustBeComplete = group.MinimumItemCount == group.Items.Count;
            if (group.SetLevel > 0 && setMustBeComplete && itemsOfGroup.All(i => i.Level > group.SetLevel))
            {
                // When all items are of higher level and the set bonus is applied when all items are there, another item set group will take care.
                // This should prevent that for example set bonus defense is applied multiple times.
                continue;
            }

            var itemCount = group.CountDistinct ? itemsOfGroup.Select(item => item.Definition).Distinct().Count() : itemsOfGroup.Count();
            var setIsComplete = itemCount == group.Items.Count;
            if (setIsComplete)
            {
                // Take all options when the set is complete
                result = result.Concat(group.Options.Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
                continue;
            }

            if (itemCount >= group.MinimumItemCount)
            {
                // Take the first n-1 options
                result = result.Concat(group.Options.OrderBy(o => o.Number)
                    .Take(itemCount - 1)
                    .Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
            }
        }

        result = result.Concat(this.GetOptionCombinationBonus(activeItems, gameConfiguration));

        return result.SelectMany(p => PowerUpWrapper.CreateByPowerUpDefinition(p, attributeHolder));
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
                yield return combinationBonus.Bonus ?? throw Error.NotInitializedProperty(combinationBonus, nameof(combinationBonus.Bonus));
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

    private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute)
    {
        attribute.ThrowNotInitializedProperty(attribute.BaseValueElement is null, nameof(attribute.BaseValueElement));
        attribute.ThrowNotInitializedProperty(attribute.TargetAttribute is null, nameof(attribute.TargetAttribute));

        yield return new PowerUpWrapper(attribute.BaseValueElement, attribute.TargetAttribute, attributeHolder);

        var levelBonus = (attribute.BonusPerLevelTable?.BonusPerLevel ?? Enumerable.Empty<LevelBonus>()).FirstOrDefault(bonus => bonus.Level == item.Level);
        if (levelBonus != null)
        {
            levelBonus.ThrowNotInitializedProperty(levelBonus.AdditionalValueElement is null, nameof(levelBonus.AdditionalValueElement));
            yield return new PowerUpWrapper(levelBonus.AdditionalValueElement, attribute.TargetAttribute, attributeHolder);
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
            var option = optionLink.ItemOption ?? throw Error.NotInitializedProperty(optionLink, nameof(optionLink.ItemOption));
            var level = option.LevelType == LevelType.ItemLevel ? item.Level : optionLink.Level;

            var optionOfLevel = option.LevelDependentOptions?.FirstOrDefault(l => l.Level == level);
            if (optionOfLevel is null && level > 1)
            {
                this._logger.LogWarning($"Item has {nameof(IncreasableItemOption)} with level > 1, but no definition in {nameof(IncreasableItemOption.LevelDependentOptions)}");
                continue;
            }

            var powerUp = optionOfLevel?.PowerUpDefinition ?? option.PowerUpDefinition;

            if (powerUp?.Boost is null)
            {
                // Some options are level dependent. If they are at level 0, they might not have any boost yet.
                continue;
            }

            foreach (var wrapper in PowerUpWrapper.CreateByPowerUpDefinition(powerUp, attributeHolder))
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

        if (InventoryConstants.IsDefenseItemSlot(item.ItemSlot))
        {
            var baseDefense = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseBase)?.BaseValue ?? 0);
            var additionalDefense = (baseDefense * 12 / baseDropLevel) + (baseDropLevel / 5) + 4;
            yield return new PowerUpWrapper(new SimpleElement(additionalDefense, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);

            if (itemIsAncient)
            {
                var ancientDefenseBonus = (baseDefense * 3 / ancientDropLevel) + (ancientDropLevel / 30) + 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientDefenseBonus, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);
            }
        }

        if (item.IsShield())
        {
            var baseDefenseRate = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseRatePvm)?.BaseValue ?? 0);
            var additionalRate = (baseDefenseRate * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalRate, AggregateType.AddRaw), Stats.DefenseRatePvm, attributeHolder);
            if (itemIsAncient)
            {
                var baseDefense = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseBase)?.BaseValue ?? 0);
                var ancientDefenseBonus = (baseDefense * 20 / ancientDropLevel) + 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientDefenseBonus, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);
            }
        }

        if (item.IsPhysicalWeapon(out var minPhysDmg))
        {
            var additionalDmg = ((int)minPhysDmg * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalDmg, AggregateType.AddRaw), Stats.MinimumPhysBaseDmgByWeapon, attributeHolder);
            yield return new PowerUpWrapper(new SimpleElement(additionalDmg, AggregateType.AddRaw), Stats.MaximumPhysBaseDmgByWeapon, attributeHolder);
            if (itemIsAncient)
            {
                var ancientBonus = 5 + (ancientDropLevel / 40);
                yield return new PowerUpWrapper(new SimpleElement(ancientBonus, AggregateType.AddRaw), Stats.MinimumPhysBaseDmgByWeapon, attributeHolder);
                yield return new PowerUpWrapper(new SimpleElement(ancientBonus, AggregateType.AddRaw), Stats.MaximumPhysBaseDmgByWeapon, attributeHolder);
            }
        }

        if (item.IsWizardryWeapon(out var staffRise))
        {
            var additionalRise = ((int)staffRise * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = 5 + (ancientDropLevel / 60);
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            }
        }

        if (item.IsScepter(out var scepterRise))
        {
            var additionalRise = ((int)scepterRise * 25 / baseDropLevel) + 5;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = 5 + (ancientDropLevel / 60);
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
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