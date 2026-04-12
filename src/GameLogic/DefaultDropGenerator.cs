// <copyright file="DefaultDropGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using Nito.AsyncEx;

/// <summary>
/// The default drop generator.
/// </summary>
public class DefaultDropGenerator : IDropGenerator
{
    /// <summary>
    /// The amount of money which is dropped at least, and added to the gained experience.
    /// </summary>
    public static readonly int BaseMoneyDrop = 7;

    private const int DropLevelMaxGap = 12;
    private const int JewelOfChaosMaxMonsterLevel = 66;
    private const int JewelOfChaosGroup = 12;
    private const int JewelOfChaosNumber = 15;
    private const int SkillDropChancePercent = 50;

    private readonly IRandomizer _randomizer;

    /// <summary>
    /// A re-usable list of drop item groups.
    /// </summary>
    private readonly List<DropItemGroup> _chanceDropGroups = new(64);
    private readonly List<DropItemGroup> _guaranteedDropGroups = new(8);

    private readonly AsyncLock _lock = new();

    private readonly byte _maxItemOptionLevelDrop;

    private readonly IList<ItemDefinition> _ancientItems;

    private readonly IList<ItemDefinition> _droppableItems;

    private readonly IList<ItemDefinition>?[] _droppableItemsPerMonsterLevel = new IList<ItemDefinition>?[byte.MaxValue + 1];

    private readonly byte _excellentItemDropLevelDelta;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDropGenerator" /> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="randomizer">The randomizer.</param>
    public DefaultDropGenerator(GameConfiguration config, IRandomizer randomizer)
    {
        this._excellentItemDropLevelDelta = config.ExcellentItemDropLevelDelta;
        this._randomizer = randomizer;
        this._maxItemOptionLevelDrop = config.MaximumItemOptionLevelDrop < 1 || config.MaximumItemOptionLevelDrop > 4 ? (byte)3 : config.MaximumItemOptionLevelDrop;
        this._droppableItems = config.Items.Where(i => i.DropsFromMonsters).ToList();
        this._ancientItems = this._droppableItems.Where(
            i => i.PossibleItemSetGroups.Any(
                g => g.Options?.PossibleOptions.Any(
                    o => object.Equals(o.OptionType, ItemOptionTypes.AncientOption)) ?? false))
            .ToList();
    }

    /// <inheritdoc/>
    public async ValueTask<(IEnumerable<Item> Items, uint? Money)> GenerateItemDropsAsync(MonsterDefinition monster, int gainedExperience, Player player)
    {
        var character = player.SelectedCharacter;
        var map = player.CurrentMap?.Definition;
        if (map is null || character is null)
        {
            return ([], null);
        }

        using var l = await this._lock.LockAsync();
        this._guaranteedDropGroups.Clear();
        this._chanceDropGroups.Clear();

        if (monster.ObjectKind == NpcObjectKind.Destructible)
        {
            this.PartitionDropGroups(monster.DropItemGroups ?? []);
        }
        else
        {
            this.PartitionDropGroups(monster.DropItemGroups ?? []);
            this.PartitionDropGroups(character.DropItemGroups ?? [], monster);
            this.PartitionDropGroups(map.DropItemGroups ?? [], monster);
            this.PartitionDropGroups(await GetQuestItemGroupsAsync(player).ConfigureAwait(false) ?? [], monster);
        }

        uint money = 0;
        IList<Item>? droppedItems = null;
        var remainingDrops = monster.NumberOfMaximumItemDrops;

        // Guaranteed groups, no random selection needed.
        foreach (var group in this._guaranteedDropGroups)
        {
            if (remainingDrops <= 0)
            {
                break;
            }

            var item = this.GenerateItemDropOrMoney(monster, group, gainedExperience, out var droppedMoney);
            if (item is not null)
            {
                droppedItems ??= new List<Item>(monster.NumberOfMaximumItemDrops);
                droppedItems.Add(item);
            }

            if (droppedMoney is not null)
            {
                money += droppedMoney.Value;
            }

            remainingDrops--;
        }

        // Chance based groups with random selection
        if (remainingDrops > 0 && this._chanceDropGroups.Count > 0)
        {
            double totalChance = 0;
            foreach (var group in this._chanceDropGroups)
            {
                totalChance += group.Chance;
            }

            for (int i = 0; i < remainingDrops; i++)
            {
                var group = this.SelectRandomGroup(this._chanceDropGroups, totalChance);
                if (group is null)
                {
                    continue;
                }

                var item = this.GenerateItemDropOrMoney(monster, group, gainedExperience, out var droppedMoney);
                if (item is not null)
                {
                    droppedItems ??= new List<Item>(monster.NumberOfMaximumItemDrops);
                    droppedItems.Add(item);
                }

                if (droppedMoney is not null)
                {
                    money += droppedMoney.Value;
                }
            }
        }

        this._guaranteedDropGroups.Clear();
        this._chanceDropGroups.Clear();
        return (droppedItems ?? Enumerable.Empty<Item>(), money > 0 ? money : null);
    }

