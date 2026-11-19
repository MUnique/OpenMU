// <copyright file="WeightedValue.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// A value of a <see cref="WeightedValueTable"/>, with its relative weight.
/// </summary>
[Cloneable]
public partial class WeightedValue
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets the weight of the <see cref="Value"/>, relative to the weights of the other values of the table.
    /// </summary>
    /// <remarks>
    /// The chance of a value is its weight divided by the sum of the weights of the table, so the weights
    /// don't need to add up to 100. A value with a weight of 0 is never selected.
    /// </remarks>
    public double Weight { get; set; }
}
