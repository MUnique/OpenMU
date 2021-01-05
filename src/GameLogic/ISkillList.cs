// <copyright file="ISkillList.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface for a skill list of a character.
    /// </summary>
    public interface ISkillList
    {
        /// <summary>
        /// Gets the skills.
        /// </summary>
        IEnumerable<SkillEntry> Skills { get; }

        /// <summary>
        /// Gets the number of skills in the skill list.
        /// </summary>
        byte SkillCount { get; }

        /// <summary>
        /// Gets the skill with the specified id.
        /// </summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>The skill with the specified id.</returns>
        SkillEntry? GetSkill(ushort skillId);

        /// <summary>
        /// Adds the learned skill.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void AddLearnedSkill(Skill skill);

        /// <summary>
        /// Removes the item skill.
        /// </summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns>The success of removing the skill.</returns>
        bool RemoveItemSkill(ushort skillId);

        /// <summary>
        /// Determines whether the list contains the specified skill of the specified id.
        /// </summary>
        /// <param name="skillId">The skill identifier.</param>
        /// <returns><c>True</c>, if the skill with the specified id is contained in this list; Otherwise, <c>false</c>.</returns>
        bool ContainsSkill(ushort skillId);
    }
}