    /// <inheritdoc/>
    public Item? GenerateItemDrop(DropItemGroup selectedGroup)
    {
        return this.GenerateItemDrop(selectedGroup, selectedGroup.PossibleItems);
    }

    /// <inheritdoc/>
    public (Item? Item, uint? Money, ItemDropEffect DropEffect) GenerateItemDrop(IEnumerable<DropItemGroup> groups)
    {
        var group = this.SelectRandomGroup(groups.OrderBy(group => group.Chance), 1.0);
        if (group is null)
        {
            return (null, null, ItemDropEffect.Undefined);
        }

        var dropEffect = ItemDropEffect.Undefined;
        if (group is ItemDropItemGroup itemDropItemGroup)
        {
            dropEffect = itemDropItemGroup.DropEffect;

            if (group.ItemType == SpecialItemType.Money)
            {
                return (null, (uint)itemDropItemGroup.MoneyAmount, dropEffect);
            }
        }

        return (this.GenerateItemDrop(group), null, dropEffect);
    }

    /// <summary>
    /// Gets a random item.
    /// </summary>
    /// <param name="monsterLvl">The monster level.</param>
    /// <param name="socketItems">If set to <c>true</c>, it selects only items with sockets.</param>
    /// <returns>A random item.</returns>
    protected Item? GenerateRandomItem(int monsterLvl, bool socketItems)
    {
        var possible = this.GetPossibleList(monsterLvl, socketItems);
        var item = this.GenerateRandomItem(possible);
        if (item is null)
        {
            return null;
        }

        item.Level = GetItemLevelByMonsterLevel(item.Definition!, monsterLvl);
        item.Durability = item.GetMaximumDurabilityOfOnePiece();
        return item;
    }

    /// <summary>
    /// Applies random options to the item.
    /// </summary>
    /// <param name="item">The item.</param>
    protected void ApplyRandomOptions(Item item)
    {
        foreach (var option in item.Definition!.PossibleItemOptions.Where(o =>
            o.AddsRandomly &&
            !o.PossibleOptions.Any(po => object.Equals(po.OptionType, ItemOptionTypes.Excellent))))
        {
            this.ApplyOption(item, option);
        }

        if (item.Definition.MaximumSockets > 0)
        {
            item.SocketCount = this._randomizer.NextInt(1, item.Definition.MaximumSockets + 1);
        }

        if (item.CanHaveSkill())
        {
            item.HasSkill = this._randomizer.NextRandomBool(SkillDropChancePercent);
        }
    }

    /// <summary>
    /// Gets a random excellent item.
    /// </summary>
    /// <param name="monsterLvl">The monster level, if it's a monster drop.</param>
    /// <param name="possibleItems">The possible items, if the drop is from an item box (e.g. box of kundun).</param>
    /// <returns>A random excellent item.</returns>
    protected Item? GenerateRandomExcellentItem(int monsterLvl = 0, ICollection<ItemDefinition>? possibleItems = null)
    {
        if (monsterLvl < this._excellentItemDropLevelDelta && possibleItems is null)
        {
            return null;
        }

        var possible = possibleItems ?? this.GetPossibleList(monsterLvl - this._excellentItemDropLevelDelta);
        var item = this.GenerateRandomItem(possible);
        if (item is null)
        {
            return null;
        }

        item.HasSkill = item.CanHaveSkill(); // every excellent item got skill

        this.AddRandomExcOptions(item);
        item.Durability = item.GetMaximumDurabilityOfOnePiece();
        return item;
    }

    /// <summary>
    /// Gets a random ancient item.
    /// </summary>
    /// <returns>A random ancient item.</returns>
    protected Item? GenerateRandomAncient()
    {
        var item = this.GenerateRandomItem(this._ancientItems);
        if (item is null)
        {
            return null;
        }

        item.HasSkill = item.CanHaveSkill(); // every ancient item got skill

        this.ApplyRandomAncientOption(item);
        item.Durability = item.GetMaximumDurabilityOfOnePiece();
        return item;
    }

    private static byte GetItemLevelByMonsterLevel(ItemDefinition itemDefinition, int monsterLevel)
    {
        return Math.Min((byte)((monsterLevel - itemDefinition.DropLevel) / 3), itemDefinition.MaximumItemLevel);
    }

    private static async ValueTask<IEnumerable<DropItemGroup>> GetQuestItemGroupsAsync(Player player)
    {
        if (player.SelectedCharacter is not { } character)
        {
            return [];
        }

        if (player.Party is { } party)
        {
            return await party.GetQuestDropItemGroupsAsync(player).ConfigureAwait(false);
        }

        return character.GetQuestDropItemGroups();
    }

