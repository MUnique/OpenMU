// <copyright file="IElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The interface of an element.
/// </summary>
public interface IElement
{
    /// <summary>
    /// Occurs when the value has been changed.
    /// </summary>
    event EventHandler? ValueChanged;

    /// <summary>
    /// Gets the value.
    /// </summary>
    float Value { get; }

    /// <summary>
    /// Gets the type of the aggregate.
    /// </summary>
    AggregateType AggregateType { get; }

    /// <summary>
    /// Gets the stage at which the element value is calculated when part of a <see cref="ComposableAttribute"/>, if any.
    /// </summary>
    /// <remarks>Useful for attributes which have several rounds of addition and multiplication elements, like the min/max base type damages.
    /// For example, <c>(a * b + c) * d + e</c> would be a two-stage calculation, with <c>a</c>, <c>b</c>, <c>c</c> comprising the first stage, and <c>d</c>, <c>e</c> the second.
    /// In such a case <c>a</c> represents an <see cref="AggregateType.AddRaw"/> value; <c>b</c> and <c>d</c> an <see cref="AggregateType.Multiplicate"/> values; <c>c</c> and <c>e</c> an <see cref="AggregateType.AddFinal"/> values.</remarks>
    byte Stage { get; }
}