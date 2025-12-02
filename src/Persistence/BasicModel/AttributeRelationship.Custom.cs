// <copyright file="AttributeRelationship.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel;

using MUnique.OpenMU.DataModel;

/// <summary>
/// A plain implementation of <see cref="MUnique.OpenMU.AttributeSystem.AttributeRelationship"/>.
/// </summary>
public partial class AttributeRelationship :
    IAssignable,
    IAssignable<MUnique.OpenMU.AttributeSystem.AttributeRelationship>,
    ICloneable<MUnique.OpenMU.AttributeSystem.AttributeRelationship>
{
    /// <inheritdoc />
    public virtual AttributeSystem.AttributeRelationship Clone(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new AttributeRelationship();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(object other, DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        if (other is AttributeSystem.AttributeRelationship typedOther)
        {
            this.AssignValuesOf(typedOther, gameConfiguration);
        }
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(AttributeSystem.AttributeRelationship other, DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        this.InputAttribute = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == other.InputAttribute?.Id) ?? other.InputAttribute;
        this.OperandAttribute = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == other.OperandAttribute?.Id) ?? other.OperandAttribute;
        this.TargetAttribute = gameConfiguration.Attributes.FirstOrDefault(a => a.Id == other.TargetAttribute?.Id) ?? other.TargetAttribute;
        this.InputOperand = other.InputOperand;
        this.InputOperator = other.InputOperator;
        this.AggregateType = other.AggregateType;
    }
}