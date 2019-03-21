// <copyright file="IFriendStateUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about a changed online state of a friend.
    /// </summary>
    public interface IFriendStateUpdatePlugIn : IViewPlugIn
    {
        /// <summary>
        /// Updates the online state (server id) of a friend.
        /// </summary>
        /// <param name="friend">The friends character name.</param>
        /// <param name="serverId">The server identifier.</param>
        void FriendStateUpdate(string friend, int serverId);
    }
}