// <copyright file="ItemSlotType.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.BasicModel;

using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// A plain implementation of <see cref="ItemSlotType"/>.
/// </summary>
public partial class ItemSlotType : MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType, IIdentifiable, IConvertibleTo<ItemSlotType>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new ItemSlotType();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        base.AssignValuesOf(other, gameConfiguration);
        this.Id = other.GetId();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        var baseObject = obj as IIdentifiable;
        if (baseObject != null)
        {
            return baseObject.Id == this.Id;
        }

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    /// <inheritdoc/>
    public ItemSlotType Convert() => this;
}
