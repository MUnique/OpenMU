// <copyright file="RepositoryAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;

/// <summary>
/// An adapter which takes an untyped <see cref="IRepository"/> and implements a generic <see cref="IRepository{T}"/> on top.
/// </summary>
/// <typeparam name="T">The target type of the repository.</typeparam>
public class RepositoryAdapter<T> : IRepository<T>
    where T : class
{
    private readonly IRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryAdapter{T}"/> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public RepositoryAdapter(IRepository repository)
    {
        this._repository = repository;
    }

    /// <inheritdoc />
    public ValueTask<bool> DeleteAsync(object obj)
    {
        return this._repository.DeleteAsync(obj);
    }

    /// <inheritdoc />
    public ValueTask<bool> DeleteAsync(Guid id)
    {
        return this._repository.DeleteAsync(id);
    }

    /// <inheritdoc />
    ValueTask<IEnumerable> IRepository.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public async ValueTask<IEnumerable<T>> GetAllAsync()
    {
        return (IEnumerable<T>)await this._repository.GetAllAsync();
    }

    /// <inheritdoc />
    public async ValueTask<T?> GetByIdAsync(Guid id)
    {
        return (T?)await this._repository.GetByIdAsync(id);
    }

    /// <inheritdoc />
    ValueTask<object?> IRepository.GetByIdAsync(Guid id)
    {
        return this._repository.GetByIdAsync(id);
    }
}