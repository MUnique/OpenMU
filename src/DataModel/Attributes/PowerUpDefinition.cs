// <copyright file="PowerUpDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Attributes
{
    using System.Globalization;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;

    /// <summary>
    /// The power up definition which describes the boost of an target attribute.
    /// </summary>
    public class PowerUpDefinition
    {
        /// <summary>
        /// Gets or sets the target attribute.
        /// </summary>
        public virtual AttributeDefinition TargetAttribute { get; set; }

        /// <summary>
        /// Gets or sets the boost.
        /// </summary>
        public virtual PowerUpDefinitionValue Boost { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            string value = string.Empty;
            if (this.Boost != null && this.Boost.ConstantValue.Value > 0)
            {
                value = this.Boost.ConstantValue.Value.ToString(CultureInfo.InvariantCulture);
            }
            else if (this.Boost?.RelatedValues != null && this.Boost.RelatedValues.Any())
            {
                var relation = this.Boost.RelatedValues.First();
                value = relation.InputAttribute.Designation + OperatorAsString(relation.InputOperator) + relation.InputOperand;
            }

            return value + " " + this.TargetAttribute.Designation;
        }

        private static string OperatorAsString(InputOperator inputOperator)
        {
            switch (inputOperator)
            {
                case InputOperator.Add: return "+";
                case InputOperator.Multiply: return "*";
                case InputOperator.Exponentiate: return "^";
            }

            return string.Empty;
        }
    }
}
