// <copyright file="InMemoryRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    /// <summary>
    /// A repository manager which creates new in-memory repositories on-demand.
    /// </summary>
    public class InMemoryRepositoryManager : BaseRepositoryManager
    {
        /// <summary>
        /// Gets the memory repository, and creates it if it wasn't created yet.
        /// </summary>
        /// <typeparam name="T">The type of the business object.</typeparam>
        /// <returns>The memory repository.</returns>
        public new MemoryRepository<T> GetRepository<T>()
        {
            return this.InternalGetRepository(typeof(T)) as MemoryRepository<T>
                   ?? this.CreateAndRegisterMemoryRepository<T>();
        }

        private MemoryRepository<T> CreateAndRegisterMemoryRepository<T>()
        {
            var repository = new MemoryRepository<T>();
            this.RegisterRepository(repository);
            return repository;
        }
    }
}