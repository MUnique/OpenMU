// <copyright file="ElementAggregate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// Combines the values of elements as a <see cref="ComposableAttribute"/> does:
/// (added values + highest maximum) * factors + finally added values, limited to the maximum value.
/// Only factors give the product of the factors. Without elements, the value is 0.
/// </summary>
/// <remarks>
/// It combines any elements, so also a part of an attribute's elements, e.g. the ones of one magic effect.
/// A ref struct, because it only lives on the stack during one calculation and has no identity or equality.
/// </remarks>
public ref struct ElementAggregate
{
    // The sums are doubles, as in Enumerable.Sum for floats, so the values stay the same as before.
    private double _raw;
    private double _final;
    private float _factor;
    private float _maximum;
    private int _count;
    private bool _hasRaw;
    private bool _hasFactor;
    private bool _hasFinal;
    private bool _hasMaximum;
    private bool _hasOther;

    /// <summary>
    /// Adds the value of an element.
    /// </summary>
    /// <param name="element">The element.</param>
    public void Add(IElement element) => this.Add(element.AggregateType, element.Value);

    /// <summary>
    /// Adds a value.
    /// </summary>
    /// <param name="aggregateType">The aggregate type of the value.</param>
    /// <param name="value">The value.</param>
    public void Add(AggregateType aggregateType, float value)
    {
        if (this._count == 0)
        {
            this._factor = 1;
        }

        switch (aggregateType)
        {
            case AggregateType.AddRaw:
                this._raw += value;
                this._hasRaw = true;
                break;
            case AggregateType.Multiplicate:
                this._factor *= value;
                this._hasFactor = true;
                break;
            case AggregateType.AddFinal:
                this._final += value;
                this._hasFinal = true;
                break;
            case AggregateType.Maximum:
                this._maximum = this._hasMaximum ? Math.Max(this._maximum, value) : value;
                this._hasMaximum = true;
                break;
            default:
                // An unknown type adds nothing, but the value is no longer only factors.
                this._hasOther = true;
                break;
        }

        this._count++;
    }

    /// <summary>
    /// Determines whether a value of the aggregate type was added.
    /// </summary>
    /// <param name="aggregateType">The aggregate type.</param>
    /// <returns><c>true</c>, if a value of the aggregate type was added.</returns>
    public readonly bool Contains(AggregateType aggregateType)
    {
        return aggregateType switch
        {
            AggregateType.AddRaw => this._hasRaw,
            AggregateType.Multiplicate => this._hasFactor,
            AggregateType.AddFinal => this._hasFinal,
            AggregateType.Maximum => this._hasMaximum,
            _ => this._hasOther,
        };
    }

    /// <summary>
    /// Gets the combined value.
    /// </summary>
    /// <param name="definition">The definition of the attribute, whose <see cref="AttributeDefinition.MaximumValue"/> limits the value.</param>
    /// <param name="maximumValue">An additional maximum value, e.g. the one of a <see cref="ComposableAttribute"/>.</param>
    /// <returns>The combined value.</returns>
    public readonly float GetValue(AttributeDefinition? definition = null, float? maximumValue = null)
    {
        if (this._count == 0)
        {
            return 0;
        }

        var onlyFactors = !this._hasRaw && !this._hasFinal && !this._hasMaximum && !this._hasOther;
        var raw = onlyFactors ? 1 : (float)this._raw + (this._hasMaximum ? this._maximum : 0);
        var value = (raw * this._factor) + (float)this._final;
        if (maximumValue.HasValue)
        {
            value = Math.Min(maximumValue.Value, value);
        }

        if (definition?.MaximumValue is { } definitionMaximum)
        {
            value = Math.Min(definitionMaximum, value);
        }

        return value;
    }
}
