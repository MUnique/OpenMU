// <copyright file="SkillComboDefinition.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.SkillComboDefinition"/>.
/// </summary>
[Table(nameof(SkillComboDefinition), Schema = SchemaNames.Configuration)]
internal partial class SkillComboDefinition : MUnique.OpenMU.DataModel.Configuration.SkillComboDefinition, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="Steps" />.
    /// </summary>
    public ICollection<SkillComboStep> RawSteps { get; } = new EntityFramework.List<SkillComboStep>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.SkillComboStep> Steps => base.Steps ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.SkillComboStep, SkillComboStep>(this.RawSteps);


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
