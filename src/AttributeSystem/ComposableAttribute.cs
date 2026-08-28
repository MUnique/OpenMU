// <copyright file="ComposableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute which is a composition of elements.
/// </summary>
public class ComposableAttribute : BaseAttribute, IComposableAttribute
{
    /// <summary>
    /// Serializes the copy-on-write mutations of <see cref="_elements"/>, the element subscription
    /// changes and the <see cref="_version"/> bump. Elements are usually added on the game logic thread,
    /// but they can also be removed from a thread pool thread when a magic effect expires on its timer,
    /// while a value recalculation is aggregating at the same time. The cached fast path of
    /// <see cref="Value"/> and the <see cref="Elements"/> getter are lock-free (a single volatile read of
    /// <see cref="_elements"/>); on a cache miss, <see cref="GetAndCacheValue"/> takes this lock only
    /// briefly to capture a consistent (snapshot, version) pair and again to write the cache, and does the
    /// aggregation itself outside the lock on the immutable snapshot.
    /// </summary>
    private readonly object _elementLock = new();

    /// <summary>
    /// The elements this attribute is composed of. Treated as immutable: mutations replace the whole
    /// array under <see cref="_elementLock"/> (copy-on-write), so a reader that captured the reference
    /// can keep enumerating it safely while another thread adds or removes an element.
    /// </summary>
    private volatile IElement[] _elements = [];

    /// <summary>
    /// Incremented under <see cref="_elementLock"/> on every structural change or cache invalidation.
    /// <see cref="GetAndCacheValue"/> captures it before aggregating and only writes the result back into
    /// <see cref="_cachedValue"/> when it is still unchanged, so a recalculation that raced a concurrent
    /// removal cannot latch a stale value into the cache.
    /// </summary>
    private long _version;

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
        this._maximumValue = maximumValue;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Returns the current copy-on-write snapshot of the elements. It is detached from later mutations on
    /// purpose, so the caller can enumerate it safely even while elements are concurrently added or
    /// removed; a previously obtained sequence will not reflect subsequent changes.
    /// </remarks>
    public IEnumerable<IElement> Elements => this._elements;

    /// <inheritdoc/>
    public override float Value => this._cachedValue ?? this.GetAndCacheValue();

    /// <inheritdoc/>
    public IComposableAttribute AddElement(IElement element)
    {
        lock (this._elementLock)
        {
            this._elements = [.. this._elements, element];

            // Subscribing inside the lock keeps a concurrent Add/Remove of the same element from
            // interleaving into a live subscription on an element that is no longer in the list. It does
            // not call into user code, so it is safe to hold the lock across it.
            element.ValueChanged += this.ElementChanged;
        }

        this.ElementChanged(element, EventArgs.Empty);

        return this;
    }

    /// <inheritdoc/>
    public void RemoveElement(IElement element)
    {
        bool removed;
        lock (this._elementLock)
        {
            var index = Array.IndexOf(this._elements, element);
            removed = index >= 0;
            if (removed)
            {
                var newElements = new IElement[this._elements.Length - 1];
                Array.Copy(this._elements, 0, newElements, 0, index);
                Array.Copy(this._elements, index + 1, newElements, index, this._elements.Length - index - 1);
                this._elements = newElements;
                element.ValueChanged -= this.ElementChanged;
            }
        }

        if (removed)
        {
            this.ElementChanged(element, EventArgs.Empty);
        }
    }

    private float GetAndCacheValue()
    {
        // Aggregate a copy-on-write snapshot, so a concurrent AddElement/RemoveElement (e.g. an expiring
        // magic effect on a timer thread) cannot tear the list. The version captured alongside the
        // snapshot guards the cache write below: if anything changed the composition or invalidated the
        // cache while this recomputation was running, the freshly computed value is still returned to this
        // caller, but it is not written back - otherwise the lost update would latch a stale value into
        // _cachedValue until some unrelated element change happened to invalidate it again.
        IElement[] elements;
        long versionAtStart;
        lock (this._elementLock)
        {
            elements = this._elements;
            versionAtStart = this._version;
        }

        if (elements.Length == 0)
        {
            lock (this._elementLock)
            {
                if (this._version == versionAtStart)
                {
                    this._cachedValue = 0;
                }
            }

            return 0;
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

        lock (this._elementLock)
        {
            if (this._version == versionAtStart)
            {
                this._cachedValue = newValue;
            }
        }

        return newValue;
    }

    private void ElementChanged(object? sender, EventArgs eventArgs)
    {
        lock (this._elementLock)
        {
            this._version++;
            this._cachedValue = null;
        }

        this.RaiseValueChanged();
    }
}
