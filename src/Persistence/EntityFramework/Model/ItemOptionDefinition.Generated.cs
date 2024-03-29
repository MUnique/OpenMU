// <copyright file="ItemOptionDefinition.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition"/>.
/// </summary>
[Table(nameof(ItemOptionDefinition), Schema = SchemaNames.Configuration)]
internal partial class ItemOptionDefinition : MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="PossibleOptions" />.
    /// </summary>
    public ICollection<IncreasableItemOption> RawPossibleOptions { get; } = new EntityFramework.List<IncreasableItemOption>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.IncreasableItemOption> PossibleOptions => base.PossibleOptions ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.IncreasableItemOption, IncreasableItemOption>(this.RawPossibleOptions);

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new ItemOptionDefinition();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
