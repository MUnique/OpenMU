// <copyright file="IShowFriendInvitationResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about the success of sending a friend invitation.
    /// </summary>
    public interface IShowFriendInvitationResultPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the friend invitation result.
        /// </summary>
        /// <param name="result">If set to <c>true</c>, the invitation has been sent to the invited player.</param>
        /// <param name="requestId">The request identifier.</param>
        void ShowFriendInvitationResult(bool result, uint requestId);
    }
}