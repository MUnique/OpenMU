// <copyright file="QuestMonsterKillRequirementState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Keeps the progress of the <see cref="QuestDefinition.RequiredMonsterKills"/> of the currently active quest.
/// </summary>
public class QuestMonsterKillRequirementState
{
    /// <summary>
    /// Gets or sets the requirement for which this state is kept.
    /// </summary>
    public virtual QuestMonsterKillRequirement? Requirement { get; set; }

    /// <summary>
    /// Gets or sets the monster kill count for this <see cref="Requirement"/>.
    /// </summary>
    public int KillCount { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.KillCount}/{this.Requirement?.MinimumNumber} {this.Requirement?.Monster}";
    }
}