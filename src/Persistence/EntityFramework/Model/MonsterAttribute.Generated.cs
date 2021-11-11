// <copyright file="MonsterAttribute.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.MonsterAttribute"/>.
/// </summary>
[Table(nameof(MonsterAttribute), Schema = SchemaNames.Configuration)]
internal partial class MonsterAttribute : MUnique.OpenMU.DataModel.Configuration.MonsterAttribute, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of <see cref="AttributeDefinition"/>.
    /// </summary>
    public Guid? AttributeDefinitionId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="AttributeDefinition" />.
    /// </summary>
    [ForeignKey(nameof(AttributeDefinitionId))]
    public AttributeDefinition RawAttributeDefinition
    {
        get => base.AttributeDefinition as AttributeDefinition;
        set => base.AttributeDefinition = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.AttributeSystem.AttributeDefinition AttributeDefinition
    {
        get => base.AttributeDefinition;set
        {
            base.AttributeDefinition = value;
            this.AttributeDefinitionId = this.RawAttributeDefinition?.Id;
        }
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

    
}
