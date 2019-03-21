// <copyright file="IFriendAddedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    /// <summary>
    /// Interface of a view whose implementation informs about an added friend.
    /// </summary>
    public interface IFriendAddedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A friend has been added to the friend list.
        /// </summary>
        /// <param name="friendName">The character name of the friend.</param>
        void FriendAdded(string friendName);
    }
}