// <copyright file="AttributeDefinition.Custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel;

using MUnique.OpenMU.DataModel;

/// <summary>
/// A plain implementation of <see cref="MUnique.OpenMU.AttributeSystem.AttributeDefinition"/>.
/// </summary>
public partial class AttributeDefinition :
    IAssignable,
    IAssignable<MUnique.OpenMU.AttributeSystem.AttributeDefinition>,
    ICloneable<MUnique.OpenMU.AttributeSystem.AttributeDefinition>
{
    /// <inheritdoc />
    public virtual AttributeSystem.AttributeDefinition Clone(DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new AttributeDefinition();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(object other, DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        if (other is AttributeSystem.AttributeDefinition typedOther)
        {
            AssignValuesOf(typedOther, gameConfiguration);
        }
    }

    /// <inheritdoc />
    public virtual void AssignValuesOf(AttributeSystem.AttributeDefinition other, DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        this.Id = other.Id;
        this.Designation = other.Designation;
        this.Description = other.Description;
        this.MaximumValue = other.MaximumValue;
    }
}