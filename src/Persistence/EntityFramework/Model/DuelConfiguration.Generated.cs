// <copyright file="DuelConfiguration.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.DuelConfiguration"/>.
/// </summary>
[Table(nameof(DuelConfiguration), Schema = SchemaNames.Configuration)]
internal partial class DuelConfiguration : MUnique.OpenMU.DataModel.Configuration.DuelConfiguration, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="DuelAreas" />.
    /// </summary>
    public ICollection<DuelArea> RawDuelAreas { get; } = new EntityFramework.List<DuelArea>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.DuelArea> DuelAreas => base.DuelAreas ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.DuelArea, DuelArea>(this.RawDuelAreas);

    /// <summary>
    /// Gets or sets the identifier of <see cref="Exit"/>.
    /// </summary>
    public Guid? ExitId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Exit" />.
    /// </summary>
    [ForeignKey(nameof(ExitId))]
    public ExitGate RawExit
    {
        get => base.Exit as ExitGate;
        set => base.Exit = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.ExitGate Exit
    {
        get => base.Exit;set
        {
            base.Exit = value;
            this.ExitId = this.RawExit?.Id;
        }
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.DuelConfiguration Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new DuelConfiguration();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.DuelConfiguration other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
