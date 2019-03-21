// <copyright file="IUpdateMasterSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about the current master skill list.
    /// </summary>
    public interface IUpdateMasterSkillsPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Sends the master skill list to the client.
        /// </summary>
        void UpdateMasterSkills();
    }
}