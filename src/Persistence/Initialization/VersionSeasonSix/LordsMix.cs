// <copyright file="LordsMix.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

/// <summary>
/// The Lord's Mix of the castle senior NPC, which only the guild master of the guild which owns the castle can use,
/// once per day.
/// </summary>
/// <remarks>
/// The number, the required items and the price are the ones of the Season 6 client (mix 18 of the castle senior).
/// The result list is the one of a fan guide (guiamuonline.com/lordmix, 2010) which matches mixes of Global MU Online
/// in videos of 2013 to 2019: excellent weapons and armor with two to four excellent options, and a few normal ones.
/// Rage Fighter items aren't in that list.
/// </remarks>
internal static class LordsMix
{
    /// <summary>
    /// The number of the crafting, which the client sends for the mix of the castle senior.
    /// </summary>
    internal const byte CraftingNumber = 18;

    /// <summary>
    /// The number of successful Lord's Mixes per <see cref="LimitPeriod"/>.
    /// </summary>
    internal const int Limit = 1;

    /// <summary>
    /// The period of the <see cref="Limit"/>.
    /// </summary>
    internal const LordsMixLimitPeriod LimitPeriod = LordsMixLimitPeriod.Day;

    private const int Price = 1_000_000_000;

    private const byte LuckOptionChance = 20;

    /// <summary>
    /// The chance of the skill of a normal result item, the same as for a dropped item
    /// (<c>DefaultDropGenerator.SkillDropChancePercent</c>). Excellent result items always have their skill.
    /// </summary>
    private const byte SkillChance = 50;

    private const short ExcellentOptionCountTableNumber = 3;

    private static readonly (int Value, double Weight)[] ExcellentOptionCountWeights = [(2, 1), (3, 1), (4, 1)];

    /// <summary>
    /// The excellent weapons and shields: (group, number).
    /// </summary>
    private static readonly (byte Group, short Number)[] ExcellentWeapons =
    [
        (0, 16), // Sword of Destruction
        (0, 20), // Knight Blade
        (0, 21), // Dark Reign Blade
        (0, 24), // Daybreak
        (0, 25), // Sword Dancer
        (0, 31), // Rune Blade
        (2, 11), // Lord Scepter
        (2, 12), // Great Lord Scepter
        (2, 15), // Shining Scepter
        (3, 10), // Dragon Spear
        (4, 19), // Great Reign Crossbow
        (4, 20), // Arrow Viper Bow
        (4, 22), // Albatross Bow
        (5, 6), // Staff of Resurrection
        (5, 9), // Dragon Soul Staff
        (5, 11), // Staff of Kundun
        (5, 13), // Platina Staff
        (6, 14), // Legendary Shield
    ];

    /// <summary>
    /// The armor sets of which all existing pieces (helm, armor, pants, gloves, boots) are excellent results.
    /// </summary>
    private static readonly short[] ExcellentArmorSets =
    [
        21, // Great Dragon
        22, // Dark Soul
        23, // Hurricane
        24, // Red Spirit
        28, // Dark Master
        34, // Ashcrow
        35, // Eclipse
        36, // Iris
        37, // Valiant
        38, // Glorious
    ];

    /// <summary>
    /// The normal (not excellent) results: (group, number).
    /// </summary>
    private static readonly (byte Group, short Number)[] NormalItems =
    [
        (0, 20), // Knight Blade
        (0, 21), // Dark Reign Blade
        (2, 12), // Great Lord Scepter
        (4, 20), // Arrow Viper Bow
        (5, 11), // Staff of Kundun, seen in a video; the guide has no normal staff
        (8, 21), // Great Dragon Armor
        (8, 22), // Dark Soul Armor
        (8, 23), // Hurricane Armor
        (8, 24), // Red Spirit Armor
        (8, 28), // Dark Master Armor
    ];

