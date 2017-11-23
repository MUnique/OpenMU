// <copyright file="IPartyView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    /// <summary>
    /// The view for party informations.
    /// </summary>
    public interface IPartyView : IChatView
    {
        /// <summary>
        /// Shows the party request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowPartyRequest(IPartyMember requester);

        /// <summary>
        /// The party has been closed.
        /// </summary>
        void PartyClosed();

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
    }
}