    private static bool IsGroupRelevant(MonsterDefinition monsterDefinition, DropItemGroup group)
    {
        if (group is null)
        {
            return false;
        }

        if (group.MinimumMonsterLevel.HasValue && monsterDefinition[Stats.Level] < group.MinimumMonsterLevel)
        {
            return false;
        }

        if (group.MaximumMonsterLevel.HasValue && monsterDefinition[Stats.Level] > group.MaximumMonsterLevel)
        {
            return false;
        }

        if (group.Monster is { } monster && !monster.Equals(monsterDefinition))
        {
            return false;
        }

        return true;
    }

    private void PartitionDropGroups(IEnumerable<DropItemGroup> groups, MonsterDefinition? monster = null)
    {
        foreach (var group in groups)
        {
            if (monster is not null && !IsGroupRelevant(monster, group))
            {
                continue;
            }

            if (group.Chance >= 1.0)
            {
                this._guaranteedDropGroups.Add(group);
            }
            else
            {
                this._chanceDropGroups.Add(group);
            }
        }
    }

    private Item? GenerateItemDrop(DropItemGroup selectedGroup, ICollection<ItemDefinition> possibleItems)
    {
        var item = selectedGroup.ItemType switch
        {
            SpecialItemType.Ancient => this.GenerateRandomAncient(),
            SpecialItemType.Excellent => this.GenerateRandomExcellentItem(possibleItems: possibleItems),
            _ => this.GenerateRandomItem(possibleItems),
        };

        if (item is null)
        {
            return null;
        }

        if (item.Durability == 0)
        {
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
        }

        if (selectedGroup is ItemDropItemGroup itemDropItemGroup)
        {
            item.Level = (byte)this._randomizer.NextInt(itemDropItemGroup.MinimumLevel, itemDropItemGroup.MaximumLevel + 1);
        }
        else if (selectedGroup.ItemLevel is { } itemLevel)
        {
            item.Level = itemLevel;
        }
        else
        {
            // no level defined, so it stays at 0.
        }

        item.Level = Math.Min(item.Level, item.Definition!.MaximumItemLevel);

        return item;
    }

    private void ApplyOption(Item item, ItemOptionDefinition option)
    {
        for (int i = 0; i < option.MaximumOptionsPerItem; i++)
        {
            if (this._randomizer.NextRandomBool(option.AddChance))
            {
                var remainingOptions = option.PossibleOptions.Where(possibleOption => item.ItemOptions.All(link => link.ItemOption != possibleOption));
                var newOption = remainingOptions.SelectRandom(this._randomizer);
                if (newOption is null)
                {
                    break;
                }

                var itemOptionLink = new ItemOptionLink
                {
                    ItemOption = newOption,
                    Level = newOption.LevelDependentOptions
                        .Select(ldo => ldo.Level)
                        .Concat(newOption.LevelDependentOptions.Count > 0 ? [1] : []) // For base def/dmg opts level 1 is not an ItemOptionOfLevel entry
                        .Distinct()
                        .Where(l => l <= this._maxItemOptionLevelDrop)
                        .DefaultIfEmpty(0)
                        .SelectRandom(),
                };
                item.ItemOptions.Add(itemOptionLink);
            }
        }
    }

    private Item? GenerateRandomItem(ICollection<ItemDefinition>? possibleItems)
    {
        if (possibleItems is null || possibleItems.Count == 0)
        {
            return null;
        }

        var item = new TemporaryItem
        {
            Definition = possibleItems.ElementAt(this._randomizer.NextInt(0, possibleItems.Count)),
        };

        this.ApplyRandomOptions(item);

        return item;
    }

    private void ApplyRandomAncientOption(Item item)
    {
        var ancientSet = item.Definition?.PossibleItemSetGroups
            .Where(g => g!.Options?.PossibleOptions.Any(o => object.Equals(o.OptionType, ItemOptionTypes.AncientOption)) ?? false)
            .SelectRandom(this._randomizer);
        if (ancientSet is null)
        {
            return;
        }

        var itemOfSet = ancientSet.Items.First(i => object.Equals(i.ItemDefinition, item.Definition));
        item.ItemSetGroups.Add(itemOfSet);
        if (itemOfSet.BonusOption is { } bonusOption) // for example: +5str or +10str
        {
            var bonusOptionLink = new ItemOptionLink();
            bonusOptionLink.ItemOption = bonusOption;
            bonusOptionLink.Level = bonusOption.LevelDependentOptions.Select(o => o.Level).SelectRandom();
            item.ItemOptions.Add(bonusOptionLink);
        }
    }

