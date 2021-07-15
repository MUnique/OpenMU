// <copyright file="InMemoryPersistenceContextProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    /// <summary>
    /// A context provider which uses in-memory repositories to hold its data, e.g. for testing or demo purposes.
    /// Changes in one context directly have effect in other contexts! Calling SaveChanges or not doesn't matter.
    /// </summary>
    public class InMemoryPersistenceContextProvider : IPersistenceContextProvider
    {
        private readonly InMemoryRepositoryManager repositoryManager = new ();

        /// <inheritdoc/>
        public IContext CreateNewContext()
        {
            return new InMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc/>
        public IContext CreateNewContext(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new InMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc/>
        public IContext CreateNewTradeContext()
        {
            return new InMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc/>
        public IPlayerContext CreateNewPlayerContext(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
        {
            return new PlayerInMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc/>
        public IContext CreateNewConfigurationContext()
        {
            return new InMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc />
        public IFriendServerContext CreateNewFriendServerContext()
        {
            return new FriendServerInMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc/>
        public IGuildServerContext CreateNewGuildContext()
        {
            return new GuildServerInMemoryContext(this.repositoryManager);
        }

        /// <inheritdoc />
        public IContext CreateNewTypedContext<T>()
        {
            return new InMemoryContext(this.repositoryManager);
        }
    }
}
