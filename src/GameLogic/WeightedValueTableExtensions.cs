// <copyright file="WeightedValueTableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Extensions for the <see cref="WeightedValueTable"/>.
/// </summary>
public static class WeightedValueTableExtensions
{
    /// <summary>
    /// Selects a random value of the table, by the weights of its values.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="randomizer">The randomizer.</param>
    /// <param name="minimumValue">The minimum value which may be selected. Smaller values of the table are ignored.</param>
    /// <param name="maximumValue">The maximum value which may be selected. Greater values of the table are ignored.</param>
    /// <returns>
    /// The selected value; <c>null</c>, if the table has no value with a weight above 0 within the range.
    /// </returns>
    public static int? SelectRandomValue(this WeightedValueTable table, IRandomizer randomizer, int minimumValue, int maximumValue)
    {
        var totalWeight = 0.0;
        foreach (var weightedValue in table.Values)
        {
            if (IsSelectable(weightedValue, minimumValue, maximumValue))
            {
                totalWeight += weightedValue.Weight;
            }
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        var threshold = randomizer.NextDouble() * totalWeight;
        int? lastSelectableValue = null;
        foreach (var weightedValue in table.Values)
        {
            if (!IsSelectable(weightedValue, minimumValue, maximumValue))
            {
                continue;
            }

            if (threshold < weightedValue.Weight)
            {
                return weightedValue.Value;
            }

            threshold -= weightedValue.Weight;
            lastSelectableValue = weightedValue.Value;
        }

        // Only reached when rounding errors left a remainder of the threshold.
        return lastSelectableValue;
    }

    private static bool IsSelectable(WeightedValue weightedValue, int minimumValue, int maximumValue)
    {
        return weightedValue.Weight > 0
               && weightedValue.Value >= minimumValue
               && weightedValue.Value <= maximumValue;
    }
}
