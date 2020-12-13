// <copyright file="ComposableAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An attribute which is a composition of elements.
    /// </summary>
    public class ComposableAttribute : BaseAttribute, IComposableAttribute
    {
        private readonly IList<IElement> elementList;

        private float? cachedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComposableAttribute"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        public ComposableAttribute(AttributeDefinition definition)
            : base(definition, AggregateType.AddRaw)
        {
            this.elementList = new List<IElement>();
        }

        /// <inheritdoc/>
        public IEnumerable<IElement> Elements => this.elementList;

        /// <inheritdoc/>
        public override float Value => this.cachedValue ?? this.GetAndCacheValue();

        /// <inheritdoc/>
        public IComposableAttribute AddElement(IElement element)
        {
            this.elementList.Add(element);
            element.ValueChanged += this.ElementChanged;
            this.ElementChanged(element, EventArgs.Empty);

            return this;
        }

        /// <inheritdoc/>
        public void RemoveElement(IElement element)
        {
            if (this.elementList.Remove(element))
            {
                element.ValueChanged -= this.ElementChanged;
                this.ElementChanged(element, EventArgs.Empty);
            }
        }

        private float GetAndCacheValue()
        {
            this.cachedValue = (this.Elements.Where(e => e.AggregateType == AggregateType.AddRaw).Sum(e => e.Value)
                * this.Elements.Where(e => e.AggregateType == AggregateType.Multiplicate).Select(e => e.Value).Concat(Enumerable.Repeat(1.0F, 1)).Aggregate((a, b) => a * b))
                + this.Elements.Where(e => e.AggregateType == AggregateType.AddFinal).Sum(e => e.Value);

            return this.cachedValue.Value;
        }

        private void ElementChanged(object? sender, EventArgs eventArgs)
        {
            this.cachedValue = null;
            this.RaiseValueChanged();
        }
    }
}
