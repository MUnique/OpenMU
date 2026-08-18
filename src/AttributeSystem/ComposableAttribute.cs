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

    /// <summary>
    /// Synchronizes access to <see cref="_elementList"/>. Elements are usually added on the game logic
    /// thread, but they can also be removed from a thread pool thread when a magic effect expires on its
    /// timer, while a value recalculation may be enumerating the list at the same time. Without this lock
    /// the concurrent <see cref="List{T}.Remove"/> can transiently expose a <c>null</c> slot to the
    /// enumeration in <see cref="GetAndCacheValue"/>, causing a <see cref="NullReferenceException"/>.
    /// </summary>
    private readonly object _elementLock = new();

    private float? _maximumValue;

    private float? _cachedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposableAttribute" /> class.
    /// </summary>
    /// <param name="definition">The definition.</param>
    /// <param name="aggregateType">Type of the aggregate.</param>
    /// <param name="maximumValue">The inner maximum value.</param>
    public ComposableAttribute(AttributeDefinition definition, AggregateType aggregateType = AggregateType.AddRaw, float? maximumValue = null)
        : base(definition, aggregateType)
    {
        this._elementList = new List<IElement>();
        this._maximumValue = maximumValue;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns a snapshot of the current elements, detached from the internal list on purpose, so the
    /// caller can enumerate it safely even while elements are concurrently added or removed.
    /// </remarks>
    public IEnumerable<IElement> Elements
    {
        get
        {
            lock (this._elementLock)
            {
                return this._elementList.ToArray();
            }
        }
    }

    /// <inheritdoc/>
    public override float Value => this._cachedValue ?? this.GetAndCacheValue();

    /// <inheritdoc/>
    public IComposableAttribute AddElement(IElement element)
    {
        lock (this._elementLock)
        {
            this._elementList.Add(element);
        }

        element.ValueChanged += this.ElementChanged;
        this.ElementChanged(element, EventArgs.Empty);

        return this;
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element)
    {
        bool removed;
        lock (this._elementLock)
        {
            removed = this._elementList.Remove(element);
        }

        if (removed)
        {
            element.ValueChanged -= this.ElementChanged;
            this.ElementChanged(element, EventArgs.Empty);
        }
    }

    private float GetAndCacheValue()
    {
        // Enumerate a snapshot taken under the lock, so a concurrent AddElement/RemoveElement (e.g. an
        // expiring magic effect on a timer thread) cannot expose a torn list - the transiently null slot
        // that would otherwise be dereferenced here and throw a NullReferenceException. The aggregation
        // itself runs outside the lock on purpose: reading an element's value recurses into other
        // attributes, and holding the lock across that traversal would invite lock-ordering issues and
        // lengthen contention. The value cache stays best-effort - an occasional stale or torn read
        // corrects itself on the next recalculation - which matches the pre-existing behavior.
        IElement[] elements;
        lock (this._elementLock)
        {
            if (this._elementList.Count == 0)
            {
                this._cachedValue = 0;
                return 0;
            }

            elements = this._elementList.ToArray();
        }

        var rawValues = elements.Where(e => e.AggregateType == AggregateType.AddRaw).Sum(e => e.Value);
        var multiValues = elements.Where(e => e.AggregateType == AggregateType.Multiplicate).Select(e => e.Value).Concat(Enumerable.Repeat(1.0F, 1)).Aggregate((a, b) => a * b);
        var finalValues = elements.Where(e => e.AggregateType == AggregateType.AddFinal).Sum(e => e.Value);
        var maxValues = elements.Where(e => e.AggregateType == AggregateType.Maximum).MaxBy(e => e.Value)?.Value ?? 0;
        rawValues += maxValues;

        if (elements.All(e => e.AggregateType == AggregateType.Multiplicate))
        {
            rawValues = 1;
        }

        var newValue = (rawValues * multiValues) + finalValues;
        if (this._maximumValue.HasValue)
        {
            newValue = Math.Min(this._maximumValue.Value, newValue);
        }

        if (this.Definition.MaximumValue.HasValue)
        {
            newValue = Math.Min(this.Definition.MaximumValue.Value, newValue);
        }

        this._cachedValue = newValue;

        return newValue;
    }

    private void ElementChanged(object? sender, EventArgs eventArgs)
    {
        this._cachedValue = null;
        this.RaiseValueChanged();
    }
}