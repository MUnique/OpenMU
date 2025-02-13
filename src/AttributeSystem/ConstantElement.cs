// <copyright file="ConstantElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An element with a constant value.
/// </summary>
public class ConstantElement : IElement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConstantElement" /> class.
    /// </summary>
    /// <param name="value">The constant value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <param name="stage">The calculation stage.</param>
    public ConstantElement(float value, AggregateType aggregateType = AggregateType.AddRaw, byte stage = 0)
    {
        this.Value = value;
        this.AggregateType = aggregateType;
        this.Stage = stage;
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

    /// <inheritdoc/>
    public float Value { get; }

    /// <inheritdoc/>
    public AggregateType AggregateType { get; }

    /// <inheritdoc />
    public byte Stage { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Value} ({this.AggregateType}) @{this.Stage}";
    }
}