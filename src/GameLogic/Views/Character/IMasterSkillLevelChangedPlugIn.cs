// <copyright file="IMasterSkillLevelChangedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface of a view whose implementation informs about a changed master skill level.
    /// </summary>
    public interface IMasterSkillLevelChangedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Informs the player that the master skill level has been changed (usually increased).
        /// </summary>
        /// <param name="learnedSkill">The learned master skill.</param>
        void MasterSkillLevelChanged(SkillEntry learnedSkill);
    }
}