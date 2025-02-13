// <copyright file="BaseAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The base class for an attribute.
/// </summary>
public abstract class BaseAttribute : IAttribute
{
    private AttributeDefinition _definition = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseAttribute"/> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    protected BaseAttribute(AttributeDefinition definition, AggregateType aggregateType)
    {
        this.Definition = definition;
        this.AggregateType = aggregateType;
    }

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;

    /// <inheritdoc/>
    public virtual AttributeDefinition Definition
    {
        get => this._definition;
        protected set => this._definition = value;
    }

    /// <inheritdoc/>
    public abstract float Value { get; }

    /// <inheritdoc/>
    public AggregateType AggregateType { get; }

    /// <inheritdoc />
    public byte Stage { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Definition?.Designation}: {this.Value}";
    }

    /// <summary>
    /// Raises the value changed event.
    /// </summary>
    protected void RaiseValueChanged()
    {
        this.ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}