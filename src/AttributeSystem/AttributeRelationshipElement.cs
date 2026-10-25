// <copyright file="AttributeRelationshipElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// An attribute relationship element which takes several input elements which are summed up and multiplied.
/// Calculated values are cached for a better performance.
/// </summary>
/// <remarks>
/// The element subscribes to its inputs, which may live longer than the element itself, e.g. the stat
/// attributes of a character which outlive the attribute system of one game session. Dispose the element
/// when it's no longer used, so that its inputs don't keep it - and whatever observes it - alive.
/// </remarks>
public class AttributeRelationshipElement : SimpleElement, IDisposable
{
    /// <summary>
    /// Serializes the <see cref="_version"/> bump with the guarded cache write, so a recalculation that
    /// races a concurrent input change (e.g. an input attribute whose element is removed when a magic
    /// effect expires) cannot latch its stale result into <see cref="_cachedValue"/>.
    /// </summary>
    private readonly object _cacheLock = new();

    private long _version;

    private float? _cachedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttributeRelationshipElement" /> class.
    /// </summary>
    /// <param name="inputElements">The input elements which are summed up.</param>
    /// <param name="inputOperand">The operand which is applied to the summed up input elements.</param>
    /// <param name="inputOperator">The input operator.</param>
    public AttributeRelationshipElement(IEnumerable<IElement> inputElements, IElement inputOperand, InputOperator inputOperator)
    {
        // A copy, so that Dispose unsubscribes from exactly the elements subscribed here, even if the
        // passed sequence is evaluated lazily.
        this.InputElements = inputElements.ToArray();
        this.InputOperand = inputOperand;
        this.InputOperator = inputOperator;
        foreach (var element in this.InputElements)
        {
            element.ValueChanged += this.ElementChanged;
        }

        inputOperand.ValueChanged += this.ElementChanged;
    }

    /// <summary>
    /// Gets the input elements.
    /// </summary>
    public IEnumerable<IElement> InputElements { get; }

    /// <summary>
    /// Gets or sets the multiplier with which the sum of all input element values are multiplied.
    /// </summary>
    public IElement InputOperand { get; set; }

    /// <summary>
    /// Gets or sets the input operator.
    /// </summary>
    public InputOperator InputOperator { get; set; }

    /// <summary>
    /// Gets the calculated value.
    /// </summary>
    public override float Value => this._cachedValue ?? this.GetAndCacheValue();

    /// <summary>
    /// Unsubscribes from the input elements and the input operand.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Unsubscribes from the input elements and the input operand, see <see cref="Dispose()"/>.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        foreach (var element in this.InputElements)
        {
            element.ValueChanged -= this.ElementChanged;
        }

        this.InputOperand.ValueChanged -= this.ElementChanged;
    }

    private void ElementChanged(object? sender, EventArgs eventArgs)
    {
        lock (this._cacheLock)
        {
            this._version++;
            this._cachedValue = null;
        }

        this.RaiseValueChanged();
    }

    private float GetAndCacheValue()
    {
        // Return the freshly computed value rather than re-reading the field: a concurrent ElementChanged
        // (e.g. from an input attribute whose element is removed when a magic effect expires) can reset
        // _cachedValue to null between the assignment and the read, which would make _cachedValue.Value
        // throw an InvalidOperationException. The version captured before calculating guards the cache
        // write: if an input changed while CalculateValue was running, the fresh value is still returned
        // to this caller but not cached, so the lost update cannot latch a stale value.
        long versionAtStart;
        lock (this._cacheLock)
        {
            versionAtStart = this._version;
        }

        var value = this.CalculateValue();

        lock (this._cacheLock)
        {
            if (this._version == versionAtStart)
            {
                this._cachedValue = value;
            }
        }

        return value;
    }

    private float CalculateValue()
    {
        return this.InputOperator switch
        {
            InputOperator.Multiply => this.InputElements.Sum(a => a.Value) * this.InputOperand.Value,
            InputOperator.Add => this.InputElements.Sum(a => a.Value) + this.InputOperand.Value,
            InputOperator.Exponentiate => (float)Math.Pow(
                this.InputElements.Sum(a => a.Value),
                this.InputOperand.Value),
            InputOperator.ExponentiateByAttribute => (float)Math.Pow(
                this.InputOperand.Value,
                this.InputElements.Sum(a => a.Value)),
            InputOperator.Maximum => Math.Max(
                this.InputElements.Sum(a => a.Value),
                this.InputOperand.Value),
            InputOperator.Minimum => Math.Min(
                this.InputElements.Sum(a => a.Value),
                this.InputOperand.Value),
            _ => throw new InvalidOperationException($"Input operator {this.InputOperator} unknown"),
        };
    }
}