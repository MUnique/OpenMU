// <copyright file="Item.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Entities.Item"/>.
/// </summary>
[Table(nameof(Item), Schema = SchemaNames.AccountData)]
internal partial class Item : MUnique.OpenMU.DataModel.Entities.Item, IIdentifiable
{
    /// <inheritdoc />
    public Item()
    {
        this.InitJoinCollections();
    }

    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="ItemOptions" />.
    /// </summary>
    public ICollection<ItemOptionLink> RawItemOptions { get; } = new EntityFramework.List<ItemOptionLink>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Entities.ItemOptionLink> ItemOptions => base.ItemOptions ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Entities.ItemOptionLink, ItemOptionLink>(this.RawItemOptions);

    /// <summary>
    /// Gets or sets the identifier of <see cref="Definition"/>.
    /// </summary>
    public Guid? DefinitionId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Definition" />.
    /// </summary>
    [ForeignKey(nameof(DefinitionId))]
    public ItemDefinition RawDefinition
    {
        get => base.Definition as ItemDefinition;
        set => base.Definition = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition Definition
    {
        get => base.Definition;set
        {
            base.Definition = value;
            this.DefinitionId = this.RawDefinition?.Id;
        }
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Entities.Item Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new Item();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Entities.Item other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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

    protected void InitJoinCollections()
    {
        this.ItemSetGroups = new ManyToManyCollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.ItemOfItemSet, ItemItemOfItemSet>(this.JoinedItemSetGroups, joinEntity => joinEntity.ItemOfItemSet, entity => new ItemItemOfItemSet { Item = this, ItemId = this.Id, ItemOfItemSet = (ItemOfItemSet)entity, ItemOfItemSetId = ((ItemOfItemSet)entity).Id});
    }
}
