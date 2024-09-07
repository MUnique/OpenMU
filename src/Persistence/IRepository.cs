// <copyright file="IRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence;

using System.Collections;
using System.Threading;

/// <summary>
/// A base repository which can return an object by an id.
/// </summary>
public interface IRepository
{
    /// <summary>
    /// Gets the object by an identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The loaded object, or null if not found.
    /// </returns>
    ValueTask<object?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified object when the unit of work is saved.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>The success of the delete operation.</returns>
    ValueTask<bool> DeleteAsync(object obj);

    /// <summary>
    /// Deletes the object with the specified identifier when the unit of work is saved.
    /// </summary>
    /// <param name="id">The identifier of the object which should be deleted.</param>
    /// <returns>The success.</returns>
    ValueTask<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Gets all objects.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// All objects of the repository.
    /// </returns>
    ValueTask<IEnumerable> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Description of IRepository.
/// </summary>
/// <typeparam name="T">The type which this repository handles.</typeparam>
public interface IRepository<T> : IRepository
    where T : class
{
    /// <summary>
    /// Gets all objects.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// All objects of the repository.
    /// </returns>
    new ValueTask<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an object by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The object with the identifier.
    /// </returns>
    new ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}