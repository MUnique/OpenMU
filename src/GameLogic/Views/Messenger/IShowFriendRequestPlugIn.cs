// <copyright file="IShowFriendRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about an incoming friend request.
    /// </summary>
    public interface IShowFriendRequestPlugIn : IViewPlugIn
    {
        /// <summary>
        /// Shows the friend request.
        /// </summary>
        /// <param name="requester">The character name of the requesting player.</param>
        void ShowFriendRequest(string requester);
    }
}