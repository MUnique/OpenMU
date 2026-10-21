// <copyright file="ItemOptionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Extension methods for <see cref="IncreasableItemOption"/>.
/// </summary>
public static class ItemOptionExtensions
{
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

    /// <summary>
    /// Gets the bits of the fixed options of the item, which are the combined numbers of its fixed options.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The combined numbers of the fixed options of the item.</returns>
    public static int GetFixedOptionBits(this Item item)
    {
        return item.ItemOptions
            .Select(link => link.ItemOption)
            .Where(option => option?.IsFixedOption() is true)
            .Aggregate(0, (bits, option) => bits | option!.Number);
    }
}
