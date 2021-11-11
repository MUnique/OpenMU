// <copyright file="QuestReward.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward"/>.
/// </summary>
[Table(nameof(QuestReward), Schema = SchemaNames.Configuration)]
internal partial class QuestReward : MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of <see cref="ItemReward"/>.
    /// </summary>
    public Guid? ItemRewardId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="ItemReward" />.
    /// </summary>
    [ForeignKey(nameof(ItemRewardId))]
    public Item RawItemReward
    {
        get => base.ItemReward as Item;
        set => base.ItemReward = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Entities.Item ItemReward
    {
        get => base.ItemReward;set
        {
            base.ItemReward = value;
            this.ItemRewardId = this.RawItemReward?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="AttributeReward"/>.
    /// </summary>
    public Guid? AttributeRewardId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="AttributeReward" />.
    /// </summary>
    [ForeignKey(nameof(AttributeRewardId))]
    public AttributeDefinition RawAttributeReward
    {
        get => base.AttributeReward as AttributeDefinition;
        set => base.AttributeReward = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.AttributeSystem.AttributeDefinition AttributeReward
    {
        get => base.AttributeReward;set
        {
            base.AttributeReward = value;
            this.AttributeRewardId = this.RawAttributeReward?.Id;
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
