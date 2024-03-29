// <copyright file="AttributeRequirement.Generated.cs" company="MUnique">
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
/// A plain implementation of <see cref="AttributeRequirement"/>.
/// </summary>
public partial class AttributeRequirement : MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement, IIdentifiable, IConvertibleTo<AttributeRequirement>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw object of <see cref="Attribute" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("attribute")]
    public AttributeDefinition RawAttribute
    {
        get => base.Attribute as AttributeDefinition;
        set => base.Attribute = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.AttributeSystem.AttributeDefinition Attribute
    {
        get => base.Attribute;
        set => base.Attribute = value;
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new AttributeRequirement();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
    public AttributeRequirement Convert() => this;
}
