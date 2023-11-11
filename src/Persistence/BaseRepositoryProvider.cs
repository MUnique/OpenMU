// <copyright file="BaseRepositoryProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

/// <summary>
/// The base repository provider.
/// </summary>
public class BaseRepositoryProvider : IRepositoryProvider
{
    private bool _isInitialized;

    /// <summary>
    /// Gets the repositories for each entity type.
    /// </summary>
    protected IDictionary<Type, object> Repositories { get; } = new Dictionary<Type, object>();

    /// <summary>
    /// Gets the repository of the specified generic type.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <returns>The repository of the specified generic type.</returns>
    public virtual IRepository<T>? GetRepository<T>()
        where T : class
    {
        var repository = this.GetRepository(typeof(T));
        if (repository is null)
        {
            return null;
        }

        // TODO: Not always an adapter is required. Also, the adapter could be cached.
        return new RepositoryAdapter<T>(repository);
    }

    /// <summary>
    /// Gets the repository of the specified generic type.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <typeparam name="TRepository">The type of the repository.</typeparam>
    /// <returns>
    /// The repository of the specified generic type.
    /// </returns>
    public TRepository? GetRepository<T, TRepository>()
        where T : class
        where TRepository : IRepository
    {
        return (TRepository?)this.GetRepository(typeof(T));
    }

    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The repository of the specified type.</returns>
    public virtual IRepository? GetRepository(Type objectType)
    {
        var repository = this.InternalGetRepository(objectType);
        return repository;
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    protected virtual void Initialize()
    {
    }

    /// <summary>
    /// Gets the repository of the specified type.
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <returns>The repository of the specified type.</returns>
    protected IRepository? InternalGetRepository(Type objectType)
    {
        this.EnsureInitialized();
        Type? currentSearchType = objectType;
        do
        {
            if (currentSearchType is null)
            {
                break;
            }

            if (this.Repositories.TryGetValue(currentSearchType, out var repository))
            {
                return repository as IRepository;
            }

            if (currentSearchType.Name != currentSearchType.BaseType?.Name)
            {
                break;
            }

            currentSearchType = currentSearchType.BaseType;
        }
        while (currentSearchType != typeof(object));

        return null;
    }

    /// <summary>
    /// Registers the repository.
    /// </summary>
    /// <typeparam name="T">The generic type which the repository handles.</typeparam>
    /// <param name="repository">The repository.</param>
    protected void RegisterRepository<T>(IRepository<T> repository)
        where T : class
    {
        this.RegisterRepository(typeof(T), repository);
    }

    /// <summary>
    /// Registers the repository.
    /// </summary>
    /// <param name="type">The generic type which the repository handles.</param>
    /// <param name="repository">The repository.</param>
    protected virtual void RegisterRepository(Type type, IRepository repository)
    {
        this.Repositories.Add(type, repository);
    }

    private void EnsureInitialized()
    {
        if (!this._isInitialized)
        {
            this.Initialize();
            this._isInitialized = true;
        }
    }
}