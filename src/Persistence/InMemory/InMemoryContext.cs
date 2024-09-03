// <copyright file="InMemoryContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

using System.Collections;
using System.Threading;
using Nito.AsyncEx.Synchronous;
using Nito.Disposables;

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
    /// Occurs when changes have been "saved".
    /// </summary>
    public event EventHandler? SavedChanges;

    /// <inheritdoc />
    public bool HasChanges => false;

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
        this.SavedChanges = null;
    }

    /// <summary>
    /// Saves the changes of the context.
    /// </summary>
    /// <returns><c>True</c>, if the saving was successful; <c>false</c>, otherwise.</returns>
    public bool SaveChanges()
    {
        foreach (var repository in this.Provider.MemoryRepositories)
        {
            repository.OnSaveChanges();
        }

        return true;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = this.SaveChanges();
        if (result)
        {
            this.SavedChanges?.Invoke(this, EventArgs.Empty);
        }

        return result;
    }

    /// <inheritdoc />
    public IDisposable SuspendChangeNotifications()
    {
        return new Disposable(() => { });
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
                identifiable.Id = GuidV7.NewGuid();
            }

            var repository = this.Provider.GetRepository<T>() as IMemoryRepository;
            repository?.Add(identifiable.Id, newObject);
        }

        return newObject;
    }

    /// <inheritdoc/>
    public object CreateNew(Type type, params object?[] args)
    {
        var newObject = typeof(Persistence.BasicModel.GameConfiguration).Assembly.CreateNew(type, args);
        if (newObject is IIdentifiable identifiable)
        {
            if (identifiable.Id == Guid.Empty)
            {
                identifiable.Id = GuidV7.NewGuid();
            }

            var repository = this.Provider.GetRepository(type) as IMemoryRepository;
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
    public async Task<T?> GetByIdAsync<T>(Guid id, CancellationToken cancellationToken)
        where T : class
    {
        return await this.Provider.GetRepository<T>().GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<object?> GetByIdAsync(Guid id, Type type, CancellationToken cancellationToken)
    {
        return await this.Provider.GetRepository(type).GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable<T>> GetAsync<T>(CancellationToken cancellationToken)
        where T : class
    {
        return this.Provider.GetRepository<T>().GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public ValueTask<IEnumerable> GetAsync(Type type, CancellationToken cancellationToken)
    {
        return this.Provider.GetRepository(type).GetAllAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public bool IsSupporting(Type type)
    {
        return true;
    }
}