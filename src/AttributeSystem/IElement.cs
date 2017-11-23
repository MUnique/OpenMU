// <copyright file="IElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;

    /// <summary>
    /// The interface of an element.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Occurs when the value has been changed.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Gets the value.
        /// </summary>
        float Value { get; }

        /// <summary>
        /// Gets the type of the aggregate.
        /// </summary>
        AggregateType AggregateType { get; }
    }
}
