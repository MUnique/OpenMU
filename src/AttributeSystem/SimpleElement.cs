// <copyright file="SimpleElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;

    /// <summary>
    /// A simple element with a variable value.
    /// </summary>
    public class SimpleElement : IElement
    {
        private float value;
        private AggregateType aggregateType;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElement"/> class.
        /// </summary>
        public SimpleElement()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElement"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public SimpleElement(float value, AggregateType aggregateType)
        {
            this.value = value;
            this.aggregateType = aggregateType;
        }

        /// <inheritdoc/>
        public event EventHandler ValueChanged;

        /// <inheritdoc/>
        public virtual float Value
        {
            get => this.value;

            set
            {
                if (Math.Abs(this.value - value) > 0.01f)
                {
                    this.value = value;
                    this.RaiseValueChanged();
                }
            }
        }

        /// <inheritdoc/>
        public AggregateType AggregateType
        {
            get => this.aggregateType;

            set
            {
                if (this.aggregateType != value)
                {
                    this.aggregateType = value;
                    this.RaiseValueChanged();
                }
            }
        }

        /// <summary>
        /// Raises the value changed event.
        /// </summary>
        protected void RaiseValueChanged()
        {
            this.ValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
