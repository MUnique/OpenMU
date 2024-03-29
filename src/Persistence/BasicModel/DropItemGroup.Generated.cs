// <copyright file="DropItemGroup.Generated.cs" company="MUnique">
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
/// A plain implementation of <see cref="DropItemGroup"/>.
/// </summary>
public partial class DropItemGroup : MUnique.OpenMU.DataModel.Configuration.DropItemGroup, IIdentifiable, IConvertibleTo<DropItemGroup>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="PossibleItems" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("possibleItems")]
    public ICollection<ItemDefinition> RawPossibleItems { get; } = new List<ItemDefinition>();
    
    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition> PossibleItems
    {
        get => base.PossibleItems ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition, ItemDefinition>(this.RawPossibleItems);
        protected set
        {
            this.PossibleItems.Clear();
            foreach (var item in value)
            {
                this.PossibleItems.Add(item);
            }
        }
    }

    /// <summary>
    /// Gets the raw object of <see cref="Monster" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("monster")]
    public MonsterDefinition RawMonster
    {
        get => base.Monster as MonsterDefinition;
        set => base.Monster = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.DataModel.Configuration.MonsterDefinition Monster
    {
        get => base.Monster;
        set => base.Monster = value;
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.DropItemGroup Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new DropItemGroup();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.DropItemGroup other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
    public DropItemGroup Convert() => this;
}
