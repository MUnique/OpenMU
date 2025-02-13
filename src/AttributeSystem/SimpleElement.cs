// <copyright file="SimpleElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A simple element with a variable value.
/// </summary>
public class SimpleElement : IElement
{
    private float _value;
    private AggregateType _aggregateType;
    private byte _stage;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleElement"/> class.
    /// </summary>
    public SimpleElement()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleElement"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <param name="stage">The calculation stage.</param>
    public SimpleElement(float value, AggregateType aggregateType, byte stage = 0)
    {
        this._value = value;
        this._aggregateType = aggregateType;
        this._stage = stage;
    }

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;

    /// <inheritdoc/>
    public virtual float Value
    {
        get => this._value;

        set
        {
            if (Math.Abs(this._value - value) > 0.00001f)
            {
                this._value = value;
                this.RaiseValueChanged();
            }
        }
    }

    /// <inheritdoc/>
    public AggregateType AggregateType
    {
        get => this._aggregateType;

        set
        {
            if (this._aggregateType != value)
            {
                this._aggregateType = value;
                this.RaiseValueChanged();
            }
        }
    }

    /// <inheritdoc />
    public byte Stage
    {
        get => this._stage;

        set
        {
            if (this._stage != value)
            {
                this._stage = value;
                this.RaiseValueChanged();
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Value} ({this.AggregateType}) @{this.Stage}";
    }

    /// <summary>
    /// Raises the value changed event.
    /// </summary>
    protected void RaiseValueChanged()
    {
        this.ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}