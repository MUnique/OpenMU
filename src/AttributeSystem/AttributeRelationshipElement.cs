// <copyright file="AttributeRelationshipElement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An attribute relationship element which takes several input elements which are summed up and multiplied.
    /// Calculated values are cached for a better performance.
    /// </summary>
    public class AttributeRelationshipElement : SimpleElement
    {
        private float? cachedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeRelationshipElement" /> class.
        /// </summary>
        /// <param name="inputElements">The input elements which are summed up.</param>
        /// <param name="inputOperand">The operand which is applied to the summed up input elements.</param>
        /// <param name="inputOperator">The input operator.</param>
        public AttributeRelationshipElement(IEnumerable<IElement> inputElements, float inputOperand, InputOperator inputOperator)
        {
            this.InputElements = inputElements;
            this.InputOperand = inputOperand;
            this.InputOperator = inputOperator;
            foreach (var element in this.InputElements)
            {
                element.ValueChanged += this.ElementChanged;
            }
        }

        /// <summary>
        /// Gets the input elements.
        /// </summary>
        public IEnumerable<IElement> InputElements { get; }

        /// <summary>
        /// Gets or sets the multiplier with which the sum of all input element values are multiplied.
        /// </summary>
        public float InputOperand { get; set; }

        /// <summary>
        /// Gets or sets the input operator.
        /// </summary>
        public InputOperator InputOperator { get; set; }

        /// <summary>
        /// Gets the calculated value.
        /// </summary>
        public override float Value => this.cachedValue ?? this.GetAndCacheValue();

        private void ElementChanged(object sender, EventArgs eventArgs)
        {
            this.cachedValue = null;
            this.RaiseValueChanged();
        }

        private float GetAndCacheValue()
        {
            this.cachedValue = this.CalculateValue();
            return this.cachedValue.Value;
        }

        private float CalculateValue()
        {
            switch (this.InputOperator)
            {
                case InputOperator.Multiply:
                    return this.InputElements.Sum(a => a.Value) * this.InputOperand;
                case InputOperator.Add:
                    return this.InputElements.Sum(a => a.Value) + this.InputOperand;
                case InputOperator.Exponentiate:
                    return (float)System.Math.Pow(this.InputElements.Sum(a => a.Value), this.InputOperand);
                default:
                    throw new System.InvalidOperationException($"Input operator {this.InputOperator} unknown");
            }
        }
    }
}
