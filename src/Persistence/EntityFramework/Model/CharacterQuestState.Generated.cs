// <copyright file="CharacterQuestState.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Entities.CharacterQuestState"/>.
/// </summary>
[Table(nameof(CharacterQuestState), Schema = SchemaNames.AccountData)]
internal partial class CharacterQuestState : MUnique.OpenMU.DataModel.Entities.CharacterQuestState, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="RequirementStates" />.
    /// </summary>
    public ICollection<QuestMonsterKillRequirementState> RawRequirementStates { get; } = new EntityFramework.List<QuestMonsterKillRequirementState>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Entities.QuestMonsterKillRequirementState> RequirementStates => base.RequirementStates ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Entities.QuestMonsterKillRequirementState, QuestMonsterKillRequirementState>(this.RawRequirementStates);

    /// <summary>
    /// Gets or sets the identifier of <see cref="LastFinishedQuest"/>.
    /// </summary>
    public Guid? LastFinishedQuestId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="LastFinishedQuest" />.
    /// </summary>
    [ForeignKey(nameof(LastFinishedQuestId))]
    public QuestDefinition RawLastFinishedQuest
    {
        get => base.LastFinishedQuest as QuestDefinition;
        set => base.LastFinishedQuest = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.Quests.QuestDefinition LastFinishedQuest
    {
        get => base.LastFinishedQuest;set
        {
            base.LastFinishedQuest = value;
            this.LastFinishedQuestId = this.RawLastFinishedQuest?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="ActiveQuest"/>.
    /// </summary>
    public Guid? ActiveQuestId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="ActiveQuest" />.
    /// </summary>
    [ForeignKey(nameof(ActiveQuestId))]
    public QuestDefinition RawActiveQuest
    {
        get => base.ActiveQuest as QuestDefinition;
        set => base.ActiveQuest = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.Quests.QuestDefinition ActiveQuest
    {
        get => base.ActiveQuest;set
        {
            base.ActiveQuest = value;
            this.ActiveQuestId = this.RawActiveQuest?.Id;
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
