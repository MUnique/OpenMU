// <copyright file="WeightedValueTableFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Creates the <see cref="WeightedValueTable"/>s of the season 6 data, with fixed ids, so that the initialization and the
/// updates create each table only once.
/// </summary>
internal static class WeightedValueTableFactory
{
    /// <summary>
    /// Gets the table with the number from the game configuration, or creates it if it doesn't exist yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="tableNumber">The number of the table, which determines its id.</param>
    /// <param name="name">The name.</param>
    /// <param name="description">The description.</param>
    /// <param name="weights">The values and their weights.</param>
    /// <returns>The existing or created table.</returns>
    internal static WeightedValueTable GetOrCreate(IContext context, GameConfiguration gameConfiguration, short tableNumber, LocalizedString name, LocalizedString description, (int Value, double Weight)[] weights)
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
        foreach (var (value, weight) in weights)
        {
            var weightedValue = context.CreateNew<WeightedValue>();
            weightedValue.SetGuid(tableNumber, (short)value);
            weightedValue.Value = value;
            weightedValue.Weight = weight;
            table.Values.Add(weightedValue);
        }

        gameConfiguration.WeightedValueTables.Add(table);
        return table;
    }
}