    private void AddRandomExcOptions(Item item)
    {
        var possibleItemOptions = item.Definition!.PossibleItemOptions;
        var excellentOptions = possibleItemOptions.FirstOrDefault(
            o => o.PossibleOptions.Any(p => object.Equals(p.OptionType, ItemOptionTypes.Excellent)));

        if (excellentOptions is null)
        {
            return;
        }

        var existingOptionCount = item.ItemOptions.Count(o => object.Equals(o.ItemOption?.OptionType, ItemOptionTypes.Excellent));

        for (int i = existingOptionCount; i < excellentOptions.MaximumOptionsPerItem; i++)
        {
            if (i == 0)
            {
                // The first option is always added without a chance
                var itemOptionLink = new ItemOptionLink();
                itemOptionLink.ItemOption = excellentOptions.PossibleOptions.SelectRandom(this._randomizer);
                if (itemOptionLink.ItemOption is not null)
                {
                    item.ItemOptions.Add(itemOptionLink);
                    existingOptionCount++;
                }

                continue;
            }

            if (this._randomizer.NextRandomBool(excellentOptions.AddChance))
            {
                var option = excellentOptions.PossibleOptions.SelectRandom(this._randomizer);
                while (item.ItemOptions.Any(o => object.Equals(o.ItemOption, option)))
                {
                    option = excellentOptions.PossibleOptions.SelectRandom(this._randomizer);
                }

                if (option is not null)
                {
                    var itemOptionLink = new ItemOptionLink();
                    itemOptionLink.ItemOption = option;
                    item.ItemOptions.Add(itemOptionLink);
                }
            }
        }
    }

    private Item? GenerateItemDropOrMoney(MonsterDefinition monster, DropItemGroup selectedGroup, int gainedExperience, out uint? droppedMoney)
    {
        droppedMoney = null;

        if (selectedGroup.PossibleItems?.Count > 0)
        {
            var isDropSpecificForMonster = monster.DropItemGroups.Contains(selectedGroup);

            if (isDropSpecificForMonster)
            {
                return this.GenerateItemDrop(selectedGroup, selectedGroup.PossibleItems);
            }
            else
            {
                var monsterLevel = (int)monster[Stats.Level];
                List<ItemDefinition> filteredPossibleItems;

                if (selectedGroup.ItemType == SpecialItemType.Jewel)
                {
                    filteredPossibleItems = [.. selectedGroup.PossibleItems.Where(it => it.DropLevel <= monsterLevel)];

                    if (monsterLevel > JewelOfChaosMaxMonsterLevel)
                    {
                        // Jewel of Chaos doesn't drop after a certain monster level
                        filteredPossibleItems.RemoveAll(it => it.Group == JewelOfChaosGroup && it.Number == JewelOfChaosNumber);
                    }
                }
                else
                {
                    filteredPossibleItems = [.. selectedGroup.PossibleItems.Where(it => it.DropLevel == 0 || (it.DropLevel <= monsterLevel && it.DropLevel > monsterLevel - DropLevelMaxGap))];
                }

                return this.GenerateItemDrop(selectedGroup, filteredPossibleItems);
            }
        }

        switch (selectedGroup.ItemType)
        {
            case SpecialItemType.Ancient:
                return this.GenerateRandomAncient();
            case SpecialItemType.Excellent:
                return this.GenerateRandomExcellentItem((int)monster[Stats.Level]);
            case SpecialItemType.RandomItem:
                return this.GenerateRandomItem((int)monster[Stats.Level], false);
            case SpecialItemType.SocketItem:
                return this.GenerateRandomItem((int)monster[Stats.Level], true);
            case SpecialItemType.Money:
                droppedMoney = (uint)(gainedExperience + BaseMoneyDrop);
                return null;
            default:
                // none
                return null;
        }
    }

    private DropItemGroup? SelectRandomGroup(IEnumerable<DropItemGroup> groups, double totalChance)
    {
        var lot = this._randomizer.NextDouble();
        if (totalChance > 1.0)
        {
            lot *= totalChance;
        }

        foreach (var group in groups)
        {
            if (lot > group.Chance)
            {
                lot -= group.Chance;
            }
            else
            {
                return group;
            }
        }

        return null;
    }

    private IList<ItemDefinition>? GetPossibleList(int monsterLevel, bool socketItems = false)
    {
        if (monsterLevel is < byte.MinValue or > byte.MaxValue)
        {
            return null;
        }

        return this._droppableItemsPerMonsterLevel[monsterLevel]
            ??= (from it in this._droppableItems
                 where (it.DropLevel <= monsterLevel)
                       && (it.DropLevel > monsterLevel - DropLevelMaxGap)
                       && (!socketItems || it.MaximumSockets > 0)
                 select it).ToList();
    }
}