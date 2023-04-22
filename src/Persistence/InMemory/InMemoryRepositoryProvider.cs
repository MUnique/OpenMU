// <copyright file="InMemoryRepositoryManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// A repository provider which creates new in-memory repositories on-demand.
/// </summary>
public class InMemoryRepositoryProvider : BaseRepositoryProvider
{
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
        var repository = this.InternalGetRepository(typeof(T)) ?? this.CreateAndRegisterMemoryRepository(typeof(T));
        return new InMemoryRepositoryAdapter<T>((IMemoryRepository)repository);
    }

    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The repository of the specified type.</returns>
    public new IRepository GetRepository(Type objectType)
    {
        var repository = this.InternalGetRepository(objectType) ?? this.CreateAndRegisterMemoryRepository(objectType);

        return repository;
    }

    private IRepository CreateAndRegisterMemoryRepository(Type type)
    {
        var baseModelAssembly = typeof(GameConfiguration).Assembly;
        var persistentType = baseModelAssembly.GetPersistentTypeOf(type) ?? type;
        var repositoryType = typeof(MemoryRepository<>).MakeGenericType(persistentType);
        var repository = (IRepository)Activator.CreateInstance(repositoryType)!;
        var baseType = type.Assembly == baseModelAssembly ? type.BaseType ?? type : type;
        this.RegisterRepository(baseType, repository!);
        return repository;
    }
}