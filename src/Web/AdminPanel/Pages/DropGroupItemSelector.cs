// <copyright file="DropGroupItemSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Pages;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Categories understood by the drop-group editor.
/// </summary>
public enum DropGroupItemCategory
{
    /// <summary>Every item matching the text/group filters.</summary>
    Any,

    /// <summary>Items belonging to the selected set group.</summary>
    Set,

    /// <summary>Items which can receive the level 380 Guardian option.</summary>
    Guardian380,

    /// <summary>Items which can receive Excellent options.</summary>
    Excellent,

    /// <summary>Items which belong to an ancient set.</summary>
    Ancient,

    /// <summary>Items with socket support.</summary>
    Socket,

    /// <summary>Items equipped in either hand.</summary>
    Weapons,

    /// <summary>Items equipped in an armor slot.</summary>
    Armor,

    /// <summary>Wing definitions.</summary>
    Wings,
}

/// <summary>
/// Pure item selection rules shared by the Batch Operations page and tests.
/// </summary>
public static class DropGroupItemSelector
{
    /// <summary>
    /// Filters item definitions using the same rules displayed by the drop-group editor.
    /// </summary>
    public static IReadOnlyList<ItemDefinition> Filter(
        IEnumerable<ItemDefinition> items,
        string? nameFilter,
        int? group,
        Guid? setId,
        DropGroupItemCategory category)
    {
        ArgumentNullException.ThrowIfNull(items);

        IEnumerable<ItemDefinition> result = items;
        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            result = result.Where(item => (item.Name.ToString() ?? string.Empty).Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (group.HasValue)
        {
            result = result.Where(item => item.Group == group.Value);
        }

        if (category == DropGroupItemCategory.Set)
        {
            result = setId.HasValue
                ? result.Where(item => item.PossibleItemSetGroups.Any(set => set.GetId() == setId.Value))
                : [];
        }

        result = category switch
        {
            DropGroupItemCategory.Guardian380 => result.Where(HasOptionType(ItemOptionTypes.GuardianOption)),
            DropGroupItemCategory.Excellent => result.Where(HasOptionType(ItemOptionTypes.Excellent)),
            DropGroupItemCategory.Ancient => result.Where(IsAncientItem),
            DropGroupItemCategory.Socket => result.Where(item => item.MaximumSockets > 0),
            DropGroupItemCategory.Weapons => result.Where(IsWeapon),
            DropGroupItemCategory.Armor => result.Where(IsArmor),
            DropGroupItemCategory.Wings => result.Where(item => item.IsWing()),
            _ => result,
        };

        return result
            .OrderBy(item => item.Group)
            .ThenBy(item => item.Number)
            .ToList();
    }

    /// <summary>
    /// Determines whether a definition can receive a specific option type.
    /// </summary>
    public static bool HasOptionType(ItemDefinition item, ItemOptionType optionType)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(optionType);
        return item.PossibleItemOptions
            .SelectMany(option => option.PossibleOptions)
            .Any(option => option.OptionType == optionType);
    }

    /// <summary>
    /// Determines whether a definition is linked to an ancient set entry.
    /// </summary>
    public static bool IsAncientItem(ItemDefinition item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var id = item.GetId();
        return item.PossibleItemSetGroups
            .SelectMany(set => set.Items)
            .Any(entry => entry.AncientSetDiscriminator > 0 && entry.ItemDefinition?.GetId() == id);
    }

    private static bool IsWeapon(ItemDefinition item)
        => item.ItemSlot?.ItemSlots.Any(slot => slot is 0 or 1) is true;

    private static bool IsArmor(ItemDefinition item)
        => item.ItemSlot?.ItemSlots.Any(slot => slot is >= 2 and <= 6) is true;

    private static Func<ItemDefinition, bool> HasOptionType(ItemOptionType optionType)
        => item => HasOptionType(item, optionType);
}
