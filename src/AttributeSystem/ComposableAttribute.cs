// <copyright file="ComposableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute which is a composition of elements.
/// </summary>
public class ComposableAttribute : BaseAttribute, IComposableAttribute
{
    private readonly IList<IElement> _elementList;

    private float? _cachedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposableAttribute"/> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    public ComposableAttribute(AttributeDefinition definition)
        : base(definition, AggregateType.AddRaw)
    {
        this._elementList = new List<IElement>();
    }

    /// <inheritdoc/>
    public IEnumerable<IElement> Elements => this._elementList;

    /// <inheritdoc/>
    public override float Value => this._cachedValue ?? this.GetAndCacheValue();

    /// <inheritdoc/>
    public IComposableAttribute AddElement(IElement element)
    {
        this._elementList.Add(element);
        element.ValueChanged += this.ElementChanged;
        this.ElementChanged(element, EventArgs.Empty);

        return this;
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element)
    {
        if (this._elementList.Remove(element))
        {
            element.ValueChanged -= this.ElementChanged;
            this.ElementChanged(element, EventArgs.Empty);
        }
    }

    private float GetAndCacheValue()
    {
        this._cachedValue = (this.Elements.Where(e => e.AggregateType == AggregateType.AddRaw).Sum(e => e.Value)
                            * this.Elements.Where(e => e.AggregateType == AggregateType.Multiplicate).Select(e => e.Value).Concat(Enumerable.Repeat(1.0F, 1)).Aggregate((a, b) => a * b))
                           + this.Elements.Where(e => e.AggregateType == AggregateType.AddFinal).Sum(e => e.Value);

        return this._cachedValue.Value;
    }

    private void ElementChanged(object? sender, EventArgs eventArgs)
    {
        this._cachedValue = null;
        this.RaiseValueChanged();
    }
}