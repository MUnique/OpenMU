// <copyright file="QuestMonsterKillRequirement.Generated.cs" company="MUnique">
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
/// A plain implementation of <see cref="QuestMonsterKillRequirement"/>.
/// </summary>
public partial class QuestMonsterKillRequirement : MUnique.OpenMU.DataModel.Configuration.Quests.QuestMonsterKillRequirement, IIdentifiable, IConvertibleTo<QuestMonsterKillRequirement>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw object of <see cref="Monster" />.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("monster")]
    [System.Text.Json.Serialization.JsonPropertyName("monster")]
    public MonsterDefinition RawMonster
    {
        get => base.Monster as MonsterDefinition;
        set => base.Monster = value;
    }

    /// <inheritdoc/>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.DataModel.Configuration.MonsterDefinition Monster
    {
        get => base.Monster;
        set => base.Monster = value;
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
    public QuestMonsterKillRequirement Convert() => this;
}
