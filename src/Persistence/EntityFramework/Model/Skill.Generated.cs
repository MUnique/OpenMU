// <copyright file="Skill.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Skill"/>.
/// </summary>
[Table(nameof(Skill), Schema = SchemaNames.Configuration)]
internal partial class Skill : MUnique.OpenMU.DataModel.Configuration.Skill, IIdentifiable
{
    /// <inheritdoc />
    public Skill()
    {
        this.InitJoinCollections();
    }

    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="Requirements" />.
    /// </summary>
    public ICollection<AttributeRequirement> RawRequirements { get; } = new EntityFramework.List<AttributeRequirement>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement> Requirements => base.Requirements ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement, AttributeRequirement>(this.RawRequirements);

    /// <summary>
    /// Gets the raw collection of <see cref="ConsumeRequirements" />.
    /// </summary>
    public ICollection<AttributeRequirement> RawConsumeRequirements { get; } = new EntityFramework.List<AttributeRequirement>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement> ConsumeRequirements => base.ConsumeRequirements ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement, AttributeRequirement>(this.RawConsumeRequirements);

    /// <summary>
    /// Gets or sets the identifier of <see cref="ElementalModifierTarget"/>.
    /// </summary>
    public Guid? ElementalModifierTargetId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="ElementalModifierTarget" />.
    /// </summary>
    [ForeignKey(nameof(ElementalModifierTargetId))]
    public AttributeDefinition RawElementalModifierTarget
    {
        get => base.ElementalModifierTarget as AttributeDefinition;
        set => base.ElementalModifierTarget = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.AttributeSystem.AttributeDefinition ElementalModifierTarget
    {
        get => base.ElementalModifierTarget;set
        {
            base.ElementalModifierTarget = value;
            this.ElementalModifierTargetId = this.RawElementalModifierTarget?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="MagicEffectDef"/>.
    /// </summary>
    public Guid? MagicEffectDefId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="MagicEffectDef" />.
    /// </summary>
    [ForeignKey(nameof(MagicEffectDefId))]
    public MagicEffectDefinition RawMagicEffectDef
    {
        get => base.MagicEffectDef as MagicEffectDefinition;
        set => base.MagicEffectDef = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition MagicEffectDef
    {
        get => base.MagicEffectDef;set
        {
            base.MagicEffectDef = value;
            this.MagicEffectDefId = this.RawMagicEffectDef?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="MasterDefinition"/>.
    /// </summary>
    public Guid? MasterDefinitionId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="MasterDefinition" />.
    /// </summary>
    [ForeignKey(nameof(MasterDefinitionId))]
    public MasterSkillDefinition RawMasterDefinition
    {
        get => base.MasterDefinition as MasterSkillDefinition;
        set => base.MasterDefinition = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.MasterSkillDefinition MasterDefinition
    {
        get => base.MasterDefinition;set
        {
            base.MasterDefinition = value;
            this.MasterDefinitionId = this.RawMasterDefinition?.Id;
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

    protected void InitJoinCollections()
    {
        this.QualifiedCharacters = new ManyToManyCollectionAdapter<MUnique.OpenMU.DataModel.Configuration.CharacterClass, SkillCharacterClass>(this.JoinedQualifiedCharacters, joinEntity => joinEntity.CharacterClass, entity => new SkillCharacterClass { Skill = this, SkillId = this.Id, CharacterClass = (CharacterClass)entity, CharacterClassId = ((CharacterClass)entity).Id});
    }
}
