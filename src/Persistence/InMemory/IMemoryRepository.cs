// <copyright file="IMemoryRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Interface for a memory repository, which allows to modify its stored items.
/// </summary>
public interface IMemoryRepository : IRepository
{
    /// <summary>
    /// Adds the specified object with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="obj">The object.</param>
    void Add(Guid key, object obj);

    /// <summary>
    /// Removes the object with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    ValueTask RemoveAsync(Guid key);

    /// <summary>
    /// Called when the context saves the changes.
    /// </summary>
    void OnSaveChanges();
}