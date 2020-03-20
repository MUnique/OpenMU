// <copyright file="PersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.Logging;

    /// <summary>
    /// The persistence context provider for the persistence implemented with entity framework core.
    /// </summary>
    public class PersistenceContextProvider : IPersistenceContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.
        /// </summary>
        public PersistenceContextProvider()
        {
            this.CachingRepositoryManager = new CachingRepositoryManager();
            this.CachingRepositoryManager.RegisterRepositories();
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        /// <value>
        /// The repository manager.
        /// </value>
        internal CachingRepositoryManager CachingRepositoryManager { get; }

        /// <summary>
        /// Initializes the logging of sql statements.
        /// </summary>
        public static void InitializeSqlLogging()
        {
            NpgsqlLogManager.Provider = new NpgsqlLog4NetLoggingProvider();
        }

        /// <summary>
        /// Determines whether the database schema is up to date.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is database up to date]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDatabaseUpToDate()
        {
            using var installationContext = new EntityDataContext();
            return !installationContext.Database.GetPendingMigrations().Any();
        }

        /// <summary>
        /// Applies all pending updates to the database schema.
        /// </summary>
        public void ApplyAllPendingUpdates()
        {
            using var installationContext = new EntityDataContext();
            installationContext.Database.Migrate();
        }

        /// <summary>
        /// Determines if the database exists already, by checking if any migration has been applied.
        /// </summary>
        /// <returns><c>True</c>, if the database exists; Otherwise, <c>false</c>.</returns>
        public bool DatabaseExists()
        {
            using var installationContext = new EntityDataContext();
            return installationContext.Database.GetAppliedMigrations().Any();
        }

        /// <summary>
        /// Recreates the database by deleting and creating it again.
        /// </summary>
        public void ReCreateDatabase()
        {
            using var installationContext = new EntityDataContext();
            installationContext.Database.EnsureDeleted();
            this.ApplyAllPendingUpdates();
        }

        /// <inheritdoc />
        public IContext CreateNewContext()
        {
            return new CachingEntityFrameworkContext(new EntityDataContext(), this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IContext CreateNewContext(DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new CachingEntityFrameworkContext(new EntityDataContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IPlayerContext CreateNewPlayerContext(DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new PlayerContext(new AccountContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IContext CreateNewConfigurationContext()
        {
            return new CachingEntityFrameworkContext(new ConfigurationContext(), this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IContext CreateNewTradeContext()
        {
            return new CachingEntityFrameworkContext(new TradeContext(), this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IFriendServerContext CreateNewFriendServerContext()
        {
            return new FriendServerContext(new FriendContext(), this.CachingRepositoryManager);
        }

        /// <inheritdoc/>
        public IGuildServerContext CreateNewGuildContext()
        {
            return new GuildServerContext(new GuildContext(), this.CachingRepositoryManager);
        }

        /// <inheritdoc />
        public IContext CreateNewTypedContext<T>()
        {
            return new EntityFrameworkContext(new TypedContext<T>());
        }
    }
}