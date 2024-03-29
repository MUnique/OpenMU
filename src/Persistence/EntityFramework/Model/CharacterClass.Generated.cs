// <copyright file="CharacterClass.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.CharacterClass"/>.
/// </summary>
[Table(nameof(CharacterClass), Schema = SchemaNames.Configuration)]
internal partial class CharacterClass : MUnique.OpenMU.DataModel.Configuration.CharacterClass, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="StatAttributes" />.
    /// </summary>
    public ICollection<StatAttributeDefinition> RawStatAttributes { get; } = new EntityFramework.List<StatAttributeDefinition>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.StatAttributeDefinition> StatAttributes => base.StatAttributes ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.StatAttributeDefinition, StatAttributeDefinition>(this.RawStatAttributes);

    /// <summary>
    /// Gets the raw collection of <see cref="AttributeCombinations" />.
    /// </summary>
    public ICollection<AttributeRelationship> RawAttributeCombinations { get; } = new EntityFramework.List<AttributeRelationship>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.AttributeSystem.AttributeRelationship> AttributeCombinations => base.AttributeCombinations ??= new CollectionAdapter<MUnique.OpenMU.AttributeSystem.AttributeRelationship, AttributeRelationship>(this.RawAttributeCombinations);

    /// <summary>
    /// Gets the raw collection of <see cref="BaseAttributeValues" />.
    /// </summary>
    public ICollection<ConstValueAttribute> RawBaseAttributeValues { get; } = new EntityFramework.List<ConstValueAttribute>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.AttributeSystem.ConstValueAttribute> BaseAttributeValues => base.BaseAttributeValues ??= new CollectionAdapter<MUnique.OpenMU.AttributeSystem.ConstValueAttribute, ConstValueAttribute>(this.RawBaseAttributeValues);

    /// <summary>
    /// Gets or sets the identifier of <see cref="NextGenerationClass"/>.
    /// </summary>
    public Guid? NextGenerationClassId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="NextGenerationClass" />.
    /// </summary>
    [ForeignKey(nameof(NextGenerationClassId))]
    public CharacterClass RawNextGenerationClass
    {
        get => base.NextGenerationClass as CharacterClass;
        set => base.NextGenerationClass = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.CharacterClass NextGenerationClass
    {
        get => base.NextGenerationClass;set
        {
            base.NextGenerationClass = value;
            this.NextGenerationClassId = this.RawNextGenerationClass?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="HomeMap"/>.
    /// </summary>
    public Guid? HomeMapId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="HomeMap" />.
    /// </summary>
    [ForeignKey(nameof(HomeMapId))]
    public GameMapDefinition RawHomeMap
    {
        get => base.HomeMap as GameMapDefinition;
        set => base.HomeMap = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.GameMapDefinition HomeMap
    {
        get => base.HomeMap;set
        {
            base.HomeMap = value;
            this.HomeMapId = this.RawHomeMap?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="ComboDefinition"/>.
    /// </summary>
    public Guid? ComboDefinitionId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="ComboDefinition" />.
    /// </summary>
    [ForeignKey(nameof(ComboDefinitionId))]
    public SkillComboDefinition RawComboDefinition
    {
        get => base.ComboDefinition as SkillComboDefinition;
        set => base.ComboDefinition = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.SkillComboDefinition ComboDefinition
    {
        get => base.ComboDefinition;set
        {
            base.ComboDefinition = value;
            this.ComboDefinitionId = this.RawComboDefinition?.Id;
        }
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.CharacterClass Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new CharacterClass();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.CharacterClass other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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

    
}
