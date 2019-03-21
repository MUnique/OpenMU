// <copyright file="IPartyHealthViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Party
{
    /// <summary>
    /// The view plugin for party information.
    /// </summary>
    public interface IPartyHealthViewPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the party health.
        /// </summary>
        void UpdatePartyHealth();

        /// <summary>
        /// Determines whether a health update is needed.
        /// </summary>
        /// <returns>If set to <c>true</c>, a health update is needed; Otherwise not.</returns>
        bool IsHealthUpdateNeeded();
    }
}
