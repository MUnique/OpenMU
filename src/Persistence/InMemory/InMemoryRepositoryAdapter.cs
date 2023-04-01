// <copyright file="InMemoryRepositoryAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// A <see cref="RepositoryAdapter{T}"/> which implements <see cref="IMemoryRepository"/>.
/// </summary>
/// <typeparam name="T">The type of the saved objects.</typeparam>
public class InMemoryRepositoryAdapter<T> : RepositoryAdapter<T>, IMemoryRepository
    where T : class
{
    private readonly IMemoryRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryRepositoryAdapter{T}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public InMemoryRepositoryAdapter(IMemoryRepository repository)
        : base(repository)
    {
        this._repository = repository;
    }

    /// <inheritdoc />
    public void Add(Guid key, object obj)
    {
        this._repository.Add(key, obj);
    }

    /// <inheritdoc />
    public ValueTask RemoveAsync(Guid key)
    {
        return this._repository.RemoveAsync(key);
    }

    /// <inheritdoc />
    public void OnSaveChanges()
    {
        this._repository.OnSaveChanges();
    }
}