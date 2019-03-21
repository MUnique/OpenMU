// <copyright file="IChatRoomCreatedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Messenger
{
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Interface of a view whose implementation informs about a created chat room which can be joined.
    /// </summary>
    public interface IChatRoomCreatedPlugIn : IViewPlugIn
    {
        /// <summary>
        /// A chat room has been created for this player and his friend.
        /// </summary>
        /// <param name="authenticationInfo">The id of the created chat room.</param>
        /// <param name="friendName">The character name of the friend which is expected in the chat room.</param>
        /// <param name="success">If set to <c>true</c>, the chat room has been created successfully; Otherwise not.</param>
        void ChatRoomCreated(ChatServerAuthenticationInfo authenticationInfo, string friendName, bool success);
    }
}