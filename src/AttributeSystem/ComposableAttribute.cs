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
    /// Initializes a new instance of the <see cref="ComposableAttribute" /> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    public ComposableAttribute(AttributeDefinition definition, AggregateType aggregateType = AggregateType.AddRaw)
        : base(definition, aggregateType)
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
        if (this._elementList.Count == 0)
        {
            this._cachedValue = 0;
            return 0;
        }

        var highestStage = this._elementList.Max(e => e.Stage);
        float newValue = 0;
        for (int i = 0; i <= highestStage; i++)
        {
            var stageElements = this._elementList.Where(e => e.Stage == i);

            var rawValues = stageElements.Where(e => e.AggregateType == AggregateType.AddRaw).Sum(e => e.Value);
            var multiValues = stageElements.Where(e => e.AggregateType == AggregateType.Multiplicate).Select(e => e.Value).Concat(Enumerable.Repeat(1.0F, 1)).Aggregate((a, b) => a * b);
            var finalValues = stageElements.Where(e => e.AggregateType == AggregateType.AddFinal).Sum(e => e.Value);
            var maxValues = stageElements.Where(e => e.AggregateType == AggregateType.Maximum).MaxBy(e => e.Value)?.Value ?? 0;
            rawValues += maxValues;

            if (multiValues == 0 && stageElements.All(e => e.AggregateType != AggregateType.Multiplicate))
            {
                multiValues = 1;
            }
            else if (newValue + rawValues == 0 && multiValues != 0 && stageElements.All(e => e.AggregateType == AggregateType.Multiplicate))
            {
                rawValues = 1;
            }
            else
            {
                // nothing to do
            }

            newValue = ((newValue + rawValues) * multiValues) + finalValues;
        }

        if (this.Definition.MaximumValue.HasValue)
        {
            newValue = Math.Min(this.Definition.MaximumValue.Value, newValue);
        }

        this._cachedValue = newValue;

        return this._cachedValue.Value;
    }

    private void ElementChanged(object? sender, EventArgs eventArgs)
    {
        this._cachedValue = null;
        this.RaiseValueChanged();
    }
}