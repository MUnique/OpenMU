// <copyright file="ISkillListViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// Interface of a view whose implementation informs about the available skills.
    /// </summary>
    public interface ISkillListViewPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Adds the skill to the skill list.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void AddSkill(Skill skill);

        /// <summary>
        /// Removes the skill from the skill list.
        /// </summary>
        /// <param name="skill">The skill.</param>
        void RemoveSkill(Skill skill);

        /// <summary>
        /// Updates the skill list.
        /// </summary>
        void UpdateSkillList();

        /// <summary>
        /// Gets the skill by its index in the internal skill list.
        /// </summary>
        /// <param name="skillIndex">Index of the skill.</param>
        /// <returns>The skill by its index in the internal skill list.</returns>
        Skill GetSkillByIndex(byte skillIndex);
    }
}
