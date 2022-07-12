// <copyright file="InMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// An in-memory context which get it's data from the repositories of the <see cref="InMemoryPersistenceContextProvider"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Persistence.IContext" />
public class InMemoryContext : IContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
    /// </summary>
    /// <param name="manager">The manager which holds the memory repositories.</param>
    public InMemoryContext(InMemoryRepositoryManager manager)
    {
        this.Manager = manager;
    }

    /// <summary>
    /// Gets the manager which holds the memory repositories.
    /// </summary>
    /// <value>
    /// The manager.
    /// </value>
    protected InMemoryRepositoryManager Manager { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        // nothing to do here
    }

    /// <inheritdoc/>
    public bool SaveChanges()
    {
        return true;
    }

    /// <inheritdoc/>
    public ValueTask<bool> SaveChangesAsync()
    {
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc/>
    public bool Detach(object item)
    {
        if (item is IIdentifiable identifiable)
        {
            var repository = this.Manager.GetRepository(item.GetType()) as IMemoryRepository;
            repository?.Remove(identifiable.Id);
        }

        return false;
    }

    /// <inheritdoc/>
    public void Attach(object item)
    {
        if (item is IIdentifiable identifiable)
        {
            var repository = this.Manager.GetRepository(item.GetType()) as IMemoryRepository;
            repository?.Add(identifiable.Id, item);
        }
    }

    /// <inheritdoc/>
    public T CreateNew<T>(params object?[] args)
        where T : class
    {
        var newObject = typeof(Persistence.BasicModel.GameConfiguration).Assembly.CreateNew<T>(args);
        if (newObject is IIdentifiable identifiable)
        {
            if (identifiable.Id == Guid.Empty)
            {
                identifiable.Id = Guid.NewGuid();
            }

            var repository = this.Manager.GetRepository<T>() as IMemoryRepository;
            repository?.Add(identifiable.Id, newObject);
        }

        return newObject;
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync<T>(T obj)
        where T : class
    {
        return this.Manager.GetRepository<T>()?.DeleteAsync(obj) ?? ValueTask.FromResult(false);
    }

    /// <inheritdoc/>
    public ValueTask<T?> GetByIdAsync<T>(Guid id)
        where T : class
    {
        return this.Manager.GetRepository<T>().GetByIdAsync(id);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<T>> GetAsync<T>()
        where T : class
    {
        return this.Manager.GetRepository<T>().GetAllAsync();
    }
}