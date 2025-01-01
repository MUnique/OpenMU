// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence;

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

    private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute)
    {
        attribute.ThrowNotInitializedProperty(attribute.BaseValueElement is null, nameof(attribute.BaseValueElement));
        attribute.ThrowNotInitializedProperty(attribute.TargetAttribute is null, nameof(attribute.TargetAttribute));

        var levelBonusElmt = (attribute.BonusPerLevelTable?.BonusPerLevel ?? Enumerable.Empty<LevelBonus>())
            .FirstOrDefault(bonus => bonus.Level == item.Level)?
            .GetAdditionalValueElement(attribute.AggregateType);

        if (levelBonusElmt is null)
        {
            yield return new PowerUpWrapper(attribute.BaseValueElement, attribute.TargetAttribute, attributeHolder);
        }
        else
        {
            yield return new PowerUpWrapper(
                new CombinedElement(attribute.BaseValueElement, levelBonusElmt),
                attribute.TargetAttribute,
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

            var level = option.LevelType == LevelType.ItemLevel ? item.Level : optionLink.Level;

            var optionOfLevel = option.LevelDependentOptions?.FirstOrDefault(l => l.Level == level);
            if (optionOfLevel is null && level > 1 && item.Definition!.Skill?.Number != 49) // Dinorant options are an exception
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

        if (InventoryConstants.IsDefenseItemSlot(item.ItemSlot) && !item.IsJewelry())
        {
            var baseDefense = (int)(item.Definition?.BasePowerUpAttributes.FirstOrDefault(a => a.TargetAttribute == Stats.DefenseBase)?.BaseValue ?? 0);
            var additionalDefense = (baseDefense * 12 / baseDropLevel) + (baseDropLevel / 5) + 4;
            yield return new PowerUpWrapper(new SimpleElement(additionalDefense, AggregateType.AddRaw), Stats.DefenseBase, attributeHolder);
            if (itemIsAncient)
            {
                var ancientDefenseBonus = 2 + ((baseDefense + additionalDefense) * 3 / ancientDropLevel) + (ancientDropLevel / 30);
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
                var ancientDefenseBonus = 2 + ((baseDefense + item.Level) * 20 / ancientDropLevel);
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
            var additionalRise = (((int)staffRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus, AggregateType.AddRaw), Stats.StaffRise, attributeHolder);
            }
        }

        if (item.IsScepter(out var scepterRise))
        {
            var additionalRise = (((int)scepterRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus, AggregateType.AddRaw), Stats.ScepterRise, attributeHolder);
            }
        }

        if (item.IsBook(out var curseRise))
        {
            var additionalRise = (((int)curseRise * 2 * 25 / baseDropLevel) + 5) / 2;
            yield return new PowerUpWrapper(new SimpleElement(additionalRise, AggregateType.AddRaw), Stats.BookRise, attributeHolder);
            if (itemIsAncient)
            {
                var ancientRiseBonus = (2 + (ancientDropLevel / 60)) / 2;
                yield return new PowerUpWrapper(new SimpleElement(ancientRiseBonus, AggregateType.AddRaw), Stats.BookRise, attributeHolder);
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