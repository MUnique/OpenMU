// <copyright file="ConstantElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;

    /// <summary>
    /// An element with a constant value.
    /// </summary>
    public class ConstantElement : IElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantElement"/> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        public ConstantElement(float value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Never occurs, so the implementation is empty.
        /// </summary>
        public event EventHandler ValueChanged
        {
            add
            {
                // do nothing, as the value never changes.
            }

            remove
            {
                // do nothing, as the value never changes.
            }
        }

        /// <inheritdoc/>
        public float Value { get; }

        /// <inheritdoc/>
        public AggregateType AggregateType { get; } = AggregateType.AddRaw;
    }
}
