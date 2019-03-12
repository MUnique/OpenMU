// <copyright file="IPartyViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    /// <summary>
    /// The view plugin for party information.
    /// </summary>
    public interface IPartyViewPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the party request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowPartyRequest(IPartyMember requester);

        /// <summary>
        /// Updates the party list.
        /// </summary>
        void UpdatePartyList();

        /// <summary>
        /// Updates the party health.
        /// </summary>
        void UpdatePartyHealth();

        /// <summary>
        /// Determines whether a health update is needed.
        /// </summary>
        /// <returns>If set to <c>true</c>, a health update is needed; Otherwise not.</returns>
        bool IsHealthUpdateNeeded();

        /// <summary>
        /// Delete Member from Party.
        /// </summary>
        /// <param name="index">index of the player</param>
        void PartyMemberDelete(byte index);
    }
}