    /// <summary>
    /// Adds the Lord's Mix to the castle senior NPC, if it doesn't have it yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void AddIfMissing(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(monster => monster.NpcWindow == NpcWindow.CastleSeniorNPC) is not { } senior
            || senior.ItemCraftings.Any(crafting => crafting.Number == CraftingNumber))
        {
            return;
        }

        senior.ItemCraftings.Add(CreateCrafting(context, gameConfiguration));
    }

    private static ItemCrafting CreateCrafting(IContext context, GameConfiguration gameConfiguration)
    {
        var crafting = context.CreateNew<ItemCrafting>();
        crafting.Name = LocalizedString.FromResource(() => CraftingNames.LordsMix);
        crafting.Number = CraftingNumber;
        crafting.ItemCraftingHandlerClassName = typeof(LordsMixCrafting).FullName!;

        var settings = context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = settings;
        settings.Money = Price;
        settings.SuccessPercent = 100;
        settings.ResultItemSelect = ResultItemSelection.Any;
        settings.ResultItemLuckOptionChance = LuckOptionChance;
        settings.ResultItemSkillChance = SkillChance;
        settings.ResultItemExcellentOptionCountTable = WeightedValueTableFactory.GetOrCreate(
            context,
            gameConfiguration,
            ExcellentOptionCountTableNumber,
            LocalizedString.FromResource(() => WeightedValueTableNames.LordsMixExcellentOptionCount),
            LocalizedString.FromResource(() => WeightedValueTableNames.LordsMixExcellentOptionCountDescription),
            ExcellentOptionCountWeights);

        settings.RequiredItems.Add(CreateRequiredItem(context, GetItem(gameConfiguration, 14, 31), 30, 0)); // Jewel of Guardian
        settings.RequiredItems.Add(CreateRequiredItem(context, GetItem(gameConfiguration, 12, 30), 1, 2)); // Packed Jewel of Bless, 30 jewels
        settings.RequiredItems.Add(CreateRequiredItem(context, GetItem(gameConfiguration, 12, 31), 1, 2)); // Packed Jewel of Soul, 30 jewels

        foreach (var item in GetExcellentItems(gameConfiguration))
        {
            settings.ResultItems.Add(CreateResultItem(context, item, 100));
        }

        foreach (var (group, number) in NormalItems)
        {
            settings.ResultItems.Add(CreateResultItem(context, GetItem(gameConfiguration, group, number), 0));
        }

        return crafting;
    }

    private static ItemCraftingRequiredItem CreateRequiredItem(IContext context, ItemDefinition item, byte amount, byte level)
    {
        var requiredItem = context.CreateNew<ItemCraftingRequiredItem>();
        requiredItem.PossibleItems.Add(item);
        requiredItem.MinimumAmount = amount;
        requiredItem.MaximumAmount = amount;
        requiredItem.MinimumItemLevel = level;
        requiredItem.MaximumItemLevel = level;
        requiredItem.SuccessResult = MixResult.Disappear;
        requiredItem.FailResult = MixResult.Disappear;
        return requiredItem;
    }

    private static ItemCraftingResultItem CreateResultItem(IContext context, ItemDefinition item, byte excellentOptionChance)
    {
        var resultItem = context.CreateNew<ItemCraftingResultItem>();
        resultItem.ItemDefinition = item;
        resultItem.ExcellentOptionChance = excellentOptionChance;
        return resultItem;
    }

    /// <summary>
    /// Gets the excellent weapons and all existing pieces of the excellent armor sets.
    /// </summary>
    private static IEnumerable<ItemDefinition> GetExcellentItems(GameConfiguration gameConfiguration)
    {
        foreach (var (group, number) in ExcellentWeapons)
        {
            yield return GetItem(gameConfiguration, group, number);
        }

        foreach (var setNumber in ExcellentArmorSets)
        {
            for (byte group = (byte)ItemGroups.Helm; group <= (byte)ItemGroups.Boots; group++)
            {
                if (gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == setNumber) is { } piece)
                {
                    yield return piece;
                }
            }
        }
    }

    private static ItemDefinition GetItem(GameConfiguration gameConfiguration, byte group, short number)
    {
        return gameConfiguration.Items.FirstOrDefault(item => item.Group == group && item.Number == number)
               ?? throw new InvalidOperationException($"The item {group}/{number} of the Lord's Mix doesn't exist.");
    }
}
