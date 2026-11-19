// <copyright file="SocketCountTables.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// The tables of the number of sockets of dropped socket items.
/// </summary>
/// <remarks>
/// In the original game, a socket item drops with one to three sockets, and only two-handed weapons with up to five.
/// The number is selected by weights, so that one socket is common and the maximum is rare. The weights are those of
/// the commonly used Season 6 emulator data; the cap at the first weight of 0 matches the behaviour of the original
/// server as seen in third-party sources. Neither is data of the original server.
/// </remarks>
internal static class SocketCountTables
{
    /// <summary>
    /// The maximum number of sockets of an item. The Season 6 client and protocol support up to five sockets.
    /// </summary>
    internal const int MaximumSocketCount = 5;

    private const short OneHandedTableNumber = 1;

    private const short TwoHandedTableNumber = 2;

    private static readonly (int SocketCount, double Weight)[] OneHandedWeights = [(1, 84), (2, 15.5), (3, 0.5), (4, 0), (5, 0)];

    private static readonly (int SocketCount, double Weight)[] TwoHandedWeights = [(1, 80), (2, 10), (3, 7), (4, 2.5), (5, 0.5)];

    /// <summary>
    /// Gets the tables of the game configuration, or creates them if they don't exist yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The tables for one-handed weapons, shields and armor, and for two-handed weapons.</returns>
    internal static (WeightedValueTable OneHanded, WeightedValueTable TwoHanded) GetOrCreate(IContext context, GameConfiguration gameConfiguration)
    {
        var oneHanded = GetOrCreate(
            context,
            gameConfiguration,
            OneHandedTableNumber,
            LocalizedString.FromResource(() => WeightedValueTableNames.SocketCountOneHanded),
            LocalizedString.FromResource(() => WeightedValueTableNames.SocketCountOneHandedDescription),
            OneHandedWeights);
        var twoHanded = GetOrCreate(
            context,
            gameConfiguration,
            TwoHandedTableNumber,
            LocalizedString.FromResource(() => WeightedValueTableNames.SocketCountTwoHanded),
            LocalizedString.FromResource(() => WeightedValueTableNames.SocketCountTwoHandedDescription),
            TwoHandedWeights);
        return (oneHanded, twoHanded);
    }

    /// <summary>
    /// Determines whether the item is a two-handed weapon, like the item initialization of the weapons does.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c>, if the item is a two-handed weapon; otherwise, <c>false</c>.</returns>
    internal static bool IsTwoHandedWeapon(ItemDefinition item)
    {
        return item.Group <= (byte)ItemGroups.Staff && item.Width == 2;
    }

    private static WeightedValueTable GetOrCreate(IContext context, GameConfiguration gameConfiguration, short tableNumber, LocalizedString name, LocalizedString description, (int SocketCount, double Weight)[] weights)
    {
        var id = GuidHelper.CreateGuid<WeightedValueTable>(tableNumber);
        if (gameConfiguration.WeightedValueTables.FirstOrDefault(t => t.GetId() == id) is { } existingTable)
        {
            return existingTable;
        }

        var table = context.CreateNew<WeightedValueTable>();
        table.SetGuid(tableNumber);
        table.Name = name;
        table.Description = description;
        foreach (var (socketCount, weight) in weights)
        {
            var weightedValue = context.CreateNew<WeightedValue>();
            weightedValue.SetGuid(tableNumber, (short)socketCount);
            weightedValue.Value = socketCount;
            weightedValue.Weight = weight;
            table.Values.Add(weightedValue);
        }

        gameConfiguration.WeightedValueTables.Add(table);
        return table;
    }
}
