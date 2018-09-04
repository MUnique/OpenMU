// <copyright file="InMemoryRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory
{
    using System;
    using MUnique.OpenMU.Persistence.BasicModel;

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
        public new IRepository<T> GetRepository<T>()
        {
            var repository = this.InternalGetRepository(typeof(T)) ?? this.CreateAndRegisterMemoryRepository<T>();
            return repository as IRepository<T>;
        }

        private IRepository CreateAndRegisterMemoryRepository<T>()
        {
            var baseModelAssembly = typeof(GameConfiguration).Assembly;
            var persistentType = baseModelAssembly.GetPersistentTypeOf<T>() ?? typeof(T);
            var repositoryType = typeof(MemoryRepository<>).MakeGenericType(persistentType);
            var repository = Activator.CreateInstance(repositoryType) as IRepository;
            var baseType = typeof(T).Assembly == baseModelAssembly ? typeof(T).BaseType : typeof(T);
            this.RegisterRepository(baseType, repository);
            return repository;
        }
    }
}