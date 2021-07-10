// <copyright file="AttributeRelationship.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem
{
    /// <summary>
    /// The operator which is applied between the input attribute and the input operand.
    /// </summary>
    public enum InputOperator
    {
        /// <summary>
        /// The <see cref="AttributeRelationship.InputAttribute"/> is multiplied with the <see cref="AttributeRelationship.InputOperand"/> before adding to the <see cref="AttributeRelationship.TargetAttribute"/>.
        /// </summary>
        Multiply,

        /// <summary>
        /// The <see cref="AttributeRelationship.InputAttribute"/> is increased by the <see cref="AttributeRelationship.InputOperand"/> before adding to the <see cref="AttributeRelationship.TargetAttribute"/>.
        /// </summary>
        Add,

        /// <summary>
        /// The <see cref="AttributeRelationship.InputAttribute"/> is exponentiated by the <see cref="AttributeRelationship.InputOperand"/> before adding to the <see cref="AttributeRelationship.TargetAttribute"/>.
        /// </summary>
        Exponentiate,
    }

    /// <summary>
    /// Describes a relationship between two attributes.
    /// </summary>
    public class AttributeRelationship
    {
        private AttributeDefinition? targetAttribute;
        private AttributeDefinition? inputAttribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeRelationship"/> class.
        /// </summary>
        public AttributeRelationship()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeRelationship" /> class.
        /// </summary>
        /// <param name="targetAttribute">The target attribute.</param>
        /// <param name="inputOperand">The multiplier.</param>
        /// <param name="inputAttribute">The input attribute.</param>
        public AttributeRelationship(AttributeDefinition targetAttribute, float inputOperand, AttributeDefinition inputAttribute)
            : this(targetAttribute, inputOperand, inputAttribute, InputOperator.Multiply)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeRelationship" /> class.
        /// </summary>
        /// <param name="targetAttribute">The target attribute.</param>
        /// <param name="inputOperand">The multiplier.</param>
        /// <param name="inputAttribute">The input attribute.</param>
        /// <param name="inputOperator">The input operator.</param>
        public AttributeRelationship(AttributeDefinition targetAttribute, float inputOperand, AttributeDefinition inputAttribute, InputOperator inputOperator)
        {
            this.InputOperand = inputOperand;
            this.InputOperator = inputOperator;
            this.targetAttribute = targetAttribute;
            this.inputAttribute = inputAttribute;
        }

        /// <summary>
        /// Gets or sets the target attribute which will be affected.
        /// </summary>
        public virtual AttributeDefinition? TargetAttribute
        {
            get => this.targetAttribute;
            set => this.targetAttribute = value;
        }

        /// <summary>
        /// Gets or sets the input attribute which provides the input value.
        /// </summary>
        public virtual AttributeDefinition? InputAttribute
        {
            get => this.inputAttribute;
            set => this.inputAttribute = value;
        }

        /// <summary>
        /// Gets or sets the operator which is applied between the input attribute and the input operand.
        /// </summary>
        public InputOperator InputOperator { get; set; }

        /// <summary>
        /// Gets or sets the operand which is applied to the input attribute before adding to the target attribute.
        /// </summary>
        public float InputOperand { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.TargetAttribute is null)
            {
                return $"{this.InputAttribute} {this.InputOperator.AsString()} {this.InputOperand}";
            }

            return $"{this.TargetAttribute} += {this.InputAttribute} {this.InputOperator.AsString()} {this.InputOperand}";
        }
    }
}
