// <copyright file="KalimaConstants.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Constants for the Kalima map.
/// </summary>
public static class KalimaConstants
{
    /// <summary>
    /// The symbol of kundun item group.
    /// </summary>
    internal const byte SymbolOfKundunGroup = 14;

    /// <summary>
    /// The symbol of kundun item number.
    /// </summary>
    internal const byte SymbolOfKundunNumber = 29;

    /// <summary>
    /// The lost map item group.
    /// </summary>
    internal const byte LostMapGroup = 14;

    /// <summary>
    /// The lost map item number.
    /// </summary>
    internal const byte LostMapNumber = 28;

    /// <summary>
    /// Determines whether the specified item is a lost map.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a lost map; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLostMap(this Item item)
    {
        return item.Definition.IsLostMap();
    }

    /// <summary>
    /// Determines whether the specified item is a lost map.
    /// </summary>
    /// <param name="itemDefinition">The item definition.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a lost map; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsLostMap(this ItemDefinition? itemDefinition)
    {
        return itemDefinition is { Group: LostMapGroup, Number: LostMapNumber };
    }

    /// <summary>
    /// Determines whether the specified item is a symbol of kundun.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is a symbol of kundun; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSymbolOfKundun(this Item item)
    {
        return item.Definition is { Group: SymbolOfKundunGroup, Number: SymbolOfKundunNumber };
    }
}