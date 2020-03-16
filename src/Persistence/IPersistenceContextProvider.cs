// <copyright file="IPersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Description of IPersistenceContextProvider.
    /// </summary>
    public interface IPersistenceContextProvider
    {
        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <returns>The newly created context.</returns>
        IContext CreateNewContext();

        /// <summary>
        /// Creates a new context.
        /// </summary>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The newly created context.
        /// </returns>
        IContext CreateNewContext(GameConfiguration gameConfiguration);

        /// <summary>
        /// Creates a new context which should be used to load the configuration.
        /// </summary>
        /// <returns>
        /// The newly created context.
        /// </returns>
        IContext CreateNewConfigurationContext();

        /// <summary>
        /// Creates the new trade context which is used to exchange items in a trade.
        /// </summary>
        /// <returns>The newly created context.</returns>
        IContext CreateNewTradeContext();

        /// <summary>
        /// Creates a new context which is used for accounts.
        /// This context should only care about the objects of a player;
        /// It should not track changes of configuration objects.
        /// </summary>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The newly created account context.
        /// </returns>
        IPlayerContext CreateNewPlayerContext(GameConfiguration gameConfiguration);

        /// <summary>
        /// Creates a new context which is used by the friend server.
        /// It manages basically only <see cref="FriendViewItem"/>s.
        /// </summary>
        /// <returns>A new context which is used by the friend server.</returns>
        IFriendServerContext CreateNewFriendServerContext();

        /// <summary>
        /// Creates a new context which is used by the guild server.
        /// It manages basically only <see cref="MUnique.OpenMU.Interfaces.Guild"/>s and <see cref="GuildMember"/>s.
        /// </summary>
        /// <returns>A new context which is used by the guild server.</returns>
        IGuildServerContext CreateNewGuildContext();

        /// <summary>
        /// Creates the new context which can be used to load and edit an object of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of object which should be handled.</typeparam>
        /// <returns>
        /// A new context which can be used to load and edit an object of <typeparamref name="T" />.
        /// </returns>
        IContext CreateNewTypedContext<T>();
    }
}
