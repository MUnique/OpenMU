// <copyright file="InMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx.Synchronous;

namespace MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// An in-memory context which get it's data from the repositories of the <see cref="InMemoryPersistenceContextProvider"/>.
/// </summary>
public class InMemoryContext : IContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryContext"/> class.
    /// </summary>
    /// <param name="provider">The manager which holds the memory repositories.</param>
    public InMemoryContext(InMemoryRepositoryProvider provider)
    {
        this.Provider = provider;
    }

    /// <summary>
    /// Gets the manager which holds the memory repositories.
    /// </summary>
    /// <value>
    /// The manager.
    /// </value>
    protected InMemoryRepositoryProvider Provider { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        // nothing to do here
    }

    public bool HasChanges => false;

    /// <inheritdoc/>
    public bool SaveChanges()
    {
        foreach (var repository in this.Provider.MemoryRepositories)
        {
            repository.OnSaveChanges();
        }

        return true;
    }

    /// <inheritdoc/>
    public ValueTask<bool> SaveChangesAsync()
    {
        return ValueTask.FromResult(this.SaveChanges());
    }

    /// <inheritdoc/>
    public bool Detach(object item)
    {
        if (item is IIdentifiable identifiable)
        {
            var repository = this.Provider.GetRepository(item.GetType()) as IMemoryRepository;
            repository?.RemoveAsync(identifiable.Id).AsTask().WaitWithoutException();
        }

        return false;
    }

    /// <inheritdoc/>
    public void Attach(object item)
    {
        if (item is IIdentifiable identifiable)
        {
            var repository = this.Provider.GetRepository(item.GetType()) as IMemoryRepository;
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

            var repository = this.Provider.GetRepository<T>() as IMemoryRepository;
            repository?.Add(identifiable.Id, newObject);
        }

        return newObject;
    }

    /// <inheritdoc/>
    public ValueTask<bool> DeleteAsync<T>(T obj)
        where T : class
    {
        return this.Provider.GetRepository<T>().DeleteAsync(obj);
    }

    /// <inheritdoc/>
    public async Task<T?> GetByIdAsync<T>(Guid id)
        where T : class
    {
        return await this.Provider.GetRepository<T>().GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<object?> GetByIdAsync(Guid id, Type type)
    {
        return await this.Provider.GetRepository(type).GetByIdAsync(id).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<T>> GetAsync<T>()
        where T : class
    {
        return this.Provider.GetRepository<T>().GetAllAsync();
    }
}