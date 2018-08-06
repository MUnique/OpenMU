// <copyright file="PersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.Logging;

    /// <summary>
    /// The persistence context provider for the persistence implemented with entity framework core.
    /// </summary>
    public class PersistenceContextProvider : IPersistenceContextProvider
    {
        private readonly IDictionary<Thread, Stack<IContext>> contextsPerThread = new Dictionary<Thread, Stack<IContext>>();

        private readonly ReaderWriterLockSlim contextLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Field to detect redundant calls.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceContextProvider"/> class.
        /// </summary>
        public PersistenceContextProvider()
        {
            this.RepositoryManager = new RepositoryManager();
            this.RepositoryManager.RegisterRepositories(this);
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        /// <value>
        /// The repository manager.
        /// </value>
        internal RepositoryManager RepositoryManager { get; }

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
            using (var installationContext = new EntityDataContext())
            {
                return !installationContext.Database.GetPendingMigrations().Any();
            }
        }

        /// <summary>
        /// Applies all pending updates to the database schema.
        /// </summary>
        public void ApplyAllPendingUpdates()
        {
            using (var installationContext = new EntityDataContext())
            {
                installationContext.Database.Migrate();
            }
        }

        /// <summary>
        /// Determines if the database exists already, by checking if any migration has been applied.
        /// </summary>
        /// <returns><c>True</c>, if the database exists; Otherwise, <c>false</c>.</returns>
        public bool DatabaseExists()
        {
            using (var installationContext = new EntityDataContext())
            {
                return installationContext.Database.GetAppliedMigrations().Any();
            }
        }

        /// <summary>
        /// Recreates the database by deleting and creating it again.
        /// </summary>
        public void ReCreateDatabase()
        {
            using (var installationContext = new EntityDataContext())
            {
                installationContext.Database.EnsureDeleted();
                this.ApplyAllPendingUpdates();
            }
        }

        /// <inheritdoc />
        public IContext CreateNewContext()
        {
            return new EntityFrameworkContext(new EntityDataContext(), this);
        }

        /// <inheritdoc />
        public IContext CreateNewContext(DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new EntityFrameworkContext(new EntityDataContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this);
        }

        /// <inheritdoc />
        public IPlayerContext CreateNewPlayerContext(DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new PlayerContext(new AccountContext { CurrentGameConfiguration = gameConfiguration as GameConfiguration }, this);
        }

        /// <inheritdoc />
        public IContext CreateNewConfigurationContext()
        {
            return new EntityFrameworkContext(new ConfigurationContext(), this);
        }

        /// <inheritdoc />
        public IContext CreateNewTradeContext()
        {
            return new EntityFrameworkContext(new TradeContext(), this);
        }

        /// <inheritdoc />
        public IFriendServerContext CreateNewFriendServerContext()
        {
            return new FriendServerContext(new FriendContext(), this);
        }

        /// <inheritdoc/>
        public IGuildServerContext CreateNewGuildContext()
        {
            return new GuildServerContext(new GuildContext(), this);
        }

        /// <summary>
        /// Puts this context on the context stack of the current thread to be used for the upcoming repository actions.
        /// If no context is on the context stack of the current thread, a new temporary context will be used for the action.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The disposable to end the usage.</returns>
        public IDisposable UseContext(IContext context)
        {
            Stack<IContext> contextsOfCurrentThread;
            this.contextLock.EnterWriteLock();
            try
            {
                if (!this.contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread))
                {
                    contextsOfCurrentThread = new Stack<IContext>();
                    this.contextsPerThread.Add(Thread.CurrentThread, contextsOfCurrentThread);
                }
            }
            finally
            {
                this.contextLock.ExitWriteLock();
            }

            contextsOfCurrentThread.Push(context);
            return new ContextPop(contextsOfCurrentThread, this.AfterPop);
        }

        /// <summary>
        /// Gets the current context of the current thread.
        /// </summary>
        /// <returns>The current context.</returns>
        public IContext GetCurrentContext()
        {
            Stack<IContext> contextsOfCurrentThread;
            this.contextLock.EnterReadLock();
            try
            {
                this.contextsPerThread.TryGetValue(Thread.CurrentThread, out contextsOfCurrentThread);
            }
            finally
            {
                this.contextLock.ExitReadLock();
            }

            if (contextsOfCurrentThread != null && contextsOfCurrentThread.Count > 0)
            {
                return contextsOfCurrentThread.Peek();
            }

            return null;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                try
                {
                    this.Dispose(true);
                }
                finally
                {
                    this.isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.contextLock.Dispose();
            }
        }

        private void AfterPop(Stack<IContext> context)
        {
            if (context.Count > 0)
            {
                return;
            }

            this.contextLock.EnterWriteLock();
            try
            {
                this.contextsPerThread.Remove(Thread.CurrentThread);
            }
            finally
            {
                this.contextLock.ExitWriteLock();
            }
        }

        private sealed class ContextPop : IDisposable
        {
            private Stack<IContext> stack;
            private Action<Stack<IContext>> afterPopAction;

            public ContextPop(Stack<IContext> stack, Action<Stack<IContext>> afterPopAction)
            {
                this.stack = stack;
                this.afterPopAction = afterPopAction;
            }

            public void Dispose()
            {
                if (this.stack != null)
                {
                    this.stack.Pop();
                    if (this.afterPopAction != null)
                    {
                        this.afterPopAction(this.stack);
                        this.afterPopAction = null;
                    }

                    this.stack = null;
                }
            }
        }
    }
}