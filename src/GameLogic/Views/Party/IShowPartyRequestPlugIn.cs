// <copyright file="IShowPartyRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Party
{
    /// <summary>
    /// Interface of a view whose implementation informs about a party request from another player.
    /// </summary>
    public interface IShowPartyRequestPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the party request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowPartyRequest(IPartyMember requester);
    }
}