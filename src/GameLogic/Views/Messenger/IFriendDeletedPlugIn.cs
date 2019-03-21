// <copyright file="IFriendDeletedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about a deleted friend.
    /// </summary>
    public interface IFriendDeletedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A friend has been deleted from the friend list.
        /// </summary>
        /// <param name="deletedFriend">The deleted friend.</param>
        void FriendDeleted(string deletedFriend);
    }
}