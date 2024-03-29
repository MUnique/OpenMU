// <copyright file="MagicEffectDefinition.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition"/>.
/// </summary>
[Table(nameof(MagicEffectDefinition), Schema = SchemaNames.Configuration)]
internal partial class MagicEffectDefinition : MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="PowerUpDefinitions" />.
    /// </summary>
    public ICollection<PowerUpDefinition> RawPowerUpDefinitions { get; } = new EntityFramework.List<PowerUpDefinition>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition> PowerUpDefinitions => base.PowerUpDefinitions ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition, PowerUpDefinition>(this.RawPowerUpDefinitions);

    /// <summary>
    /// Gets or sets the identifier of <see cref="Duration"/>.
    /// </summary>
    public Guid? DurationId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Duration" />.
    /// </summary>
    [ForeignKey(nameof(DurationId))]
    public PowerUpDefinitionValue RawDuration
    {
        get => base.Duration as PowerUpDefinitionValue;
        set => base.Duration = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Attributes.PowerUpDefinitionValue Duration
    {
        get => base.Duration;set
        {
            base.Duration = value;
            this.DurationId = this.RawDuration?.Id;
        }
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new MagicEffectDefinition();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.MagicEffectDefinition other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
