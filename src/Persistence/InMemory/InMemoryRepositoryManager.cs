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
            where T : class
        {
            var repository = this.InternalGetRepository(typeof(T)) ?? this.CreateAndRegisterMemoryRepository<T>();
            return (IRepository<T>)repository;
        }

        private IRepository CreateAndRegisterMemoryRepository<T>()
        {
            var baseModelAssembly = typeof(GameConfiguration).Assembly;
            var persistentType = baseModelAssembly.GetPersistentTypeOf<T>() ?? typeof(T);
            var repositoryType = typeof(MemoryRepository<>).MakeGenericType(persistentType);
            var repository = (IRepository)Activator.CreateInstance(repositoryType)!;
            var baseType = typeof(T).Assembly == baseModelAssembly ? typeof(T).BaseType ?? typeof(T) : typeof(T);
            this.RegisterRepository(baseType, repository!);
            return repository;
        }
    }
}