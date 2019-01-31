// <copyright file="CombinedElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;

    /// <summary>
    /// An element which combines two elements into one.
    /// </summary>
    public class CombinedElement : IElement
    {
        private readonly IElement element1;
        private readonly IElement element2;

        /// <summary>
        /// Initializes a new instance of the <see cref="CombinedElement"/> class.
        /// </summary>
        /// <param name="element1">The first element.</param>
        /// <param name="element2">The second element.</param>
        /// <exception cref="ArgumentException">The aggregate type of both elements need to match.</exception>
        public CombinedElement(IElement element1, IElement element2)
        {
            if (element1.AggregateType != element2.AggregateType)
            {
                throw new ArgumentException($"The aggregate type of both elements need to match. Element1: {element1.AggregateType}, Element2: {element2.AggregateType}");
            }

            this.element1 = element1;
            this.element2 = element2;
            this.element1.ValueChanged += (_, e) => this.ValueChanged?.Invoke(this, EventArgs.Empty);
            this.element2.ValueChanged += (_, e) => this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public event EventHandler ValueChanged;

        /// <inheritdoc />
        public float Value => this.element1.Value + this.element2.Value;

        /// <inheritdoc />
        public AggregateType AggregateType => this.element1.AggregateType;
    }
}