// <copyright file="BaseStatAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    /// <summary>
    /// An attribute which represents an increasable stat attribute (e.g. by level-up points).
    /// </summary>
    /// <remarks>
    /// Intermediate class, needed because we want to add a setter.
    /// We do just override the getter here and have to introduce a new Value get/set-property on a derived type.
    /// </remarks>
    public abstract class BaseStatAttribute : BaseAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseStatAttribute"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        protected BaseStatAttribute(AttributeDefinition definition, AggregateType aggregateType)
            : base(definition, aggregateType)
        {
        }

        /// <inheritdoc/>
        public override float Value => this.ValueGetter;

        /// <summary>
        /// Gets the value.
        /// </summary>
        protected abstract float ValueGetter { get; }
    }
}
