// <copyright file="IRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;

namespace MUnique.OpenMU.Persistence
{
    using System;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Description of IRepositoryManager.
    /// </summary>
    public interface IRepositoryManager
    {
        /// <summary>
        /// Gets the repository of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <returns>The repository of the specified generic type.</returns>
        IRepository<T> GetRepository<T>();

        /// <summary>
        /// Gets the repository of the specified type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>The repository of the specified type.</returns>
        IRepository GetRepository(Type objectType);

        /// <summary>
        /// Gets the specified repository of the specified generic type.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <returns>The repository of the specified generic type.</returns>
        TRepository GetRepository<T, TRepository>()
            where TRepository : class;

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
        /// Creates a new context which is used for accounts.
        /// This context should only care about the objects of a player;
        /// It should not track changes of configuration objects.
        /// </summary>
        /// <param name="gameConfiguration">The game configuration.</param>
        /// <returns>
        /// The newly created account context.
        /// </returns>
        IContext CreateNewAccountContext(GameConfiguration gameConfiguration);

        /// <summary>
        /// Creates a new context which is used by the friend server.
        /// It manages basically only <see cref="FriendViewItem"/>s.
        /// </summary>
        /// <returns>A new context which is used by the friend server.</returns>
        IContext CreateNewFriendServerContext();

        /// <summary>
        /// Creates a new context which is used by the guild server.
        /// It manages basically only <see cref="Guild"/>s and <see cref="GuildMember"/>s.
        /// </summary>
        /// <returns>A new context which is used by the guild server.</returns>
        IContext CreateNewGuildContext();

        /// <summary>
        /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
        /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The disposable to end the usage.</returns>
        IDisposable UseContext(IContext context);

        /// <summary>
        /// Gets the current context of the current thread.
        /// </summary>
        /// <returns>The current context.</returns>
        IContext GetCurrentContext();

        /// <summary>
        /// Creates a new instance of <typeparamref name="T" />.
        /// Attention: This operation needs a currently used context in the current thread!.
        /// </summary>
        /// <typeparam name="T">The type which should get created.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// A new instance of <typeparamref name="T" />.
        /// </returns>
        T CreateNew<T>(params object[] args)
            where T : class;
    }
}
