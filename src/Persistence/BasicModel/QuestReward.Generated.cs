// <copyright file="QuestReward.Generated.cs" company="MUnique">
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
/// A plain implementation of <see cref="QuestReward"/>.
/// </summary>
public partial class QuestReward : MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward, IIdentifiable, IConvertibleTo<QuestReward>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw object of <see cref="ItemReward" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("itemReward")]
    public Item RawItemReward
    {
        get => base.ItemReward as Item;
        set => base.ItemReward = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.DataModel.Entities.Item ItemReward
    {
        get => base.ItemReward;
        set => base.ItemReward = value;
    }

    /// <summary>
    /// Gets the raw object of <see cref="AttributeReward" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("attributeReward")]
    public AttributeDefinition RawAttributeReward
    {
        get => base.AttributeReward as AttributeDefinition;
        set => base.AttributeReward = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.AttributeSystem.AttributeDefinition AttributeReward
    {
        get => base.AttributeReward;
        set => base.AttributeReward = value;
    }

    /// <summary>
    /// Gets the raw object of <see cref="SkillReward" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("skillReward")]
    public Skill RawSkillReward
    {
        get => base.SkillReward as Skill;
        set => base.SkillReward = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.DataModel.Configuration.Skill SkillReward
    {
        get => base.SkillReward;
        set => base.SkillReward = value;
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new QuestReward();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Quests.QuestReward other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
    public QuestReward Convert() => this;
}
