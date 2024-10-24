// <copyright file="StatAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute which represents an increasable stat attribute (e.g. by level-up points).
/// </summary>
public class StatAttribute : BaseStatAttribute
{
    private float _statValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatAttribute"/> class.
    /// </summary>
    public StatAttribute()
        : base(null!, AggregateType.AddRaw)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StatAttribute"/> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="baseValue">The base value.</param>
    public StatAttribute(AttributeDefinition definition, float baseValue)
        : base(definition, AggregateType.AddRaw)
    {
        this._statValue = baseValue;
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public new virtual float Value
    {
        get
        {
            if (this.Definition.MaximumValue.HasValue)
            {
                return Math.Min(this.Definition.MaximumValue.Value, this._statValue);
            }

            return this._statValue;
        }

        set
        {
            if (Math.Abs(this._statValue - value) > 0.01f)
            {
                this._statValue = value;
                this.RaiseValueChanged();
            }
        }
    }

    /// <inheritdoc/>
    protected override float ValueGetter => this._statValue;
}