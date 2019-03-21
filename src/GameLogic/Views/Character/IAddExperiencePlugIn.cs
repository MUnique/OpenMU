// <copyright file="IAddExperiencePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Character
{
    /// <summary>
    /// Interface of a view whose implementation informs about added experience.
    /// </summary>
    public interface IAddExperiencePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Adds Experience after the object has been killed.
        /// </summary>
        /// <param name="gainedExperience">The experience gain.</param>
        /// <param name="killedObject">The killed object.</param>
        void AddExperience(int gainedExperience, IIdentifiable killedObject);
    }
}