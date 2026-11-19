// <copyright file="WebsiteItemDetails.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;

/// <summary>
/// Gets the details of an equipped item for a website: the file name of its picture, its ancient set and its structured options.
/// </summary>
/// <remarks>
/// The names are in the neutral language (english) of the configuration, like the names of the other website results.
/// The website builds the texts of the options itself, from the structured options.
/// </remarks>
internal static class WebsiteItemDetails
{
    /// <summary>
    /// The inventory slot of the wings, whose picture doesn't depend on the item level.
    /// </summary>
    private const int WingsSlot = 7;

    /// <summary>
    /// The inventory slot of the pet, whose picture doesn't depend on the item level.
    /// </summary>
    private const int PetSlot = 8;

    /// <summary>
    /// Maps the item level to the level in the file name of the item picture, like the item pictures of the item editor
    /// of the admin panel: the client shows a new glow only every second level from +3.
    /// </summary>
    private static readonly int[] PictureLevels = [0, 0, 0, 3, 3, 5, 5, 7, 7, 9, 9, 11, 11, 13, 13, 15, 15];

    /// <summary>
    /// Gets the file name of the picture of an item, e.g. <c>item_0_16_13_e.png</c>,
    /// as the pictures are named in <c>src/Web/ItemEditor/wwwroot/img/items</c>.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The file name of the picture.</returns>
    internal static string GetPictureName(Item item)
    {
        var definition = item.Definition!;
        var pictureLevel = item.ItemSlot is WingsSlot or PetSlot || item.Level < 0 || item.Level >= PictureLevels.Length
            ? 0
            : PictureLevels[item.Level];
        var suffix = GetAncientSetName(item) is not null ? "_a" : (definition.Group < 12 && item.IsExcellent() ? "_e" : string.Empty);
        return $"item_{definition.Group}_{definition.Number}_{pictureLevel}{suffix}.png";
    }

    /// <summary>
    /// Gets the name of the ancient set of an item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The name of the ancient set; <c>null</c>, if the item isn't ancient.</returns>
    internal static string? GetAncientSetName(Item item)
        => item.ItemSetGroups.FirstOrDefault(set => set.AncientSetDiscriminator > 0)?.ItemSetGroup?.Name.ValueInNeutralLanguage;

    /// <summary>
    /// Gets the structured options of an item, one entry per option link, ordered by option type name and number.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The options of the item.</returns>
    internal static IReadOnlyList<WebsiteItemOption> GetOptions(Item item)
    {
        return item.ItemOptions
            .Select(link => CreateOption(item, link))
            .OrderBy(option => option.Type, StringComparer.Ordinal)
            .ThenBy(option => option.Number)
            .ToList();
    }

    /// <summary>
    /// Creates the structured option of an option link, with the values of the power-up of its level.
    /// </summary>
    /// <param name="item">The item, whose level selects the power-up of options with <see cref="LevelType.ItemLevel"/>.</param>
    /// <param name="link">The option link of the item.</param>
    /// <returns>The structured option.</returns>
    private static WebsiteItemOption CreateOption(Item item, ItemOptionLink link)
    {
        var option = link.ItemOption;
        var powerUp = ResolvePowerUp(item, link);
        var related = powerUp?.Boost?.RelatedValues?.FirstOrDefault();
        return new WebsiteItemOption(
            option?.OptionType?.Name.ValueInNeutralLanguage ?? string.Empty,
            option?.Number ?? 0,
            link.Level,
            powerUp?.TargetAttribute?.Designation,
            powerUp?.Boost?.ConstantValue.Value,
            powerUp?.Boost?.ConstantValue.AggregateType.ToString(),
            related?.InputAttribute?.Designation,
            related?.InputOperator.ToString(),
            related?.InputOperand);
    }

    /// <summary>
    /// Gets the power-up of an option link: the one of its level, if the option is level dependent, else the own one.
    /// Like the game logic, options with <see cref="LevelType.ItemLevel"/> (e.g. wing options) use the level of the item.
    /// </summary>
    /// <param name="item">The item, whose level selects the power-up of options with <see cref="LevelType.ItemLevel"/>.</param>
    /// <param name="link">The option link of the item.</param>
    /// <returns>The power-up; <c>null</c>, if the option has none.</returns>
    private static PowerUpDefinition? ResolvePowerUp(Item item, ItemOptionLink link)
    {
        var option = link.ItemOption;
        var optionLevel = option?.LevelType == LevelType.ItemLevel ? item.Level : link.Level;
        return option?.LevelDependentOptions.FirstOrDefault(level => level.Level == optionLevel)?.PowerUpDefinition
               ?? option?.PowerUpDefinition;
    }
}
