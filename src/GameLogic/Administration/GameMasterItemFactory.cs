// <copyright file="GameMasterItemFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;

/// <summary>Shared item construction for GM commands and live administration.</summary>
public static class GameMasterItemFactory
{
    /// <summary>Validates item options against their configured definitions.</summary>
    /// <param name="definition">The configured item definition.</param>
    /// <param name="item">The requested item.</param>
    /// <returns>Whether every requested option can be represented by the definition.</returns>
    public static bool IsValid(ItemDefinition definition, ItemSpecification item)
    {
        if (item.Level > definition.MaximumItemLevel || (item.Skill && definition.Skill is null))
        {
            return false;
        }

        var options = definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions).ToArray();
        if (item.Luck && !options.Any(o => o.OptionType == ItemOptionTypes.Luck))
        {
            return false;
        }

        if (item.OptionLevel > 0 && !IsValidOption(options, item.OptionLevel))
        {
            return false;
        }

        var excellentMask = options.Where(o => o.OptionType == ItemOptionTypes.Excellent && o.Number is >= 1 and <= 8)
            .Aggregate(0, (mask, o) => mask | (1 << (o.Number - 1)));
        if ((item.ExcellentOptions & ~excellentMask) != 0)
        {
            return false;
        }

        return item.AncientSet == 0 || (item.AncientBonusLevel is 1 or 2
            && definition.PossibleItemSetGroups.Any(g => g.Items.Any(i => i.ItemDefinition == definition && i.AncientSetDiscriminator == item.AncientSet)));
    }

    /// <summary>Creates a detached item after the caller validates the specification.</summary>
    /// <param name="itemDefinition">The item definition.</param>
    /// <param name="arguments">The item specification.</param>
    /// <returns>The detached item, suitable for a drop or live inventory insertion.</returns>
    public static TemporaryItem CreateItem(ItemDefinition itemDefinition, ItemSpecification arguments)
    {
        var item = new TemporaryItem();
        item.Definition = itemDefinition;
        item.Durability = item.IsStackable() ? 1 : item.Definition.Durability;
        item.HasSkill = item.Definition.Skill != null && arguments.Skill;
        item.Level = arguments.Level;
        item.SocketCount = item.Definition.MaximumSockets;

        AddOption(item, arguments);
        AddLuckOption(item, arguments);
        AddExcellentOptions(item, arguments);
        AddAncientBonusOption(item, arguments);

        return item;
    }

    private static bool IsValidOption(IncreasableItemOption[] options, byte level)
    {
        var normal = options.Where(o => o.OptionType == ItemOptionTypes.Option).ToArray();
        var fixedOptions = normal.Where(o => o.IsFixedOption()).ToArray();
        if (fixedOptions.Length > 0)
        {
            var allowed = fixedOptions.Aggregate(0, (mask, o) => mask | o.Number);
            return (level & ~allowed) == 0;
        }

        return normal.Any(o => level == 1 || o.LevelDependentOptions.Any(l => l.Level == level));
    }

    private static void AddOption(TemporaryItem item, ItemSpecification arguments)
    {
        if (item.Definition != null && arguments.OptionLevel > 0)
        {
            var allOptions = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Option);

            // Fixed options (e.g. of the Dinorant) are selected by their bits, which are their numbers.
            var fixedOptions = allOptions.Where(o => o.IsFixedOption()).ToList();
            if (fixedOptions.Count > 0)
            {
                foreach (var fixedOption in fixedOptions.Where(o => (arguments.OptionLevel & o.Number) != 0))
                {
                    item.ItemOptions.Add(new ItemOptionLink { ItemOption = fixedOption });
                }
            }
            else
            {
                var optionLink = new ItemOptionLink { ItemOption = allOptions.First(), Level = arguments.OptionLevel };
                item.ItemOptions.Add(optionLink);
            }
        }
    }

    private static void AddLuckOption(TemporaryItem item, ItemSpecification arguments)
    {
        if (item.Definition != null && arguments.Luck)
        {
            var optionLink = new ItemOptionLink
            {
                ItemOption = item.Definition.PossibleItemOptions
                    .SelectMany(o => o.PossibleOptions)
                    .First(o => o.OptionType == ItemOptionTypes.Luck),
            };

            item.ItemOptions.Add(optionLink);
        }
    }

    private static void AddExcellentOptions(TemporaryItem item, ItemSpecification arguments)
    {
        if (item.Definition != null && arguments.ExcellentOptions > 0)
        {
            var excellentOptions = item.Definition.PossibleItemOptions
                .SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .Where(o => ((1 << (o.Number - 1)) & arguments.ExcellentOptions) > 0)
                .ToList();

            ushort appliedOptions = 0;
            foreach (var excellentOption in excellentOptions)
            {
                var optionLink = new ItemOptionLink { ItemOption = excellentOption };
                item.ItemOptions.Add(optionLink);
                appliedOptions++;
            }

            // every excellent item has skill (if is in item definition)
            item.HasSkill = appliedOptions > 0 && item.Definition.Skill != null;
        }
    }

    private static void AddAncientBonusOption(TemporaryItem item, ItemSpecification arguments)
    {
        if (item.Definition != null && arguments.AncientSet > 0
                                    && item.Definition.PossibleItemSetGroups.FirstOrDefault(g => g.Items.Any(i => i.ItemDefinition == item.Definition && i.AncientSetDiscriminator == arguments.AncientSet)) is { } ancientSet
                                    && ancientSet.Items.FirstOrDefault(i => i.ItemDefinition == item.Definition) is { } itemOfItemSet)
        {
            var optionLink = new ItemOptionLink { ItemOption = itemOfItemSet.BonusOption, Level = arguments.AncientBonusLevel };
            item.ItemOptions.Add(optionLink);
            item.ItemSetGroups.Add(itemOfItemSet);
        }
    }
}
