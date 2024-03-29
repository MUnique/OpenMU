// <copyright file="ItemLevelBonusTable.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable"/>.
/// </summary>
[Table(nameof(ItemLevelBonusTable), Schema = SchemaNames.Configuration)]
internal partial class ItemLevelBonusTable : MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="BonusPerLevel" />.
    /// </summary>
    public ICollection<LevelBonus> RawBonusPerLevel { get; } = new EntityFramework.List<LevelBonus>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus> BonusPerLevel => base.BonusPerLevel ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus, LevelBonus>(this.RawBonusPerLevel);

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new ItemLevelBonusTable();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.ItemLevelBonusTable other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
