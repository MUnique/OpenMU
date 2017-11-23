// <copyright file="ConstValueAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;

    /// <summary>
    /// An attribute with a constant value.
    /// </summary>
    public class ConstValueAttribute : IAttribute
    {
        private AttributeDefinition definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstValueAttribute" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="definition">The definition.</param>
        public ConstValueAttribute(float value, AttributeDefinition definition)
        {
            this.Value = value;
            this.definition = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstValueAttribute"/> class.
        /// </summary>
        protected ConstValueAttribute()
        {
        }

        /// <inheritdoc/>
        /// <remarks>Empty implementation, because the value can't change.</remarks>
        public event EventHandler ValueChanged
        {
            #pragma warning disable S3237 //Empty implementation, because the value can't change.
            add
            {
                // no action required
            }

            remove
            {
                // no action required
            }
            #pragma warning restore S3237
        }

        /// <inheritdoc/>
        public virtual AttributeDefinition Definition
        {
            get => this.definition;
            protected set => this.definition = value;
        }

        /// <inheritdoc/>
        public float Value { get; protected set; }

        /// <inheritdoc/>
        public AggregateType AggregateType => AggregateType.AddRaw;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Definition.Designation}: {this.Value}";
        }
    }
}
