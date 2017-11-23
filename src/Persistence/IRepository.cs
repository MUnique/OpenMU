// <copyright file="IRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A base repository which can return an object by an id.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets the object by an identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The object.</returns>
        object GetById(Guid id);
    }

    /// <summary>
    /// Description of IRepository.
    /// </summary>
    /// <typeparam name="T">The type which this repository handles.</typeparam>
    public interface IRepository<out T> : IRepository
    {
        /// <summary>
        /// Gets all objects.
        /// </summary>
        /// <returns>All objects of the repository.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets an object by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The object with the identifier.</returns>
        new T GetById(Guid id);

        /// <summary>
        /// Deletes the specified object when the unit of work is saved.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The success.</returns>
        bool Delete(object obj);

        /// <summary>
        /// Deletes the object with the specified identifier when the unit of work is saved.
        /// </summary>
        /// <param name="id">The identifier of the object which should be deleted.</param>
        /// <returns>The success.</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// A repository for guilds.
    /// </summary>
    /// <typeparam name="T">The type for guilds. The type is not specified here, because the data model is not known to this assembly.</typeparam>
    public interface IGuildRepository<out T> : IRepository<T>
    {
        /// <summary>
        /// Returns if the guild with the specified name exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>True, if the guild with the specified name exists.</returns>
        bool GuildWithNameExists(string name);
    }

    /// <summary>
    /// A repository for accounts.
    /// </summary>
    /// <typeparam name="T">The type for accounts. The type is not specified here, because the data model is not known to this assembly.</typeparam>
    public interface IAccountRepository<out T> : IRepository<T>
    {
        /// <summary>
        /// Gets the account by login name if the password is correct.
        /// </summary>
        /// <param name="loginName">The login name.</param>
        /// <param name="password">The password.</param>
        /// <returns>The account, if the password is correct. Otherwise, null.</returns>
        T GetAccountByLoginName(string loginName, string password);
    }

    /// <summary>
    /// A repository for friend view items.
    /// </summary>
    /// <typeparam name="T">The type for a friend view item. The type is not specified here, because the data model is not known to this assembly.</typeparam>
    public interface IFriendViewItemRepository<out T> : IRepository<T>
    {
        /// <summary>
        /// Gets the friend view item by friend.
        /// </summary>
        /// <param name="characterName">Name of the character holding the friend view item in its friendlist.</param>
        /// <param name="friendName">Name of the friend.</param>
        /// <returns>The friend view item of the friend.</returns>
        T GetByFriend(string characterName, string friendName);

        /// <summary>
        /// Gets the friends of a character.
        /// </summary>
        /// <param name="characterId">Id of the character.</param>
        /// <returns>The friends of the character.</returns>
        IEnumerable<T> GetFriends(Guid characterId);

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

    /// <summary>
    /// A repository for preview data.
    /// </summary>
    /// <typeparam name="T">The type for preview data. The type is not specified here, because the data model is not known to this assembly.</typeparam>
    public interface IPreviewDataRepository<out T>
    {
        /// <summary>
        /// Gets the preview data by account.
        /// </summary>
        /// <typeparam name="TAccount">The type of the account.</typeparam>
        /// <param name="account">The account.</param>
        /// <returns>The preview data of the account.</returns>
        T[] GetByAccount<TAccount>(TAccount account);
    }
}
