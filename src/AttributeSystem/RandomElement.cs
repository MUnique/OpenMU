// <copyright file="RandomElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An element with a constant value.
/// </summary>
public class RandomElement : IElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RandomElement" /> class.
    /// </summary>
    /// <param name="minValue">The min value.</param>
    /// <param name="maxValue">The max value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    public RandomElement(float minValue, float maxValue, AggregateType aggregateType = AggregateType.AddRaw)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
        this.AggregateType = aggregateType;
    }

    /// <summary>
    /// Never occurs, so the implementation is empty.
    /// </summary>
    public event EventHandler? ValueChanged
    {
        add
        {
            // do nothing, as the value never changes.
        }

        remove
        {
            // do nothing, as the value never changes.
        }
    }

    /// <summary>
    /// Gets or sets the min value.
    /// </summary>
    public float MinValue { get; set; }

    /// <summary>
    /// Gets or sets the max value.
    /// </summary>
    public float MaxValue { get; set; }

    /// <inheritdoc/>
    public float Value
    {
        get
        {
            double randomValue = Random.Shared.NextDouble();
            return (float)(this.MinValue + (randomValue * (this.MaxValue - this.MinValue)));
        }
    }

    /// <inheritdoc/>
    public AggregateType AggregateType { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Value} ({this.AggregateType})";
    }
}