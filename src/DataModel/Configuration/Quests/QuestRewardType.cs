// <copyright file="QuestRewardType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Quests;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Defines the type of the reward of a quest.
/// </summary>
public enum QuestRewardType
{
    /// <summary>
    /// Undefined quest reward. Rewards nothing.
    /// </summary>
    Undefined,

    /// <summary>
    /// The completed quest rewards additional <see cref="Character.Experience"/>.
    /// </summary>
    Experience,

    /// <summary>
    /// The completed quest rewards additional money to the characters inventory.
    /// </summary>
    Money,

    /// <summary>
    /// The completed quest rewards an item one or more times.
    /// </summary>
    Item,

    /// <summary>
    /// The completed quest rewards gens attribution points.
    /// </summary>
    GensAttribution,

    /// <summary>
    /// The completed quest rewards additional <see cref="Character.LevelUpPoints"/>.
    /// </summary>
    LevelUpPoints,

    /// <summary>
    /// When completing the quest, the <see cref="Character.CharacterClass"/> evolves from the first to the second generation.
    /// </summary>
    CharacterEvolutionFirstToSecond,

    /// <summary>
    /// When completing the quest, the <see cref="Character.CharacterClass"/> evolves from the second to the third generation (master classes).
    /// </summary>
    CharacterEvolutionSecondToThird,

    /// <summary>
    /// The completed quest rewards an additional attribute with the specified value.
    /// </summary>
    /// <remarks>
    /// For example, it could mean to add an attribute "Completed Marlon Quest" or "Combo Skill Available" which could
    /// further be a requirement for skills etc. With the the definition of an attribute, we have maximum flexibility.
    /// </remarks>
    Attribute,

    /// <summary>
    /// The completed quest rewards an additional skill, if not yet learned.
    /// </summary>
    Skill,
}