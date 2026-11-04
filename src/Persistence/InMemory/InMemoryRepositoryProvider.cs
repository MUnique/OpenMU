// <copyright file="InMemoryRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Threading;
using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// A repository provider which creates new in-memory repositories on-demand.
/// </summary>
public class InMemoryRepositoryProvider : BaseRepositoryProvider
{
    private readonly Lock _creationLock = new();

    /// <summary>
    /// Gets all <see cref="IMemoryRepository"/> which were added to this manager.
    /// </summary>
    internal IEnumerable<IMemoryRepository> MemoryRepositories => base.Repositories.Values.OfType<IMemoryRepository>();

    /// <summary>
    /// Gets the memory repository, and creates it if it wasn't created yet.
    /// </summary>
    /// <typeparam name="T">The type of the business object.</typeparam>
    /// <returns>The memory repository.</returns>
    public new IRepository<T> GetRepository<T>()
        where T : class
    {
        var repository = this.GetOrCreateMemoryRepository(typeof(T));
        return new InMemoryRepositoryAdapter<T>((IMemoryRepository)repository);
    }

    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The repository of the specified type.</returns>
    public new IRepository GetRepository(Type objectType)
    {
        var repository = this.GetOrCreateMemoryRepository(objectType);

        return repository;
    }

    private IRepository GetOrCreateMemoryRepository(Type type)
    {
        if (this.InternalGetRepository(type) is { } repository)
        {
            return repository;
        }

        // The repositories are created on demand, possibly by concurrent contexts. Without the lock,
        // both would create one, and the changes made in the one which isn't registered get lost.
        lock (this._creationLock)
        {
            return this.InternalGetRepository(type) ?? this.CreateAndRegisterMemoryRepository(type);
        }
    }

    private IRepository CreateAndRegisterMemoryRepository(Type type)
    {
        var baseModelAssembly = typeof(GameConfiguration).Assembly;
        var persistentType = baseModelAssembly.GetPersistentTypeOf(type) ?? type;
        IRepository repository;
        if (baseModelAssembly.FindPersistentType(persistentType) is { } registeredType)
        {
            repository = registeredType.Accept(MemoryRepositoryFactory.Instance);
        }
        else
        {
            var repositoryType = typeof(MemoryRepository<>).MakeGenericType(persistentType);
            repository = (IRepository)Activator.CreateInstance(repositoryType)!;
        }

        var baseType = type.Assembly == baseModelAssembly ? type.BaseType ?? type : type;
        this.RegisterRepository(baseType, repository!);
        return repository;
    }

    /// <summary>
    /// Creates the <see cref="MemoryRepository{TValue}"/> of a persistent type.
    /// </summary>
    private sealed class MemoryRepositoryFactory : IPersistentTypeVisitor<IRepository>
    {
        public static readonly MemoryRepositoryFactory Instance = new();

        public IRepository Visit<T>()
            where T : class
        {
            return new MemoryRepository<T>();
        }
    }
}