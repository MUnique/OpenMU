// <copyright file="PowerUpDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Attributes;

using System.Globalization;
using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The power up definition which describes the boost of an target attribute.
/// </summary>
[Cloneable]
public partial class PowerUpDefinition
{
    /// <summary>
    /// Gets or sets the target attribute.
    /// </summary>
    public virtual AttributeDefinition? TargetAttribute { get; set; }

    /// <summary>
    /// Gets or sets the boost.
    /// </summary>
    [MemberOfAggregate]
    public virtual PowerUpDefinitionValue? Boost { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        string value;
        if (this.Boost?.ConstantValue?.Value > 0)
        {
            value = this.Boost.ConstantValue.Value.ToString(CultureInfo.InvariantCulture);
        }
        else if (this.Boost?.RelatedValues != null && this.Boost.RelatedValues.Any())
        {
            var relation = this.Boost.RelatedValues.First();
            if (relation.InputOperator == InputOperator.ExponentiateByAttribute)
            {
                value = relation.InputOperand + relation.InputOperator.AsString() + relation.InputAttribute?.Designation;
            }
            else
            {
                value = relation.InputAttribute?.Designation + relation.InputOperator.AsString() + relation.InputOperand;
            }
        }
        else
        {
            // no value defined, so we assume "0"
            value = "0";
        }

        return value + " " + this.TargetAttribute?.Designation;
    }
}