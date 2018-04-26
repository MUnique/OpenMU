// <copyright file="IFriendServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// A context which is used by the <see cref="T:MUnique.OpenMU.Interfaces.IFriendServer" />.
    /// </summary>
    public interface IFriendServerContext : IContext
    {
        /// <summary>
        /// Creates a new friend view item.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="friendName">Name of the friend.</param>
        /// <returns>The created friend view item.</returns>
        Friend CreateNewFriend(string characterName, string friendName);

        /// <summary>
        /// Gets the friend view item by friend.
        /// </summary>
        /// <param name="characterName">Name of the character holding the friend view item in its friendlist.</param>
        /// <param name="friendName">Name of the friend.</param>
        /// <returns>The friend view item of the friend.</returns>
        Friend GetFriendByNames(string characterName, string friendName);

        /// <summary>
        /// Gets the friends of a character.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <returns>The friends of the character.</returns>
        IEnumerable<FriendViewItem> GetFriends(Guid characterId);

        /// <summary>
        /// Gets the friend names of a character.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <returns>The friend names of the character.</returns>
        IEnumerable<string> GetFriendNames(Guid characterId);

        /// <summary>
        /// Deletes the friend with name <paramref name="friendName"/> from the friendlist of <paramref name="characterName"/>.
        /// </summary>
        /// <param name="characterName">Name of the character holding the friend view item in its friendlist.</param>
        /// <param name="friendName">Name of the friend.</param>
        void Delete(string characterName, string friendName);

        /// <summary>
        /// Gets the names from characters which requested a friendship to the character with the specified id and are not answered yet.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <returns>The open friend requester names.</returns>
        IEnumerable<string> GetOpenFriendRequesterNames(Guid characterId);
    }
}